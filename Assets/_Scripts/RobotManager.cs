using UnityEngine;

public class RobotManager : MonoBehaviour
{
    public static RobotManager Instance;

    public RobotArmManagement armManagement;
    public RobotMovement robotMovement;
    public RobotController robotController;
    public PlayerPhysics physics;

    public Transform projectileSpawner;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            //add don't destory on load?
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
