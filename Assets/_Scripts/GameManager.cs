using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [SerializeField] private GameObject player;
    [SerializeField] private Transform spawnPoint;

    private LevelRotation levelRotationScr;
    //private GameUIManager gameUIManagerScr;
    private GameUIController gameUIControllerScr;
    private RobotController robotControllerScr;

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
        robotControllerScr = player.GetComponent<RobotController>();
        levelRotationScr = GetComponent<LevelRotation>();
        gameUIControllerScr = GetComponent<GameUIController>();
        GameEvents.RaiseLevelCompleted();

        //take away inputs till choices are made.
        robotControllerScr.enabled = false;

        player.transform.position = spawnPoint.position;
        //gameUIManagerScr.ShowChoiceMenu();
        gameUIControllerScr.ShowChoiceMenu();
    }


    public void DisableControls()
    {
        robotControllerScr.enabled = false;
    }

    public void EnableControls()
    {
        robotControllerScr.enabled = true;
    }

    public void GameOver()
    {
        Debug.Log("You Made it!"); 
        //UI Game won
        player.transform.position = spawnPoint.position;
        levelRotationScr.RotateLevel();
        gameUIControllerScr.ShowChoiceMenu();
        GameEvents.RaiseLevelCompleted();
    }

    public void Death()
    {
        //UI Game over
        player.transform.position = spawnPoint.position;
    }

    
}
