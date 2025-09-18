using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private bool isActive = true;
    [SerializeField] private GameObject Flag;
    private CanvasManager CanvasManager;

    public AudioClip pop;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CanvasManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<CanvasManager>();

        Debug.Log($"{gameObject.name} Checkpoint starting. Flag reference = {Flag}");

        if (Flag == null)
            Debug.LogError($"{gameObject.name} has no Flag assigned in Inspector!");
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
            if (Flag != null)
                Flag.SetActive(true);
            AudioSource.PlayClipAtPoint(pop, transform.position);
            isActive = false;
            CanvasManager.SaveGame();
        }
    }
}
