using System.Collections;
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
    public StickyHandManager manager;


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
        //Debug.Log("<color=green>Projectile from Sticky Hand Spawned</color>");
        
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
        
        Debug.Log(collision.gameObject.name + "is collided object");
        

        if (manager)
        {
            manager.isProjectileCollided = true;
            //manager.StopProjectileLifetime();
        }
    }

    public void StartProjectilePath(Vector3 direction, float magnitude)
    {
        //enter vector data here
        //enter magnitude data here
        rb.AddForce(direction * magnitude, ForceMode.Impulse);

        manager.currentProjectile = this;
        
    }

    private void Update()
    {
        if (manager)
        {
            if (manager.isProjectileCollided)
            {
                manager.worldPivot = this.transform;//feed world data back into manager
            }
        }

        if(manager == null)
        {
            Destroy(this.gameObject);
        }
        
    }



 
}
