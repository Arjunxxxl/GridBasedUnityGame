using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] private Button restartButton;

    // Start is called before the first frame update
    void Start()
    {
        restartButton.onClick.AddListener(OnClickRestartButton);
    }

    private void OnClickRestartButton()
    {
        GameManager.ReqToRestartGame?.Invoke();
    }
}
