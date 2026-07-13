using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class PlayerPhysics : MonoBehaviour
{
    public static PlayerPhysics Instance;

    private Rigidbody rb;

    [SerializeField] private bool debugActive;
    private Vector3 direction;
    [SerializeField] private float fallingSpeed;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (Instance == null) 
        {
            Instance = this;
            //add do not destroy on load
        }
        else { Destroy(this.gameObject);}


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

        Vector3 fallingVelocity = new Vector3(0,rb.linearVelocity.y,0);
        if(fallingVelocity.y < -0.1f)
        {
            //Debug.Log("You are falling, "+fallingVelocity.y);
            rb.linearVelocity = new Vector3 (rb.linearVelocity.x, fallingVelocity.y*fallingSpeed, rb.linearVelocity.z);
        }

        Debug.Log(rb.linearVelocity);
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

    public void LoadPhysicInteraction(SO_PhysicsInteraction interaction)
    {
        if(interaction.physicDirectionType == physicDirectionType.defined)
        {
            ApplyForce(interaction.distance, interaction.magnitude, interaction.forceMode);
        }
        
    }
}
