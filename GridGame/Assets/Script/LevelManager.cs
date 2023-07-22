using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Refrence")]
    [SerializeField] private LevelData levelData;
    [SerializeField] private GridSystem gridSystem;

    [Header("Level Objects")]
    [SerializeField] private GameObject levelPlayer;
    [SerializeField] private GameObject levelGround;
    [SerializeField] private List<GameObject> players;
    [SerializeField] private List<GameObject> grounds;

    [Header("Player Color")]
    [SerializeField] private List<Color> playerColor;

    private LevelData.Data currentLevelData;

    private void OnEnable()
    {
        GameManager.SetUpLevel += SetUpLevel;
        GameManager.RestartGame += ResetAllObjects;
        GameManager.GameFinished += MoveToNextLevel;
    }

    private void OnDisable()
    {
        GameManager.SetUpLevel -= SetUpLevel;
        GameManager.RestartGame -= ResetAllObjects;
        GameManager.GameFinished -= MoveToNextLevel;
    }

    // Start is called before the first frame update
    void Start()
    {
        gridSystem = GridSystem.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetUpLevel()
    {
        int level = PlayerPrefs.GetInt("Level", 0);
        
        if(level >= levelData.data.Count)
        {
            level = 0;
        }

        currentLevelData = levelData.data[level];

        if(players == null)
        {
            players = new List<GameObject>();
        }
        else
        {
            foreach (var player in players)
            {
                Destroy(player);
            }

            players.Clear();
        }

        if (grounds == null)
        {
            grounds = new List<GameObject>();
        }
        else
        {
            foreach (var ground in grounds)
            {
                Destroy(ground);
            }

            grounds.Clear();
        }

        for (int i = 0; i < currentLevelData.levelObjectData.Count; i++)
        {
            GridNode gridNode = gridSystem.GetGridNode(currentLevelData.levelObjectData[i].gridIndex);

            if(gridNode == null)
            {
                continue;
            }

            if (currentLevelData.levelObjectData[i].isPlayer)
            {
                GameObject obj = Instantiate(levelPlayer, gridNode.pos, Quaternion.identity);
                obj.SetActive(true);
                obj.transform.SetParent(transform);

                obj.transform.localScale = Vector3.zero;
                obj.transform.DOScale(Vector3.one, 0.25f).SetDelay(0.1f * (currentLevelData.levelObjectData.Count - i) + 0.25f).SetEase(Ease.InOutQuad);

                gridNode.obj = obj;
                gridNode.isPlayer = true;
                gridNode.playerIndex = currentLevelData.levelObjectData[i].playerIndex;

                obj.GetComponent<GridObject>().SetUp(gridNode.gridIndex, playerColor[gridNode.playerIndex]);

                players.Add(obj);
            }
            else if (!currentLevelData.levelObjectData[i].isPlayer)
            {
                GameObject obj = Instantiate(levelGround, gridNode.pos, Quaternion.identity);
                obj.SetActive(true);
                obj.transform.SetParent(transform);

                obj.transform.localScale = Vector3.zero;
                obj.transform.DOScale(Vector3.one, 0.25f).SetDelay(0.1f * (currentLevelData.levelObjectData.Count-  i)).SetEase(Ease.InOutQuad);

                gridNode.obj = obj;
                gridNode.isPlayer = false;

                grounds.Add(obj);
            }
        }
    }

    private void ResetAllObjects()
    {
        if (players == null)
        {
            players = new List<GameObject>();
        }
        else
        {
            foreach (var player in players)
            {
                Destroy(player);
            }

            players.Clear();
        }

        if (grounds == null)
        {
            grounds = new List<GameObject>();
        }
        else
        {
            foreach (var ground in grounds)
            {
                ground.transform.DOScale(Vector3.zero, 0.25f).OnComplete(() => Destroy(ground));
            }

            grounds.Clear();
        }
    }

    private void MoveToNextLevel()
    {
        int level = PlayerPrefs.GetInt("Level", 0);

        level++;

        if (level >= levelData.data.Count)
        {
            level = 0;
        }

        PlayerPrefs.SetInt("Level", level);
    }
}
