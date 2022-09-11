using System;

[System.Serializable]
public class Save {
    public int level;
    public bool[] watchedMemory;
    
    public Save(int curLevel, bool[] memories) {
        watchedMemory = memories;
        level = curLevel;
    }
}
