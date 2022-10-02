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
        bool _finishedGame
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
    }
}
