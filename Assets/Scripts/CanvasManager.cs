using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button quitBtn;

    [Header("Canvases")]
    public GameObject endLevelCanvas;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (quitBtn) quitBtn.onClick.AddListener(QuitGame);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.endOfLevel)
        {
            ShowEndCanvas();
        }
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ShowEndCanvas()
    {
        endLevelCanvas.SetActive(true);
    }
}
