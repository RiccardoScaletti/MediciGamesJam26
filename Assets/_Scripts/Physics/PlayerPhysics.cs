using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class PlayerPhysics : MonoBehaviour
{
    public static PlayerPhysics Instance;

    private Rigidbody rb;

    [SerializeField] private bool debugActive;
    private Vector3 direction;
    //increase falling speed when you are falling
    [SerializeField] private float fallingSpeedGrowth;
    //limit how fast you can fall
    [SerializeField] private float fallingTerminalVelocity;
    //limit how many times you are able to jump
    public GroundChecker groundCheck;

    public Camera myCamera;

    //move this somewhere better.
    //have parameter that gives slight wiggle room to input jump when moving off a platform
    public float coyoteTimer;


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

        Instantiate(Resources.Load<GameObject>("CanvasHUD"));

        groundCheck.InitializeScript(this);
    }

    private void FixedUpdate()
    {
        //read inputs or update physics here

        //speed up linear velocity in y if linear velocity in y is negative.
        Vector3 fallingVelocity = new Vector3(0,rb.linearVelocity.y,0);
        if(fallingVelocity.y < -0.1f)
        {
            groundCheck.isJumping = false;
            //Debug.Log("You are falling, "+fallingVelocity.y);

            //if player is falling and slower than terminal velocity, increase falling speed.
            if (fallingVelocity.y < fallingTerminalVelocity && groundCheck.isJumping)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, fallingVelocity.y * fallingSpeedGrowth, rb.linearVelocity.z);
            }else
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, fallingTerminalVelocity, rb.linearVelocity.z);
            }
        }

        //Debug.Log(rb.linearVelocity);
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

    public void LoadPhysicInteraction(SO_PhysicsInteraction interaction, RobotArmPlacement armPlacement)
    {
        //pause linear falling to override with new physic interaction
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        #region Jump
        if (interaction.physicDirectionType == physicDirectionType.defined)
        {
            //apply jump force in vertical direction
            ApplyForce(interaction.distance, interaction.magnitude, interaction.forceMode);
        }
        #endregion

        #region Cannon
        if (interaction.physicInteraction == physicInteractions.Cannon)
        {
            //apply cannon force in direction that is oposite to where camera is pointing
            ApplyForce(-1*myCamera.gameObject.transform.forward, interaction.magnitude, interaction.forceMode);
        }
        #endregion

        #region Sticky Hand
        //sticky hand
        if (interaction  == PhysicsInteractionManager.instance.interactionsList[(int)physicInteractions.StickyHand])   //conditional operation if using sticky hand. First press vs second press distinction
        {
            //if arm has prefab, read to see if its sticky
            //need to know which arm has the sticky hand
            //store the reference of the sticky manager to this script
            StickyHandManager stickyScr;
            
            switch (armPlacement)
            {
                case RobotArmPlacement.Left:
                    stickyScr = RobotManager.Instance.armManagement.leftArm.prefab.GetComponent<StickyHandManager>();
                    break;
                case RobotArmPlacement.Right:
                    stickyScr = RobotManager.Instance.armManagement.rightArm.prefab.GetComponent<StickyHandManager>();
                    break;
                default:
                    return;
            }

            //handle states of the sticky hand once script is initialized
            if (stickyScr)
            {
                
                if (!stickyScr.isProjectileAlive)
                {
                    //first press, spawn projectile using camera data
                    stickyScr.SpawnProjectile(myCamera.gameObject.transform.forward);
                }
                else
                {
                    //get transform data and load physic interaction
                    Vector3 stickyDirection = (stickyScr.worldPivot.transform.position - stickyScr.armPivot.transform.position).normalized;
                    //apply force that pulls character towards world point of projectile
                    ApplyForce(stickyDirection, interaction.magnitude, interaction.forceMode);
                    stickyScr.currentProjectile.DestroyProjectile();
                }
            }

        }
        #endregion

        #region SawHand
        if (interaction.physicInteraction == physicInteractions.SawHand)
        {
            Transform sawOrigin;

            switch (armPlacement)
            {
                case RobotArmPlacement.Left:
                    sawOrigin = RobotManager.Instance.armManagement.leftArm.prefab.transform;
                    break;

                case RobotArmPlacement.Right:
                    sawOrigin = RobotManager.Instance.armManagement.rightArm.prefab.transform;
                    break;

                default:
                    return;
            }

            // Deve stare sul player: controlla il Rigidbody del player.
            SawHandManager sawHand = GetComponent<SawHandManager>();
            if (sawHand == null)
                sawHand = gameObject.AddComponent<SawHandManager>();

            sawHand.Configure(sawOrigin);
        }
        #endregion

    }

    public void ResetFallingVelocity()
    {
        rb.linearVelocity = new Vector3 (rb.linearVelocity.x, 0, rb.linearVelocity.z);
    }
}
