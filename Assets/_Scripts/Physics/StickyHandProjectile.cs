using System.Collections;
using Unity.AI.MCP.Editor.Tools;
using UnityEngine;


public class StickyHandProjectile : MonoBehaviour
{
    //store reference to sticky manager
    //update transform to sticky manager
    // destory self if haven't collided after x seconds (stored in sticky manager)
    // destroy self when physic interaction is loaded. 
    //freeze self if collision happens
    //read velocity needed from sticky manager
    Rigidbody rb;
    [HideInInspector]public StickyHandManager manager;

    private void Awake()
    {
        if (!GetComponent<Rigidbody>())
        {
            Debug.Log("<color=red>projectile is missing rigidbody.</color>");
        }
        else
        {
            rb = GetComponent<Rigidbody>();
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == RobotManager.Instance.gameObject.layer)
        {
            return;
        }
        //rb.linearVelocity = Vector3.zero;
        //rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        manager.isProjectileCollided = true;
        Debug.Log(collision.gameObject.name + "is collided object");
        StopCoroutine(nameof(ProjectileLifetime));
    }

    public void StartProjectilePath(Vector3 direction, float magnitude)
    {
        //enter vector data here
        //enter magnitude data here
        rb.AddForce(direction * magnitude, ForceMode.Impulse);
        manager.isProjectileAlive = true;
        StartCoroutine(nameof(ProjectileLifetime));
    }

    private void Update()
    {
        if (manager)
        {
            if (manager.isProjectileCollided)
            {
                manager.worldPivot = this.transform;
            }
        }

        
        
    }

    public void DestroyProjectile()
    {
        manager.isProjectileAlive = false;
        manager.ToggleHand(manager.isProjectileAlive);
        manager.isProjectileCollided=false;
        manager.currentProjectile = null;
        manager.worldPivot=null;
        Destroy(this.gameObject);
    }

    public IEnumerator ProjectileLifetime()
    {
        yield return new WaitForSeconds(manager.projectileLifetime);
        DestroyProjectile();
    }
}
