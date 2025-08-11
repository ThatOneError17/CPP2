using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button quitBtn;
    public Button playBtn;

    [Header("Canvases")]
    public GameObject endLevelCanvas;
    public GameObject gameOverCanvas;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        if (quitBtn) 
            quitBtn.onClick.AddListener(QuitGame);
        if (playBtn) 
            playBtn.onClick.AddListener(startGame);

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.endOfLevel)
        {
            ShowEndCanvas();
        }

        if (GameManager.gameOver)
        {
            ShowGameOverCanvas();
            ShowEndCanvas();
        }
    }

    private void QuitGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "MainMenu")
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        else
        {
            //If we are in the game, we exit to the main menu
            SceneManager.LoadScene("MainMenu");
        }


    }

    private void startGame()
    {
        SceneManager.LoadScene("Game");
        GameManager.playerDead = false; //Reset the player dead state
        endLevelCanvas.SetActive(false);
        gameOverCanvas.SetActive(false);
        GameManager.endOfLevel = false; //Reset the end of level state
        GameManager.gameOver = false; //Reset the game over state
    }

    private void ShowEndCanvas()
    {
        endLevelCanvas.SetActive(true);
    }

    private void ShowGameOverCanvas()
    {
        gameOverCanvas.SetActive(true);
    }
}
