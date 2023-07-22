using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool isGameStarted;

    public static Action SetUpGrid;
    public static Action SetUpLevel;
    public static Action<bool> StartGame;
    public static Action GameFinished;
    public static Action ReqToRestartGame;
    public static Action RestartGame;

    private void OnEnable()
    {
        ReqToRestartGame += TryToRestartGame;
    }

    private void OnDisable()
    {
        ReqToRestartGame -= TryToRestartGame;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GameSetUpCor());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GameSetUpCor()
    {
        isGameStarted = false;

        yield return new WaitForSeconds(0.25f);

        GameSetUp();
    }

    private void GameSetUp()
    {
        isGameStarted = false;

        StartGame?.Invoke(false);
        SetUpGrid?.Invoke();

        StartCoroutine(SpawnLevel());
        StartCoroutine(StartTheGame());
    }

    IEnumerator SpawnLevel()
    {
        yield return new WaitForSeconds(1f);
        SetUpLevel?.Invoke();
    }

    IEnumerator StartTheGame()
    {
        yield return new WaitForSeconds(2.5f);
        StartGame?.Invoke(true);
        isGameStarted = true;
    }

    private void TryToRestartGame()
    {
        Debug.Log(isGameStarted);
        if (isGameStarted)
        {
            RestartGame?.Invoke();
            StartCoroutine(GameSetUpCor());
        }
    }
}
