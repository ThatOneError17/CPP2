using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool playerDead = false; //Will change if the player is dead
    public static bool endOfLevel = false;
    public static bool gameOver = false; //Will change if the game is over
    public static bool hasKey = false; //IF player has a key
    public static bool isPaused = false; //Is the game paused
    public static bool levelFinish = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static GameManager Instance
    {
        get
        {
            if (!instance)
                instance = new GameObject("GameManager").AddComponent<GameManager>();
            return instance;
        }
    }

    public static LoadSaveManager StateManager
    {
        get
        {
            if (!statemanager)
                statemanager = instance.GetComponent<LoadSaveManager>();
            if (!statemanager)
                statemanager = instance.gameObject.AddComponent<LoadSaveManager>();

            return statemanager;
        }
    }

    // Internal reference to single active instance of object - for singleton behaviour
    private static GameManager instance = null;

    // Internal reference to Saveload Game Manager
    private static LoadSaveManager statemanager = null;

    // Should load from save game state on level load, or just restart level from defaults
    private static bool bShouldLoad = false;

    void Awake()
    {
        //Check if there is an existing instance of this object
        if ((instance) && (instance.GetInstanceID() != GetInstanceID()))
            Destroy(gameObject); //Delete duplicate
        else
        {
            instance = this; //Make this object the only instance
            DontDestroyOnLoad(gameObject); //Set as do not destroy
        }
    }


    void Start()
    {

        // Initialize the LoadSaveManager
        StateManager.Init();

    }

    // Update is called once per frame
    void Update()
    {
        if (hasKey)
        {
            Debug.Log("You have a key");
        }

        if (isPaused)
        {             
            Time.timeScale = 0f; // Pause the game by setting time scale to 0
        }
        else
        {
            Time.timeScale = 1f; // Resume the game by setting time scale to 1
        }

    }

    public void SaveGame()
    {
        // Print the path where the XML is save
        Debug.Log(Application.persistentDataPath);

        // Call save game functionality
        StateManager.Save(Application.persistentDataPath + "/GameData.xml");

    }

    // Load Game
    public void LoadGame()
    {
        //Call load game functionality
        StateManager.Load(Application.persistentDataPath + "/GameData.xml");
    }


}
