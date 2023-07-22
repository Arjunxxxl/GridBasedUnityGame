using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelData;

[CreateAssetMenu(fileName = "Level Data", menuName = "Data/Level Data")]
[System.Serializable]
public class LevelData : ScriptableObject
{
    [System.Serializable]
    public class LevelObjectData
    {
        public Vector3 gridIndex;
        public bool isPlayer;
        public int playerIndex;
    }

    [System.Serializable]
    public class Data
    {
        public List<LevelObjectData> levelObjectData;
    }

    public List<Data> data;
}
