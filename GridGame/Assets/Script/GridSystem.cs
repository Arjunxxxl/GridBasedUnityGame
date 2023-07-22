using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject gridBg;

    [Header("Grid Data")]
    [SerializeField] private Vector3 gridCenter;
    [SerializeField] private Vector3 gridSize;
    [SerializeField] private Vector3 cellSize;
    [SerializeField] private Vector3 offset;

    [Header("Spawned Grid")]
    [SerializeField] private List<GridNode> gridNode;
    [SerializeField] private List<GameObject> gridBgObjs;

    public List<GridNode> GridNodes { get { return gridNode; } }
    public Vector3 GridSize { get { return gridSize; } }

    [Header("test")]
    [SerializeField] private bool test;

    #region SingleTon
    public static GridSystem Instance;
    private void Awake()
    {
        if(!Instance)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private void OnEnable()
    {
        GameManager.SetUpGrid += CreateGrid;
        GameManager.RestartGame += RemoveGrid;
    }

    private void OnDisable()
    {
        GameManager.SetUpGrid -= CreateGrid;
        GameManager.RestartGame -= RemoveGrid;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(test)
        {
            test = false;
            CreateGrid();
        }
    }

    #region Create / Remove Grid
    private void CreateGrid()
    {
        gridNode = new List<GridNode>();

        if(gridBgObjs != null )
        {
            foreach (var item in gridBgObjs)
            {
                Destroy(item.gameObject);
            }

            gridBgObjs.Clear();
        }
        else
        {
            gridBgObjs = new List<GameObject>();
        }

        Vector3 spawnPos = Vector3.zero;
        gridCenter = new Vector3(cellSize.x * gridSize.x, cellSize.y * gridSize.y, cellSize.z) / 2;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                spawnPos = new Vector3(cellSize.x * x + offset.x * x, cellSize.y * y + offset.y * y, cellSize.z) - gridCenter;

                GameObject obj = Instantiate(gridBg, spawnPos, Quaternion.identity);
                obj.SetActive(true);
                obj.transform.localScale = Vector3.zero;

                obj.transform.DOScale(cellSize, 0.25f).SetDelay(0.1f * (x + y)).SetEase(Ease.InOutQuad);

                obj.transform.SetParent(transform);

                gridBgObjs.Add(obj);

                GridNode gn = new GridNode();
                gn.gridIndex = new Vector3(x, y, 0);
                gn.pos = spawnPos;
                gn.obj = null;
                gn.isPlayer = false;
                gn.playerIndex = -1;

                gridNode.Add(gn);
            }
        }
    }

    private void RemoveGrid()
    {
        if (gridBgObjs != null)
        {
            foreach (var item in gridBgObjs)
            {
                item.transform.DOScale(Vector3.zero, 0.25f).OnComplete(() => Destroy(item));
            }

            gridBgObjs.Clear();
        }
        else
        {
            gridBgObjs = new List<GameObject>();
        }
    }
    #endregion

    #region Grid Functions
    public GridNode GetGridNode(Vector3 gridIndex)
    {
        for (int i = 0; i < gridNode.Count; i++)
        {
            if (gridNode[i].gridIndex == gridIndex)
            {
                return gridNode[i];
            }
        }

        return null;
    }

    public void MoveUpPlayerDown(Vector3 previousGridIndex)
    {
        Vector3 upIndex = previousGridIndex;
        upIndex.y++;

        if (upIndex.y >= gridSize.y - 1) { upIndex.y = gridSize.y - 1; }

        GridNode upNode = GetGridNode(upIndex);

        if (upNode.isPlayer)
        {
            upNode.obj.GetComponent<GridObject>().CheckForDownMovement();
        }
    }

    public void CheckIsPlayerIsAround(Vector3 gridIndex)
    {
        Vector3 leftIndex = gridIndex;
        leftIndex.x--;

        Vector3 rightIndex = gridIndex;
        rightIndex.x++;

        Vector3 downIndex = gridIndex;
        downIndex.y--;

        if (leftIndex.x <= 0) { leftIndex.x = 0; }
        if (rightIndex.x >= gridSize.x - 1) { rightIndex.x = gridSize.x - 1; }
        if (downIndex.x <= 0) { downIndex.x = 0; }

        GridNode leftNode = GetGridNode(leftIndex);
        GridNode rightNode = GetGridNode(rightIndex);
        GridNode downNode = GetGridNode(downIndex);
        GridNode currentNode = GetGridNode(gridIndex);

        if(currentNode.isPlayer)
        {
            if (downNode.isPlayer && currentNode.playerIndex == downNode.playerIndex && downIndex != gridIndex)
            {
                currentNode.obj.GetComponent<GridObject>().Remove();
                downNode.obj.GetComponent<GridObject>().Remove();

                CheckIfAllPlayersObjectsRemoved();

                return;
            }

            if (leftNode.isPlayer && rightNode.isPlayer && currentNode.playerIndex == leftNode.playerIndex && currentNode.playerIndex == rightNode.playerIndex && leftIndex != gridIndex && rightIndex != gridIndex)
            {
                currentNode.obj.GetComponent<GridObject>().Remove();
                leftNode.obj.GetComponent<GridObject>().Remove();
                rightNode.obj.GetComponent<GridObject>().Remove();
            }
            else
            {
                if (leftNode.isPlayer && currentNode.playerIndex == leftNode.playerIndex && leftIndex != gridIndex)
                {
                    currentNode.obj.GetComponent<GridObject>().Remove();
                    leftNode.obj.GetComponent<GridObject>().Remove();
                }

                if (rightNode.isPlayer && currentNode.playerIndex == rightNode.playerIndex && rightIndex != gridIndex)
                {
                    currentNode.obj.GetComponent<GridObject>().Remove();
                    rightNode.obj.GetComponent<GridObject>().Remove();
                }
            }

            CheckIfAllPlayersObjectsRemoved();
        }
    }
    #endregion

    #region Chekcing Near by Nodes
    private void CheckIfAllPlayersObjectsRemoved()
    {
        bool isRemove = true;

        for (int i = 0; i < gridNode.Count; i++)
        {
            if (gridNode[i].isPlayer)
            {
                isRemove = false;
            }
        }

        if(isRemove)
        {
            StartCoroutine(RestartGameWithDelay());
        }
    }
    #endregion

    #region Game over
    IEnumerator RestartGameWithDelay()
    {
        GameManager.GameFinished?.Invoke();

        yield return new WaitForSeconds(0.5f);
        GameManager.ReqToRestartGame?.Invoke();
    }
    #endregion
}
