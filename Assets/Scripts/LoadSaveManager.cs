using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;



public class LoadSaveManager : MonoBehaviour
{
    public static string key = "this_is_a_secret"; //Encryption key for saving/loading data
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
        MemoryStream memoryStream = new MemoryStream();  //Need to save like this for encryption later
        serializer.Serialize(memoryStream, gameState);    //Takes gameState and writes it to the file

        byte[] encryptedData = Encrypt(memoryStream.ToArray(), key);

        FileStream stream = new FileStream(fileName, FileMode.Create);
        stream.Write(encryptedData, 0, encryptedData.Length);

        stream.Flush();
        stream.Close();
        stream.Dispose();

        memoryStream.Flush();
        memoryStream.Close();
        memoryStream.Dispose();
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
        byte[] encryptedData = new byte[stream.Length]; //Create a byte array to hold the encrypted data
        stream.Read(encryptedData, 0, encryptedData.Length); //Read the file into the byte array
       

        

        stream.Flush();
        stream.Close();
        stream.Dispose();

        // Decrypt back into raw XML data
        byte[] decryptedData = Decrypt(encryptedData, key);

        // Deserialize back into GameState
        MemoryStream memoryStream = new MemoryStream(decryptedData);
        gameState = serializer.Deserialize(memoryStream) as GameStateData;

        memoryStream.Flush();
        memoryStream.Close();
        memoryStream.Dispose();

    }

    private byte[] Encrypt(byte[] data, string key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
            aes.IV = new byte[16]; // static IV for simplicity

            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
        }
    }

    private byte[] Decrypt(byte[] data, string key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
            aes.IV = new byte[16]; // must match Encrypt IV

            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
        }
    }


}
