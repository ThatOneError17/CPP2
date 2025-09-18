using UnityEngine;

public class NodeBasedMovingPlatform : MonoBehaviour
{

    private Transform[] nodePointList;
    private int currentNodeIndex = 0;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float lifeTime = 5f;
    private float distanceThreshold = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (nodePointList == null || nodePointList.Length == 0) return;

        currentNodeIndex = currentNodeIndex % nodePointList.Length;
        transform.position = Vector3.MoveTowards(transform.position, nodePointList[currentNodeIndex].position, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, nodePointList[currentNodeIndex].position) < distanceThreshold) 
        {
            currentNodeIndex++; //Go to next node on list
        }
        Destroy(gameObject, lifeTime);
    }

    public void InitializeNodes(Transform[] nodes)
    {
        nodePointList = nodes;
        currentNodeIndex = 0;
    }
}
