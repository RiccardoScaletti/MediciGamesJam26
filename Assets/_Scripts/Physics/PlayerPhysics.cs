using System.Xml.Schema;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class PlayerPhysics : MonoBehaviour
{

    private Rigidbody rb;

    [SerializeField] private bool debugActive;
    private Vector3 direction;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        //Create a debug menu for physics debugging if debugActive bool is active
        if (debugActive)
        {
            GameObject prefab = Instantiate(Resources.Load<GameObject>("CanvasPhysicsDebugger")) ;
            if (prefab != null) 
            {
                CanvasPhysicsDebugger debugScr = prefab.GetComponent<CanvasPhysicsDebugger>();
                Debug.Log(debugScr);
                debugScr.StorePlayerPhysicScript(this);
            }
        }
    }

    private void FixedUpdate()
    {
        //read inputs or update physics here

        //speed up linear velocity in y if linear velocity in y is negative.
    }

    public void ApplyForce(Vector3 newDirection, float newMagnitude, ForceMode newForceMode)
    {

        if (debugActive) 
        {
            Debug.Log("Direction: " + newDirection + "\nMagnitude: " + newMagnitude + "\nForceMode: " + newForceMode);
        }
        Vector3 newForce = newDirection * newMagnitude;

        rb.AddForce(newForce, newForceMode);
    }
}
