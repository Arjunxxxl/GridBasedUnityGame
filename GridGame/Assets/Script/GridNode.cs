using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridNode
{
    public Vector3 gridIndex;
    public Vector3 pos;
    public GameObject obj;
    public bool isPlayer;
    public int playerIndex;
}
