using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [SerializeField] private GameObject player;
    [SerializeField] private Transform spawnPoint;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    
    void Start()
    {
        player.transform.position = spawnPoint.position;
    }

    void Update()
    {
        
    }

    public void GameOver()
    {
        Debug.Log("Game Over"); 
        player.transform.position = spawnPoint.position;
    }
}
