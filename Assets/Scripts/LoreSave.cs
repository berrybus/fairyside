using System;

[System.Serializable]
public class LoreSave {
    public bool[] foundMonsterNotes;
    public bool[] foundLoreNotes;

    public LoreSave(
        bool[] _foundMonsterNotes,
        bool[] _foundLoreNotes
    ) {
        foundMonsterNotes = _foundMonsterNotes;
        foundLoreNotes = _foundLoreNotes;
    }
}
