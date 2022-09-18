using System;

[System.Serializable]
public class Save {
    public int level;
    public int xp;
    public bool[] watchedMemory;
    
    public Save(int curLevel, bool[] memories, int exp) {
        watchedMemory = memories;
        level = curLevel;
        xp = exp;
    }
}
