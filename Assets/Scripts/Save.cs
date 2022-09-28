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
    public bool[] watchedMemory;
    public bool[] foundMonsterNotes;
    public bool[] foundLoreNotes;

    public Save(
        int _level,
        bool[] _memories,
        bool[] _foundMonsterNotes,
        bool[] _foundLoreNotes,
        int _xp,
        int _maxHPInc,
        int _mpRegenInc,
        int _speedInc,
        int _attackInc,
        int _luckInc,
        int _castSpdInc,
        int _shotSpdInc,
        int _rangeInc,
        int _knockbackInc
    ) {
        watchedMemory = _memories;
        level = _level;
        xp = _xp;
        foundMonsterNotes = _foundMonsterNotes;
        foundLoreNotes = _foundLoreNotes;
        maxHPInc = _maxHPInc;
        mpRegenInc = _mpRegenInc;
        speedInc = _speedInc;
        attackInc = _attackInc;
        luckInc = _luckInc;
        castSpdInc = _castSpdInc;
        shotSpdInc = _shotSpdInc;
        rangeInc = _rangeInc;
        knockbackInc = _knockbackInc;

    }
}
