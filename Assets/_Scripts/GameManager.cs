using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [SerializeField] private GameObject player;
    [SerializeField] private Transform spawnPoint;

    private LevelRotation LevelRotationScr;

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
        LevelRotationScr = GetComponent<LevelRotation>();
    }


    public void GameOver()
    {
        Debug.Log("You Made it!"); 
        //UI Game won
        player.transform.position = spawnPoint.position;
        LevelRotationScr.RotateLevel();
    }

    public void Death()
    {
        //UI Game over
        player.transform.position = spawnPoint.position;
    }
}
