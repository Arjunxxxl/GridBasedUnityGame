using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    [Header("Raycast data")]
    [SerializeField] private float rayDistance;
    [SerializeField] private LayerMask layerMaskObject;

    [Header("Object Selected Data")]
    [SerializeField] private GameObject selectedObject;

    [Header("Mouse Data")]
    [SerializeField] private Vector2 startPos;
    [SerializeField] private Vector2 endPos;
    [SerializeField] private Vector2 dir;
    [SerializeField] private float dist;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float minDistanceToChangeIndex;

    [SerializeField] private GridSystem gridSystem;

    private bool isGameStarted;

    private void OnEnable()
    {
        GameManager.StartGame += OnStartGame;
    }

    private void OnDisable()
    {
        GameManager.StartGame += OnStartGame;
    }

    // Start is called before the first frame update
    void Start()
    {
        gridSystem = GridSystem.Instance;    
    }

    // Update is called once per frame
    void Update()
    {
        TakeMouseInput();
    }

    private void OnStartGame(bool isGameStarted)
    {
        this.isGameStarted = isGameStarted;
    }

    private void TakeMouseInput()
    {
        if(!isGameStarted)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, rayDistance);

            if (hit.collider != null)
            {
                selectedObject = hit.collider.gameObject;
                selectedObject.GetComponent<GridObject>().AnimateUserMouseDown();
            }

            startPos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0) && selectedObject != null)
        {
            endPos = Input.mousePosition;

            dir = endPos - startPos;

            dist = dir.magnitude * mouseSensitivity;

            dir.Normalize();

            float dotWithRight = Vector2.Dot(dir, Vector2.right);
            float dorWithTop = Mathf.Abs(Vector2.Dot(dir, Vector2.up));

            if (dist > minDistanceToChangeIndex)
            {
                if (dotWithRight != 0 && dorWithTop < 0.25f)
                {
                    GridObject grideObject = selectedObject.GetComponent<GridObject>();
                    Vector3 gridIndex = grideObject.GetGridIndex();

                    if (!grideObject.CanMove)
                    {
                        return;
                    }

                    GridNode currentNode = gridSystem.GetGridNode(gridIndex);

                    if (currentNode.isPlayer)
                    {
                        if (dotWithRight < 0)
                        {
                            Debug.Log("Left Swipe End");

                            gridIndex.x--;

                            if (gridIndex.x <= 0)
                            {
                                gridIndex.x = 0;
                            }
                        }
                        else
                        {
                            Debug.Log("Right Swipe End");

                            gridIndex.x++;

                            if (gridIndex.x >= gridSystem.GridSize.x - 1)
                            {
                                gridIndex.x = gridSystem.GridSize.x - 1;
                            }
                        }
                    }

                    GridNode nextNode = gridSystem.GetGridNode(gridIndex);

                    MoveObject(currentNode, nextNode, grideObject);
                }
                else
                {
                    selectedObject.GetComponent<GridObject>().AnimateUserMouseUp();
                }
            }
            else
            {
                selectedObject.GetComponent<GridObject>().AnimateUserMouseUp();
            }

            selectedObject = null;
        }
    }

    private void MoveObject(GridNode currentNode, GridNode nextNode, GridObject grideObject)
    {
        bool isNextNodeEmpty = nextNode.obj == null;

        if (nextNode != currentNode && isNextNodeEmpty)
        {
            nextNode.isPlayer = true;
            nextNode.obj = currentNode.obj;
            nextNode.playerIndex = currentNode.playerIndex;

            currentNode.isPlayer = false;
            currentNode.obj = null;
            currentNode.playerIndex = -1;

            grideObject.Move(nextNode.pos, nextNode.gridIndex);
        }
    }
}
