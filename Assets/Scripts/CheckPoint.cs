using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private bool isActive = true;
    [SerializeField] private GameObject Flag;
    private CanvasManager CanvasManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CanvasManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<CanvasManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player") && isActive)
        {
            Debug.Log("Checkpoint reached!");
            Flag.SetActive(true);
            isActive = false;
            CanvasManager.SaveGame();
        }
    }
}
