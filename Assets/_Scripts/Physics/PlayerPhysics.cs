using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class PlayerPhysics : MonoBehaviour
{
    public static PlayerPhysics Instance;

    private Rigidbody rb;
    [SerializeField] private AudioSource FX_Source;
    [SerializeField] private AudioClip cannonSound;
    [SerializeField] private AudioClip hookSound;
    [SerializeField] private AudioClip SawSound;
    [SerializeField] private AudioClip jumpSound;

    public bool debugActive;
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
    public bool startAcceleratingLeft, startAcceleratingRight;

    public bool stickySecondState = false;
    public Vector3 stickyDirection;
    private float nextCannonFireTime;

    [SerializeField] private float sawMass;
    private float regMass;
    

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

        regMass = rb.mass;

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


        #region SawArm
        //get direction from input vector(horizontal) + camera forward
        Vector3 accelerationDirection = RobotManager.Instance.physics.myCamera.gameObject.transform.forward;
        //apply acceleration force here
        if (startAcceleratingLeft)
        {
            rb.mass = sawMass;
            SO_PhysicsInteraction leftInteraction = PhysicsInteractionManager.instance.interactionLoadedLeft;
            ApplyForce(accelerationDirection, leftInteraction.magnitude, leftInteraction.forceMode, RobotArmPlacement.Left);
            RobotManager.Instance.robotAnimation.UpdateArmAnimationState(RobotArmPlacement.Left, (int)armAnimationStates.Saw, true);
            if (!groundCheck.isGrounded)
            {
                LoadSawArmJump();
                startAcceleratingLeft = false;
                RobotManager.Instance.robotAnimation.UpdateArmAnimationState(RobotArmPlacement.Left, (int)armAnimationStates.Idle, false);

            }
        }
        if (startAcceleratingRight)
        {
            rb.mass = sawMass;
            SO_PhysicsInteraction rightIntearction = PhysicsInteractionManager.instance.interactionLoadedRight;
            ApplyForce(accelerationDirection, rightIntearction.magnitude, rightIntearction.forceMode, RobotArmPlacement.Right);
            RobotManager.Instance.robotAnimation.UpdateArmAnimationState(RobotArmPlacement.Right, (int)armAnimationStates.Saw,true);
            if (!groundCheck.isGrounded)
            {
                LoadSawArmJump();
                startAcceleratingRight = false;
                RobotManager.Instance.robotAnimation.UpdateArmAnimationState(RobotArmPlacement.Right, (int)armAnimationStates.Idle, false );
            }
        }
        #endregion

        //Debug.Log(rb.linearVelocity);
    }

    private void LoadSawArmJump()
    {
        rb.mass = regMass;
        LoadPhysicInteraction(PhysicsInteractionManager.instance.interactionsList[4], RobotArmPlacement.Terminator);
       
    }

    public void ApplyForce(Vector3 newDirection, float newMagnitude, ForceMode newForceMode, RobotArmPlacement armPlacement)
    {

        if (debugActive) 
        {
            Debug.Log("Direction: " + newDirection + "\nMagnitude: " + newMagnitude + "\nForceMode: " + newForceMode);
        }
        Vector3 newForce = newDirection * newMagnitude;

        rb.AddForce(newForce, newForceMode);

        //RobotManager.Instance.robotAnimation.UpdateArmAnimationState(armPlacement, (int)armAnimationStates.Idle,false );
    }

    public bool TestPhysicInteraction(SO_PhysicsInteraction interaction, RobotArmPlacement armPlacement)
    {
        bool isTestSuccess = false;

        #region SawHand
        if (interaction.physicInteraction == physicInteractions.SawHand)
        {
            //Transform sawOrigin;

            ////Let's apply the placement of the arm through the robot manager.
            //switch (armPlacement)
            //{
            //    case RobotArmPlacement.Left:
            //        sawOrigin = RobotManager.Instance.armManagement.leftArm.prefab.transform;
            //        break;

            //    case RobotArmPlacement.Right:
            //        sawOrigin = RobotManager.Instance.armManagement.rightArm.prefab.transform;
            //        break;

            //    default:
            //        return;
            //}

            // Deve stare sul player: controlla il Rigidbody del player.
            //SawHandManager sawHand = GetComponent<SawHandManager>();
            //if (sawHand == null)
            //    sawHand = gameObject.AddComponent<SawHandManager>();

            //What does sawHand Configure do?
            //sawHand.Configure(sawOrigin);

            //Initialize SawHandManager
            SawHandManager sawHandManager;
            switch (armPlacement)
            {
                case RobotArmPlacement.Left:
                    sawHandManager = RobotManager.Instance.armManagement.leftArm.prefab.GetComponent<SawHandManager>();
                    break;
                case RobotArmPlacement.Right:
                    sawHandManager = RobotManager.Instance.armManagement.rightArm.prefab.GetComponent<SawHandManager>();
                    break;
                default:
                case RobotArmPlacement.Terminator:
                    sawHandManager = null;
                    break;


            }
            //let player cast raycast if they are grounded
            if (groundCheck.isGrounded) 
            {
                //Raycast out of camera , if distance is acceptable, attach.
                Vector3 rayOrigin = myCamera.transform.position;
                Vector3 rayDirection = myCamera.transform.forward;
                RaycastHit hit;


                //Fire Raycast
                if (Physics.Raycast(rayOrigin, rayDirection, out hit, 100f))
                {
                    //extract distance from the camera to the hit object
                    float rayDistance = hit.distance;
                    Debug.Log(hit.collider.name + " hit: " + rayDistance);

                    //check if raycast distance is within range of sawmanager
                    if (rayDistance <= sawHandManager.acquireDistance)
                    {
                        isTestSuccess = true;
                        FX_Source.PlayOneShot(SawSound);
                    }

                }


            }
            
        }
        #endregion

        return isTestSuccess;
    }

    //these are intearction loading for one shot physics
    public void LoadPhysicInteraction(SO_PhysicsInteraction interaction, RobotArmPlacement armPlacement)
    {
        if (interaction.physicInteraction == physicInteractions.Cannon && Time.time < nextCannonFireTime)
            return;

        if (debugActive)
        {
            Debug.Log("<color=yellow>Loading: "+interaction.name+"</color>");
        }

        //pause linear falling to override with new physic interaction
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        #region Jump
        if (interaction.physicInteraction == physicInteractions.Jump)
        {
            //apply jump force in vertical direction
            ApplyForce(interaction.distance, interaction.magnitude, interaction.forceMode, armPlacement);
            FX_Source.PlayOneShot(jumpSound);
        }
        #endregion

        #region Cannon
        if (interaction.physicInteraction == physicInteractions.Cannon)
        {
            nextCannonFireTime = Time.time + 1f;
            //apply cannon force in direction that is oposite to where camera is pointing
            ApplyForce(-1*myCamera.gameObject.transform.forward, interaction.magnitude, interaction.forceMode, armPlacement);
            FX_Source.PlayOneShot(cannonSound);
            RobotManager.Instance.robotAnimation.UpdateArmAnimationState(armPlacement, (int)armAnimationStates.Cannon, false);
            RobotManager.Instance.robotAnimation.callDelayedAnimationUpdate(armPlacement,(int)armAnimationStates.Idle, false);
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
                //first state, spawn projectile
                if (!stickySecondState)
                {
                    //first press, spawn projectile using camera data
                    stickyScr.SpawnProjectile(myCamera.gameObject.transform.forward);
                    FX_Source.PlayOneShot(hookSound);
                    RobotManager.Instance.robotAnimation.UpdateArmAnimationState(armPlacement, (int)armAnimationStates.Claw, true);

                }
                //second state, feed direction and apply force
                else if (stickySecondState)
                {
                    Debug.Log("<color=orange>Sticky Projectile distance code entered");
                    //get transform data and load physic interaction
                    //stickyDirection = stickyScr.stickyDirection;
                    //apply force that pulls character towards world point of projectile
                    ApplyForce(stickyDirection, interaction.magnitude, interaction.forceMode, armPlacement);
                    //stickyScr.startDestroyTimer();
                    RobotManager.Instance.robotAnimation.UpdateArmAnimationState(armPlacement, (int)armAnimationStates.Idle, false);
                    stickyScr.TurnGrappleOff();
                    Debug.Log(
                        $"Force:{interaction.magnitude} " +
                        $"Magnitude:{stickyDirection} " +
                        $"Mode:{interaction.forceMode}"
                    );

                    stickySecondState = false;
                }
            }
            //clear reference
            stickyScr = null;
        }
        #endregion

        #region Saw Arm Jump
        if(interaction.physicInteraction == physicInteractions.SawHandJump)
        {
            Vector3 sawJumpDirection = myCamera.transform.forward + interaction.distance;
            ApplyForce(sawJumpDirection, interaction.magnitude, interaction.forceMode, armPlacement);
            RobotManager.Instance.robotAnimation.UpdateArmAnimationState(armPlacement, (int)armAnimationStates.Idle, false);
        }
        #endregion


    }

    public void ResetFallingVelocity()
    {
        rb.linearVelocity = new Vector3 (rb.linearVelocity.x, 0, rb.linearVelocity.z);
    }

    public Rigidbody getRigidbody() { return rb; }

    public bool areWeAccelerating()
    {
        if (startAcceleratingLeft || startAcceleratingRight)
        {
            return true;
        }
        else return false;
    }
    
}
