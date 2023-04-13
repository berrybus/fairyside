using System;

[System.Serializable]
public class Save {
    public int level;
    public int xp;
    public int maxHPInc;
    public int mpRegenInc;
    public int speedInc;
    public int attackInc;
    public int luckInc;
    public int castSpdInc;
    public int shotSpdInc;
    public int rangeInc;
    public int knockbackInc;
    public bool writerDead;
    public bool[] watchedMemory;
    public bool finishedGame;
    public int currentStreak = 0;
    public int highestStreak = 0;
    public int totalWins = 0;
    public int totalDeaths = 0;
    public float fastestTime = 0f;
    public int maxRepeats = 0;

    public Save(
        int _level,
        bool[] _memories,
        int _xp,
        int _maxHPInc,
        int _mpRegenInc,
        int _speedInc,
        int _attackInc,
        int _luckInc,
        int _castSpdInc,
        int _shotSpdInc,
        int _rangeInc,
        int _knockbackInc,
        bool _writerDead,
        bool _finishedGame,
        int _currentStreak,
        int _highestStreak,
        int _totalWins,
        int _totalDeaths,
        float _fastestTime,
        int _maxRepeats
    ) {
        watchedMemory = _memories;
        level = _level;
        xp = _xp;
        maxHPInc = _maxHPInc;
        mpRegenInc = _mpRegenInc;
        speedInc = _speedInc;
        attackInc = _attackInc;
        luckInc = _luckInc;
        castSpdInc = _castSpdInc;
        shotSpdInc = _shotSpdInc;
        rangeInc = _rangeInc;
        knockbackInc = _knockbackInc;
        writerDead = _writerDead;
        finishedGame = _finishedGame;
        currentStreak = _currentStreak;
        highestStreak = _highestStreak;
        totalWins = _totalWins;
        totalDeaths = _totalDeaths;
        fastestTime = _fastestTime;
        maxRepeats = _maxRepeats;
    }
}
