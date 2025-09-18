using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using System.Collections;
using UnityEngine.Audio;

public class CanvasManager : MonoBehaviour
{
    public AudioMixer mixer;

    [Header("Buttons")]
    public Button quitBtn;
    public Button playBtn;
    public Button loadBtn;

    [Header("Canvases")]
    public GameObject endLevelCanvas;
    public GameObject gameOverCanvas;
    public GameObject pauseCanvas;
    public GameObject audioCanvas;
    public GameObject levelFinishCanvas;

    [Header("Sliders")]
    public Slider musicVolSlider;
    public Slider sfxVolSlider;

    [Header("Text")]
    public TMP_Text musicVolSliderText;
    public TMP_Text sfxVolSliderText;

    TestController cRef;
    Skeleton[] sRef;
    KnightNPC[] kRef;

    void Awake()
    {
        // Reset state when entering the scene
        GameManager.endOfLevel = false;
        GameManager.gameOver = false;
        GameManager.isPaused = false;
        GameManager.playerDead = false;
        GameManager.levelFinish = false;
    }


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
        string startSceneName = SceneManager.GetActiveScene().name;
        if (cRef == null && startSceneName == "Game")
        {
            Debug.LogError("Player with TestController script not found!");
        }

        if (musicVolSlider)
        {
            SetupSliderInformation(musicVolSlider, musicVolSliderText, "MusicVol");
            OnSliderValueChanged(musicVolSlider.value, musicVolSlider, musicVolSliderText, "MusicVol"); // Initialize the text with the current value
        }
        if (sfxVolSlider)
        {
            SetupSliderInformation(sfxVolSlider, sfxVolSliderText, "SFXVol");
            OnSliderValueChanged(sfxVolSlider.value, sfxVolSlider, sfxVolSliderText, "SFXVol"); // Initialize the text with the current value
        }



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
            ShowAudioCanvas();
        }

        if (currentSceneName == "Game" && !GameManager.isPaused && !GameManager.endOfLevel && !GameManager.gameOver && !GameManager.levelFinish)
        {
            endLevelCanvas.SetActive(false);
            pauseCanvas.SetActive(false);
            gameOverCanvas.SetActive(false);
            audioCanvas.SetActive(false);
            levelFinishCanvas.SetActive(false);
        }

        if (GameManager.levelFinish)
        {
            ShowLevelFinishCanvas();
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
        audioCanvas.SetActive(false);

        GameManager.endOfLevel = false; //Reset the end of level state
        GameManager.gameOver = false; //Reset the game over state
        GameManager.isPaused = false; //Reset the pause state
        GameManager.levelFinish = false;
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

    private void ShowAudioCanvas()
    {
        audioCanvas.SetActive(true);
    }

    private void ShowLevelFinishCanvas()
    {
        levelFinishCanvas.SetActive(true);
    }

    private void disableAllCanvas()
    {
        endLevelCanvas.SetActive(false);
        gameOverCanvas.SetActive(false);
        pauseCanvas.SetActive(false);
        audioCanvas.SetActive(false);
        levelFinishCanvas.SetActive(false);
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

    private void SetupSliderInformation(Slider slider, TMP_Text sliderText, string mixerParameterName)
    {
        slider.onValueChanged.AddListener((value) => OnSliderValueChanged(value, slider, sliderText, mixerParameterName));

    }

    private void OnSliderValueChanged(float value, Slider slider, TMP_Text sliderText, string mixerParameterName)
    {

        if (value == 0)
        {
            value = -80; //Minimum decibel value to represent silence
        }

        else
        {
            value = Mathf.Log10(slider.value) * 20; //Convert to decibels
        }

        sliderText.text = (value == -80) ? "0%" : $"{(int)(slider.value * 100)}%";
        mixer.SetFloat(mixerParameterName, value);
    }
}
