using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfLevel : MonoBehaviour
{

    public AudioClip levelCompleteSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            AudioSource.PlayClipAtPoint(levelCompleteSound, transform.position);
            GameManager.levelFinish = true;
            StartCoroutine(TimeUntilExit());
        }
    }

    private IEnumerator TimeUntilExit()
    {
        yield return new WaitForSeconds(6f);
        SceneManager.LoadScene("MainMenu");
    }
}
