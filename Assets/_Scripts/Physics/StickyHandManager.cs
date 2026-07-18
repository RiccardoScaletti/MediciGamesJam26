using Unity.VisualScripting;
using UnityEngine;

public class StickyHandManager : MonoBehaviour
{
    public Transform armPivot;
    public Transform worldPivot;
    [SerializeField] GameObject grappleHandObject;
    public StickyHandProjectile currentProjectile;
    public bool isProjectileAlive;
    public bool isProjectileCollided;
    public float projectileSpeed;
    public LineRenderer armRender;
    public float projectileLifetime;
    

    //get a vector and magnitude from two different points in the world. 

    //send direction and magnitude data to physic interaction method.

    //spawn projectile on first button click, turn off hand on first button click.

    //pull to object on second button click, delete projectile and turn on hand on second click.

    //stretch goal: add distance to stretch
    
    public void SpawnProjectile(Vector3 direction)
    {
        //turn off hand object


        //spawn projectile
        currentProjectile = Instantiate(
            Resources.Load<GameObject>("RobotArms/StickyHandProjectile"), //gameobject
            RobotManager.Instance.projectileSpawner.position , //transform position
            Quaternion.identity //transform rotation
            ).GetComponent<StickyHandProjectile>();

        currentProjectile.manager = this;
        currentProjectile.StartProjectilePath(direction, projectileSpeed);

    }

    private void Start()
    {
        
        ToggleHand(false);
    }

    private void Update()
    {
        armRender.SetPosition(0, armPivot.position);
        if (isProjectileAlive)
        {
            armRender.SetPosition(1, currentProjectile.transform.position);
            
        }
        else
        {
            armRender.SetPosition(1, grappleHandObject.transform.position);
        }
    }

    public void ToggleHand(bool toggle)
    {
        //planning to use isprojectile active as bool for toggle function
        grappleHandObject.SetActive(!toggle);
        //projectile is on, change line renderer to follow projectile
        if (!toggle)
        {
            armRender.SetPosition(1, grappleHandObject.transform.position);
        }
    }
}
