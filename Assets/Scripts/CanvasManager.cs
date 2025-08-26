using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.Rendering;

public class CanvasManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button quitBtn;
    public Button playBtn;
    public Button loadBtn;

    [Header("Canvases")]
    public GameObject endLevelCanvas;
    public GameObject gameOverCanvas;
    public GameObject pauseCanvas;

    TestController cRef;
    Skeleton[] sRef;
    KnightNPC[] kRef;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        if (quitBtn) 
            quitBtn.onClick.AddListener(QuitGame);
        if (playBtn) 
            playBtn.onClick.AddListener(startGame);
        if (loadBtn)
            loadBtn.onClick.AddListener(loadGameButton);

        cRef = GameObject.FindGameObjectWithTag("Player").GetComponent<TestController>();



    }

    // Update is called once per frame
    void Update()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (GameManager.endOfLevel)
        {
            ShowEndCanvas();
        }

        if (GameManager.gameOver)
        {
            ShowGameOverCanvas();
            ShowEndCanvas();
        }

        if (GameManager.isPaused)
        {
            ShowEndCanvas();
            ShowPauseCanvas();
        }

        if (currentSceneName == "Game" && !GameManager.isPaused && !GameManager.endOfLevel && !GameManager.gameOver)
        {
            endLevelCanvas.SetActive(false);
            pauseCanvas.SetActive(false);
            gameOverCanvas.SetActive(false);
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
        pauseCanvas.SetActive(false);

        GameManager.endOfLevel = false; //Reset the end of level state
        GameManager.gameOver = false; //Reset the game over state
        GameManager.isPaused = false; //Reset the pause state
    }

    private void loadGameButton()
    {
        LoadGame();
    }

    private void ShowEndCanvas()
    {
        endLevelCanvas.SetActive(true);
        pauseCanvas.SetActive(false);
    }

    private void ShowGameOverCanvas()
    {
        gameOverCanvas.SetActive(true);
        pauseCanvas.SetActive(false);
    }

    private void ShowPauseCanvas()
    {
        pauseCanvas.SetActive(true);
    }

    public void SaveGame()
    {
        cRef.SaveGamePrepare();
        // Save ALL Skeletons
        Skeleton[] skeletons = Object.FindObjectsByType<Skeleton>(FindObjectsSortMode.None);
        foreach (var s in skeletons)
        {
            s.SaveGamePrepare();
        }

        // Save ALL Knights
        KnightNPC[] knights = Object.FindObjectsByType<KnightNPC>(FindObjectsSortMode.None);
        foreach (var k in knights)
        {
            k.SaveGamePrepare();
        }

        GameManager.Instance.SaveGame();
    }    

    public void LoadGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName != "Game")
        {
            SceneManager.LoadScene("Game");
        }
        GameManager.Instance.LoadGame();

        cRef.LoadGameComplete();
        //Load ALL Skeletons
        Skeleton[] skeletons = Object.FindObjectsByType<Skeleton>(FindObjectsSortMode.None);
        foreach (var s in skeletons)
        {
            s.LoadGameComplete();
        }

        //Load ALL Knights
        KnightNPC[] knights = Object.FindObjectsByType<KnightNPC>(FindObjectsSortMode.None);
        foreach (var k in knights)
        {
            k.LoadGameComplete();
        }
    }
}
