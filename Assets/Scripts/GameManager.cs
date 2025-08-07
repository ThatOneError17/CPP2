using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool endOfLevel = false;
    public static bool gameOver = false; //Will change if the game is over
    public static bool hasKey = false; //Number of keys the player has collected
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (hasKey == true)
        {
            Debug.Log("You have a key");
        }
    }
}
