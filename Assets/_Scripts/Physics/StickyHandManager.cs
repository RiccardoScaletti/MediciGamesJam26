using System.Collections;
using UnityEngine;

public class StickyHandManager : MonoBehaviour
{
    [Header("Vector Direction")]
    public Transform armPivot;
    public Transform worldPivot;
    public Vector3 stickyDirection;

    [Header("Projectile Health")]
    public float projectileLifetime;
    public float projectileDestroyDelay;
    public bool isProjectileAlive;
    public bool isProjectileCollided;
    public StickyHandProjectile currentProjectile;
    public float projectileSpeed;


    [Header("Visuals")]
    [SerializeField] GameObject grappleHandObject;
    public LineRenderer armRender;

    //get a vector  from two different points in the world. 

    //send direction  data to physic interaction method.

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
        isProjectileAlive = true;
        //projectile will: start state 2 for player physics and give reference to manager to track
        //StartCoroutine(nameof(ProjectileLifetime));
    }

    private void Start()
    {
        
        ToggleHand(false);
        //isProjectileAlive = false;
    }

    private void Update()
    {
        armRender.SetPosition(0, armPivot.position);
        //entering second state
        if (isProjectileAlive)
        {
            //send world data to line renderer
            armRender.SetPosition(1, currentProjectile.transform.position);
            if (RobotManager.Instance.physics.debugActive)
            {
                if (worldPivot)
                {
                    //Debug.Log("<color=orange>Distance of sticky: " + (worldPivot.transform.position - armPivot.transform.position).normalized);
                }
                
            }
        }
        else//set line renderer to default state
        {
            armRender.SetPosition(1, grappleHandObject.transform.position);
        }


        if (isProjectileAlive && isProjectileCollided) 
        {
            RobotManager.Instance.physics.stickySecondState = true;
            RobotManager.Instance.physics.stickyDirection = (worldPivot.transform.position - armPivot.transform.position).normalized;
        }
    }

    //use to toggle between visuals
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

    //public IEnumerator ProjectileLifetime()
    //{
    //    yield return new WaitForSeconds(projectileLifetime);
    //    DestroyProjectile();

    //}
    //public void startDestroyTimer()
    //{
    //    StartCoroutine(nameof(ProjectilePulled));
    //}
    //public IEnumerator ProjectilePulled()
    //{
    //    yield return new WaitForSeconds(projectileDestroyDelay);
    //    DestroyProjectile();
    //}

    //public void DestroyProjectile()
    //{

    //    isProjectileAlive = false;
    //    ToggleHand(isProjectileAlive);
    //    isProjectileCollided = false;
    //    currentProjectile = null;
    //    worldPivot = null;
    //    Destroy(currentProjectile.gameObject);
    //}
}
