using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GridObject : MonoBehaviour
{
    [SerializeField] private Vector3 gridIndex;
    [SerializeField] private Vector3 previousGridIndex;
    [SerializeField] private bool canMove;

    public bool CanMove { get { return canMove; } }

    public void SetUp(Vector3 gridIndex, Color playerColor)
    {
        canMove = true;
        this.gridIndex = gridIndex;
        this.previousGridIndex = gridIndex;

        GetComponent<SpriteRenderer>().color = playerColor;
    }

    public Vector3 GetGridIndex()
    {
        return gridIndex;
    }

    public void Move(Vector3 pos, Vector3 gridIndex)
    {
        canMove = false;
        this.previousGridIndex = this.gridIndex;
        this.gridIndex = gridIndex;

        AnimateMove(pos, 0.25f, MoveEnded);
        AnimateScale();
    }

    private void MoveEnded()
    {
        canMove = true;
        if(!CheckForDownMovement())
        {
            GridSystem.Instance.CheckIsPlayerIsAround(gridIndex);
        }

        GridSystem.Instance.MoveUpPlayerDown(previousGridIndex);
    }

    public bool CheckForDownMovement()
    {
        Vector3 downGridIndex = gridIndex;

        downGridIndex.y--;

        if (downGridIndex.y <= 0)
        {
            downGridIndex.y = 0;
            return false;
        }

        GridNode currentNode = GridSystem.Instance.GetGridNode(gridIndex);
        GridNode nextNode = GridSystem.Instance.GetGridNode(downGridIndex);
        bool isNextNodeEmpty = nextNode.obj == null;

        if (nextNode != currentNode && isNextNodeEmpty)
        {
            nextNode.isPlayer = true;
            nextNode.obj = currentNode.obj;
            nextNode.playerIndex = currentNode.playerIndex;

            currentNode.isPlayer = false;
            currentNode.obj = null;
            currentNode.playerIndex = -1;

            Move(nextNode.pos, nextNode.gridIndex);
            return true;
        }

        return false;
    }

    public void Remove()
    {
        GridSystem.Instance.GetGridNode(gridIndex).isPlayer = false;
        GridSystem.Instance.GetGridNode(gridIndex).obj = null;

        transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InOutQuad).OnComplete(() => { gameObject.SetActive(false); });
    }

    #region Animations
    private void AnimateMove(Vector3 enPos, float duration, Action callback)
    {
        transform.DOMove(enPos, duration, false).SetEase(Ease.OutBack).OnComplete(() => callback?.Invoke());
    }

    private void AnimateScale()
    {
        transform.DOScale(Vector3.one * 0.75f, 0.125f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            transform.DOScale(Vector3.one, 0.125f).SetEase(Ease.InOutQuad);
        });
    }

    public void AnimateUserMouseDown()
    {
        transform.DOScale(Vector3.one * 0.85f, 0.125f).SetEase(Ease.InOutQuad);
    }

    public void AnimateUserMouseUp()
    {
        transform.DOScale(Vector3.one * 0.75f, 0.125f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            transform.DOScale(Vector3.one, 0.125f).SetEase(Ease.InOutQuad);
        });
    }
    #endregion
}
