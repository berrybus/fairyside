using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
    public static int maxLevel = 6;

    public static GameManager instance;

    public Image overlay;
    public Image loadingImage;
    public TMP_Text loadingText;
    [System.NonSerialized]
    public int currentLevel = 0;

    public bool isTransitioning = false;

    private bool playSingleMemory = false;
    public static int totalMemories = 13;
    public static int maxMemory = 4;

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
        (-1, -1),
        (-1, -1),
        (-1, -1),
        (-1, -1),
        (-1, -1),
        (-1, -1),
        (-1, -1),
    };

    public static int totalMonsterNotes = NoteRepository.monsterNotes.Length;
    public static int totalLoreNotes = NoteRepository.loreNotes.Length;
    [System.NonSerialized]
    public bool[] foundMonsterNotes = new bool[totalMonsterNotes];
    [System.NonSerialized]
    public bool[] foundLoreNotes = new bool[totalLoreNotes];

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
        LoadGame();
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

    public void StartGame() {
        if (isTransitioning) {
            return;
        }
        isTransitioning = true;
        currentLevel = 0;
        Time.timeScale = 1;
        playSingleMemory = false;
        StartLevelOrMemory();
    }

    public void NextLevel() {
        if (isTransitioning) {
            return;
        }
        isTransitioning = true;
        currentLevel += 1;
        currentLevel %= maxLevel;
        StartLevelOrMemory();
    }

    private void StartLevelOrMemory() {
        int startMemory = prereqMemories[currentLevel].Item1;
        if (startMemory >= 0 && !watchedMemory[startMemory]) {
            currentMemory = startMemory;
            endMemory = prereqMemories[currentLevel].Item2;
            StartCoroutine(ToScene("Memory"));
        } else {
            StartCoroutine(ToScene("Game"));
        }
    }

    public void GoToMenu() {
        if (isTransitioning) {
            return;
        }
        isTransitioning = true;
        SaveGame();
        StartCoroutine(ToScene("Menu"));
    }

    public void GoToGameOver() {
        if (isTransitioning) {
            return;
        }
        isTransitioning = true;
        StartCoroutine(ToScene("GameOver"));
    }
    
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

    // File client
    public void SaveGame() {
        Save save = new Save(
            PlayerManager.instance.lvl,
            watchedMemory,
            foundMonsterNotes,
            foundLoreNotes,
            PlayerManager.instance.exp,
            PlayerManager.instance.maxHPInc,
            PlayerManager.instance.mpRegenInc,
            PlayerManager.instance.speedInc,
            PlayerManager.instance.attackInc,
            PlayerManager.instance.luckInc,
            PlayerManager.instance.castSpdInc,
            PlayerManager.instance.shotSpdInc,
            PlayerManager.instance.rangeInc,
            PlayerManager.instance.knockbackInc
        );

    BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/fairyside.save");
        bf.Serialize(file, save);
        file.Close();
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
            if (save.foundMonsterNotes != null && save.foundLoreNotes != null) {
                for (int i = 0; i < Mathf.Min(save.foundMonsterNotes.Length, totalMonsterNotes); i++) {
                    foundMonsterNotes[i] = save.foundMonsterNotes[i];
                }

                for (int i = 0; i < Mathf.Min(save.foundLoreNotes.Length, totalLoreNotes); i++) {
                    foundLoreNotes[i] = save.foundLoreNotes[i];
                }
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
        }
    }
}
