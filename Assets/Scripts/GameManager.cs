using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Steamworks;

public class GameManager : MonoBehaviour {
    public static string[] levelNames = new string[] {
        "Lorelei 1",
        "Lorelei 2",
        "Lorelei 3",
        "Gretchen 1",
        "Gretchen 2",
        "Gretchen 3",
        "Nacht 1",
        "Nacht 2",
        "Nacht 3"
    };
    public static int levelsPerSkin = 3;
    public static int maxLevel = 9;

    public static GameManager instance;

    public Image overlay;
    public Image loadingImage;
    public TMP_Text loadingText;
    [System.NonSerialized]
    public int currentLevel = 0;
    [System.NonSerialized]
    public int numRepeats = 0;

    private int currentLevelDamage = 0;

    public bool isTransitioning = false;

    public bool playSingleMemory = false;
    public static int totalMemories = 12;
    public static int maxMemory = 12;

    public bool finishedGame = false;

    public float volume = 1f;

    [SerializeField]
    private AudioSource audioSource;

    public AudioClip mainTheme;

    public AudioClip roomOpenClip;
    public AudioClip enemyDieClip;

    [System.NonSerialized]
    public int currentMemory = 0;
    private int endMemory = 0;
    [System.NonSerialized]
    public bool[] watchedMemory = new bool[totalMemories];
    private (int, int)[] prereqMemories = new (int, int)[] {
        (0, 1),
        (2, 3),
        (4, 5),
        (6, 6),
        (7, 7),
        (8, 8),
        (9, 9),
        (-1, -1),
        (-1, -1),
    };

    public static int totalMonsterNotes = NoteRepository.monsterNotes.Length;
    public static int totalLoreNotes = NoteRepository.loreNotes.Length;
    [System.NonSerialized]
    public bool[] foundMonsterNotes = new bool[totalMonsterNotes];
    [System.NonSerialized]
    public bool[] foundLoreNotes = new bool[totalLoreNotes];

    // Stats
    public int currentStreak = 0;
    public int highestStreak = 0;
    public int totalWins = 0;
    public int totalDeaths = 0;
    public float fastestTime = 0f;
    public int maxRepeats = 0;

    // Steamworks API

    private Achievement_t[] m_Achievements = new Achievement_t[] {
        new Achievement_t(Achievement.LORELEI_MASTER, "Lorelei Master", ""),
        new Achievement_t(Achievement.GRETCHEN_MASTER, "Gretchen Master", ""),
        new Achievement_t(Achievement.NACHT_MASTER, "Nacht Master", ""),
        new Achievement_t(Achievement.HOARDER, "Hoarder", ""),
        new Achievement_t(Achievement.AUTHOR_DEATH, "Death of the Author", ""),
        new Achievement_t(Achievement.LIBRARIAN, "Librarian", ""),
        new Achievement_t(Achievement.SPEEDRUNNER, "Speedrunner", ""),
        new Achievement_t(Achievement.GOLDEN_TOUCH, "Golden Touch", "")
    };

    protected Callback<UserAchievementStored_t> m_UserAchievementStored;
    protected Callback<UserStatsReceived_t> m_UserStatsReceived;
    private CGameID m_GameID;
    private bool statsReceived = false;

    void Awake() {
        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else {
            Destroy(gameObject);
        }
        for (int i = 0; i < totalMemories; i++) {
            watchedMemory[i] = false;
        }
        overlay.color = Color.clear;
        loadingImage.enabled = false;
        loadingText.text = "";
    }

    private void Start() {
        SetupSteamworks();
        LoadGame();
        BackfillAchievementsIfPossible();
        volume = PlayerPrefs.GetFloat("volume", 1.0f);
        audioSource.volume = volume;
        audioSource.clip = mainTheme;
        audioSource.Play();
    }

    IEnumerator FadeIn() {
        Time.timeScale = 1;
        while (overlay.color.a > 0.0 && Time.deltaTime > 0) {
            float newAlpha = Mathf.Max(0, (float)(overlay.color.a - 6.0f * Time.deltaTime));
            overlay.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }
        overlay.color = Color.clear;
    }

    IEnumerator FadeOut() {
        Time.timeScale = 1;
        while (overlay.color.a < 1.0 && Time.deltaTime > 0) {
            float newAlpha = Mathf.Min(1, (float)(overlay.color.a + 6.0f * Time.deltaTime));
            overlay.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }
        overlay.color = Color.black;
    }

    IEnumerator ToScene(string sceneName) {
        Time.timeScale = 1;
        yield return FadeOut();
        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneName);
        loadingImage.enabled = !loading.isDone;
        loadingText.text = loading.isDone ? "" : "Loading";
        while (!loading.isDone) {
            yield return null;
        }
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);
        loadingImage.enabled = false;
        loadingText.text = "";
        if (sceneName == "Menu") {
            PlayMusic(mainTheme);
        }
        yield return FadeIn();
        isTransitioning = false;
    }

    public void PlaySingleMemory(int memoryIndex) {
        if (isTransitioning) {
            return;
        }
        isTransitioning = true;
        playSingleMemory = true;
        currentMemory = memoryIndex;
        StartCoroutine(ToScene("Memory"));
    }

    public void FinishedPlayingMemory() {
        if (playSingleMemory) {
            GoToMenu();
        }

        if (isTransitioning) {
            return;
        }
        isTransitioning = true;

        if (currentLevel == maxLevel) {
            for (int i = 0; i <= endMemory; i++) {
                watchedMemory[i] = true;
            }
            SaveGame();
            StartCoroutine(ToScene("GameOver"));
        } else {
            if (currentMemory >= endMemory) {
                for (int i = 0; i <= endMemory; i++) {
                    watchedMemory[i] = true;
                }
                SaveGame();
                StartCoroutine(ToScene("Game"));
            } else {
                currentMemory += 1;
                StartCoroutine(ToScene("Memory"));
            }
        }
    }

    public void LoadRunAndStart() {
        int levelToStart = 0;
        int repeats = 0;
        if (File.Exists(Application.persistentDataPath + "/run.save")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/run.save", FileMode.Open);
            RunSave save = (RunSave)bf.Deserialize(file);
            file.Close();

            PlayerManager.instance.LoadGame(save);
            levelToStart = save.runLevel;
            repeats = save.repeats;
        }
        StartGame(levelToStart, repeats);
    }

    public void StartGame(int level, int repeats) {
        if (isTransitioning) {
            return;
        }
        isTransitioning = true;
        currentLevel = level;
        numRepeats = repeats;
        Time.timeScale = 1;
        playSingleMemory = false;
        StartLevelOrMemoryOrEnd();
    }

    public void NextLevel() {
        if (isTransitioning) {
            return;
        }
        isTransitioning = true;
        if (currentLevelDamage == 0) {
            if (currentLevel < 3) {
                UnlockAchievement(Achievement.LORELEI_MASTER);
            } else if (currentLevel < 6) {
                UnlockAchievement(Achievement.GRETCHEN_MASTER);
            } else {
                UnlockAchievement(Achievement.NACHT_MASTER);
            }
        }
        currentLevel += 1;
        StartLevelOrMemoryOrEnd();
    }

    public void RepeatRun() {
        if (isTransitioning) {
            return;
        }
        isTransitioning = true;
        currentLevel = 0;
        numRepeats += 1;
        StartLevelOrMemoryOrEnd();
    }

    private void StartLevelOrMemoryOrEnd() {
        int end1 = watchedMemory.Length - 2;
        int end2 = watchedMemory.Length - 1;
        if (currentLevel == maxLevel) {
            if (!watchedMemory[end1]) {
                currentMemory = end1;
                endMemory = end1;
                StartCoroutine(ToScene("Memory"));
            } else if (!watchedMemory[end2]) {
                currentMemory = end2;
                endMemory = end2;
                StartCoroutine(ToScene("Memory"));
            } else {
                StartCoroutine(ToScene("GameOver"));
            }
            return;
        }
        int startMemory = prereqMemories[currentLevel].Item1;
        if (startMemory >= 0 && !watchedMemory[startMemory]) {
            currentMemory = startMemory;
            endMemory = prereqMemories[currentLevel].Item2;
            StartCoroutine(ToScene("Memory"));
        } else {
            StartCoroutine(ToScene("Game"));
        }
    }

    public void PlayCredits() {
        if (isTransitioning) {
            return;
        }
        isTransitioning = true;
        StartCoroutine(ToScene("Credits"));
    }

    public void GoToMenuOrCredits() {
        if (isTransitioning) {
            return;
        }
        isTransitioning = true;
        if (!finishedGame && watchedMemory[^1]) {
            finishedGame = true;
            StartCoroutine(ToScene("Credits"));
        } else {
            StartCoroutine(ToScene("Menu"));
        }
        SaveGame();
    }

    public void GoToMenu() {
        if (isTransitioning) {
            return;
        }
        isTransitioning = true;
        SaveGame();
        SaveRunIfPossible();
        StartCoroutine(ToScene("Menu"));
    }

    public void GoToGameOver() {
        if (isTransitioning) {
            return;
        }
        isTransitioning = true;
        StartCoroutine(ToScene("GameOver"));
    }

    // Sound manager

    public void PlaySFX(AudioClip clip) {
        audioSource.PlayOneShot(clip, volume);
    }

    public void PlayRoomOpenSFX() {
        PlaySFX(roomOpenClip);
    }

    public void PlayEnemyDieSFX() {
        PlaySFX(enemyDieClip);
    }

    public void PlayMusic(AudioClip clip) {
        if (audioSource.clip != clip) {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    public void AdjustVolume(float increment) {
        volume = Mathf.Max(0, Mathf.Min(1, volume + increment));
        audioSource.volume = volume;
        PlayerPrefs.SetFloat("volume", volume);
    }

    // Steamworks

    public void DidGetHit() {
        currentLevelDamage += 1;
    }

    public void DidStartLevel() {
        currentLevelDamage = 0;
    }

    void SetupSteamworks() {
        if (!SteamManager.Initialized)
            return;
        // Cache the GameID for use in the Callbacks
        m_GameID = new CGameID(SteamUtils.GetAppID());
        SteamUserStats.RequestCurrentStats();
        m_UserStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
    }

    private void OnUserStatsReceived(UserStatsReceived_t pCallback) {
        if (!SteamManager.Initialized)
            return;

        // we may get callbacks for other games' stats arriving, ignore them
        if ((ulong)m_GameID == pCallback.m_nGameID) {
            statsReceived = true;
            // load achievements
            foreach (Achievement_t ach in m_Achievements) {
                bool ret = SteamUserStats.GetAchievement(ach.m_eAchievementID.ToString(), out ach.m_bAchieved);
                if (ret) {
                    ach.m_strName = SteamUserStats.GetAchievementDisplayAttribute(ach.m_eAchievementID.ToString(), "name");
                    ach.m_strDescription = SteamUserStats.GetAchievementDisplayAttribute(ach.m_eAchievementID.ToString(), "desc");
                } else {
                    Debug.LogWarning("SteamUserStats.GetAchievement failed for Achievement " + ach.m_eAchievementID + "\nIs it registered in the Steam Partner site?");
                }
            }
            BackfillAchievementsIfPossible();
        }
    }

    public void CheckSpeedrunnerAchievement() {
        if (fastestTime > 0 && fastestTime <= 1200f) {
            UnlockAchievement(Achievement.SPEEDRUNNER);
        }
    }

    public void CheckLibrarianAchievement() {
        int foundMonster = 0;
        foreach (var found in foundMonsterNotes) {
            if (found) {
                foundMonster += 1;
            }
        }

        int foundLore = 0;
        foreach (var found in foundLoreNotes) {
            if (found) {
                foundLore += 1;
            }
        }

        if (foundMonster == totalMonsterNotes && foundLore == totalLoreNotes) {
            UnlockAchievement(Achievement.LIBRARIAN);
        }
    }

    public void BackfillAchievementsIfPossible() {
        if (PlayerManager.instance.writerDead) {
            UnlockAchievement(Achievement.AUTHOR_DEATH);
        }
        CheckSpeedrunnerAchievement();
        CheckLibrarianAchievement();
    }

    public void UnlockAchievement(Achievement ach_t) {
        if (!SteamManager.Initialized || !statsReceived)
            return;

        foreach (Achievement_t ach in m_Achievements) {
            if (ach.m_eAchievementID == ach_t) {
                print("trying to unlock achievement for: " + ach_t.ToString());
                if (ach.m_bAchieved) {
                    return;
                }
                ach.m_bAchieved = true;
                SteamUserStats.SetAchievement(ach.m_eAchievementID.ToString());
            }
        }

        SteamUserStats.StoreStats();
    }

    public void StoreAchievements() {
        if (!SteamManager.Initialized || !statsReceived)
            return;

        SteamUserStats.StoreStats();
    }

    // File client
    public static bool SavedRunAvailable() {
        return File.Exists(Application.persistentDataPath + "/run.save");
    }

    public static void DeleteSavedRun() {
        File.Delete(Application.persistentDataPath + "/run.save");
    }

    public void SaveRunIfPossible() {
        if (SceneManager.GetActiveScene().name == "Game" && PlayerManager.instance.currentSave != null) {
            RunSave runSave = PlayerManager.instance.currentSave;
            runSave.gameTime = PlayerManager.instance.gameTime;
            runSave.repeats = numRepeats;

            BinaryFormatter bf = new BinaryFormatter();
            FileStream runFile = File.Create(Application.persistentDataPath + "/run.save");
            bf.Serialize(runFile, runSave);
            runFile.Close();
        }
    }

    public void SaveGame() {
        Save save = new Save(
            PlayerManager.instance.lvl,
            watchedMemory,
            PlayerManager.instance.exp,
            PlayerManager.instance.maxHPInc,
            PlayerManager.instance.mpRegenInc,
            PlayerManager.instance.speedInc,
            PlayerManager.instance.attackInc,
            PlayerManager.instance.luckInc,
            PlayerManager.instance.castSpdInc,
            PlayerManager.instance.shotSpdInc,
            PlayerManager.instance.rangeInc,
            PlayerManager.instance.knockbackInc,
            PlayerManager.instance.writerDead,
            finishedGame,
            currentStreak,
            highestStreak,
            totalWins,
            totalDeaths,
            fastestTime,
            maxRepeats
        );

        LoreSave loreSave = new LoreSave(foundMonsterNotes, foundLoreNotes);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/fairyside.save");
        bf.Serialize(file, save);
        file.Close();
        FileStream loreFile = File.Create(Application.persistentDataPath + "/lore.save");
        bf.Serialize(loreFile, loreSave);
        loreFile.Close();
    }

    public void SaveLore() {
        LoreSave loreSave = new LoreSave(foundMonsterNotes, foundLoreNotes);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream loreFile = File.Create(Application.persistentDataPath + "/lore.save");
        bf.Serialize(loreFile, loreSave);
        loreFile.Close();
    }

    public void LoadGame() {
        if (File.Exists(Application.persistentDataPath + "/fairyside.save")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/fairyside.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            PlayerManager.instance.lvl = Mathf.Min(PlayerManager.maxLvl, Mathf.Max(1, save.level));
            PlayerManager.instance.exp = save.xp;
            for (int i = 0; i < Mathf.Min(save.watchedMemory.Length, totalMemories); i++) {
                watchedMemory[i] = save.watchedMemory[i];
            }

            PlayerManager.instance.maxHPInc = save.maxHPInc;
            PlayerManager.instance.mpRegenInc = save.mpRegenInc;
            PlayerManager.instance.speedInc = save.speedInc;
            PlayerManager.instance.attackInc = save.attackInc;
            PlayerManager.instance.luckInc = save.luckInc;
            PlayerManager.instance.castSpdInc = save.castSpdInc;
            PlayerManager.instance.shotSpdInc = save.shotSpdInc;
            PlayerManager.instance.rangeInc = save.rangeInc;
            PlayerManager.instance.knockbackInc = Mathf.Min(6, save.knockbackInc);
            PlayerManager.instance.writerDead = save.writerDead;
            finishedGame = save.finishedGame;
            currentStreak = save.currentStreak;
            highestStreak = save.highestStreak;
            totalWins = save.totalWins;
            totalDeaths = save.totalDeaths;
            fastestTime = save.fastestTime;
            maxRepeats = save.maxRepeats;
        }

        if (File.Exists(Application.persistentDataPath + "/lore.save")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/lore.save", FileMode.Open);
            LoreSave save = (LoreSave)bf.Deserialize(file);
            file.Close();

            for (int i = 0; i < Mathf.Min(save.foundMonsterNotes.Length, totalMonsterNotes); i++) {
                foundMonsterNotes[i] = save.foundMonsterNotes[i];
            }

            for (int i = 0; i < Mathf.Min(save.foundLoreNotes.Length, totalLoreNotes); i++) {
                foundLoreNotes[i] = save.foundLoreNotes[i];
            }

        }
    }
}

public class Achievement_t {
    public Achievement m_eAchievementID;
    public string m_strName;
    public string m_strDescription;
    public bool m_bAchieved;

    /// <summary>
    /// Creates an Achievement. You must also mirror the data provided here in https://partner.steamgames.com/apps/achievements/yourappid
    /// </summary>
    /// <param name="achievement">The "API Name Progress Stat" used to uniquely identify the achievement.</param>
    /// <param name="name">The "Display Name" that will be shown to players in game and on the Steam Community.</param>
    /// <param name="desc">The "Description" that will be shown to players in game and on the Steam Community.</param>
    public Achievement_t(Achievement achievementID, string name, string desc) {
        m_eAchievementID = achievementID;
        m_strName = name;
        m_strDescription = desc;
        m_bAchieved = false;
    }
}

public enum Achievement : int {
    LORELEI_MASTER,
    GRETCHEN_MASTER,
    NACHT_MASTER,
    HOARDER,
    AUTHOR_DEATH,
    LIBRARIAN,
    SPEEDRUNNER,
    GOLDEN_TOUCH
};