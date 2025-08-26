using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadSaveManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init()
    {
        Debug.Log("LoadSaveManager initialized");
    }
    public class GameStateData
    {

        public class DataTransform
        {
            //Position
            public float posX;
            public float posY;
            public float posZ;
            //Rotation
            public float rotX;
            public float rotY;
            public float rotZ;
            //Scale
            public float scaleX;
            public float scaleY;
            public float scaleZ;
        }

        public class DataEnemy
        {
            public DataTransform transform = new DataTransform();
            public int enemyID;
            public int health;
        }

        public class DataPlayer
        {
            public DataTransform transform = new DataTransform();
            public int health;
        }

        public DataPlayer player = new DataPlayer();
        public List<DataEnemy> enemies = new List<DataEnemy>();
    }
    //Game state data to be saved/loaded
    public GameStateData gameState = new GameStateData();

    public void Save(string fileName = "GameData.xml")
    {
        // Save game data
        XmlSerializer serializer = new XmlSerializer(typeof(GameStateData));  //Serialize the data to XML
        FileStream stream = new FileStream(fileName, FileMode.Create);  //Save to a file named GameData.xml
        serializer.Serialize(stream, gameState);    //Takes gameState and writes it to the file

        stream.Flush();
        stream.Close();
        stream.Dispose();
        Debug.Log("Game Saved");

    }

    // Load game data from XML file
    public void Load(string fileName = "GameData.xml")
    {
        if (!File.Exists(fileName))
        {
            //SceneManager.LoadScene("Game");
            Debug.LogWarning("No save file found at " + fileName);
            return;
        }
        //Load game data
        XmlSerializer serializer = new XmlSerializer(typeof(GameStateData)); //Deserialize the data from XML
        FileStream stream = new FileStream(fileName, FileMode.Open); //Open the file named GameData.xml
        gameState = serializer.Deserialize(stream) as GameStateData; //Read the file and store it in gameState

        

        stream.Flush();
        stream.Close();
        stream.Dispose();

    }
}
