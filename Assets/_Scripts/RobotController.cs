using UnityEngine;
using UnityEngine.InputSystem;

public class RobotController : MonoBehaviour
{
    private RobotControls controls;

    public Vector2 MoveInput { get; private set; }
    public Vector2 RotateInput { get; private set; }
    public bool rightArmHeld, leftArmHeld;

    private void Awake()
    {
        controls = new RobotControls();
    }

    private void OnEnable()
    {
        controls.RobotActions.Enable();

        controls.RobotActions.Move.performed += OnMove;
        controls.RobotActions.Move.canceled += OnMove;

        controls.RobotActions.Rotate.performed += OnRotate;
        controls.RobotActions.Rotate.canceled += OnRotate;

        controls.RobotActions.Jump.performed += OnJump;

        controls.RobotActions.RightArm.started += RightArm_started;
        controls.RobotActions.RightArm.performed += RightArm_performed;
        controls.RobotActions.RightArm.canceled += RightArm_canceled;

        controls.RobotActions.LeftArm.started += LeftArm_started;
        controls.RobotActions.LeftArm.performed += LeftArm_performed;
        controls.RobotActions.LeftArm.canceled += LeftArm_canceled;
    }

    

    private void LeftArm_started(InputAction.CallbackContext obj)
    {
        //read from a player definition what arm is equiped and send that info
        SO_PhysicsInteraction newInteraction = RobotManager.Instance.armManagement.leftArm.armData;
        if (newInteraction.armInputMode == armInputMode.Hold)
        {
            leftArmHeld = true;
        }
    }
    private void LeftArm_performed(InputAction.CallbackContext obj)
    {
        //read from a player definition what arm is equiped and send that info
        SO_PhysicsInteraction newInteraction = RobotManager.Instance.armManagement.leftArm.armData;
        PhysicsInteractionManager.instance.StoreInteractionReference(RobotArmPlacement.Left, newInteraction);

        //send that info if the physic intearction is on a press
        if (newInteraction.armInputMode == armInputMode.Press)
        {
            PlayerPhysics.Instance.LoadPhysicInteraction(newInteraction, RobotArmPlacement.Left);
        }
        //test the conditional for a physic interaction if it is on a hold
        else if (newInteraction.armInputMode == armInputMode.Hold)
        {
            if(PlayerPhysics.Instance.TestPhysicInteraction(newInteraction, RobotArmPlacement.Left))
            {
                PlayerPhysics.Instance.startAcceleratingLeft = true;
            }
        }
        else Debug.Log("<color=red>Invalid arm input mode found</color>");
    }
    private void LeftArm_canceled(InputAction.CallbackContext obj)
    {
        SO_PhysicsInteraction newInteraction = RobotManager.Instance.armManagement.leftArm.armData;
        if (newInteraction.armInputMode == armInputMode.Hold)
        {
            leftArmHeld = false;
            PlayerPhysics.Instance.startAcceleratingLeft = false;
            
        }
        PhysicsInteractionManager.instance.ResetInteractionReference(RobotArmPlacement.Left);
    }

    private void RightArm_started(InputAction.CallbackContext obj)
    {
        //read from a player definition what arm is equiped and send that info
        SO_PhysicsInteraction newInteraction = RobotManager.Instance.armManagement.rightArm.armData;
        if(newInteraction.armInputMode == armInputMode.Hold)
        {
            rightArmHeld = true;
            PlayerPhysics.Instance.startAcceleratingLeft = false;
            
        }
        PhysicsInteractionManager.instance.ResetInteractionReference(RobotArmPlacement.Right);
    }
    private void RightArm_performed(InputAction.CallbackContext obj)
    {
        
        //read from a player definition what arm is equiped and send that info
        SO_PhysicsInteraction newInteraction = RobotManager.Instance.armManagement.rightArm.armData;
        PhysicsInteractionManager.instance.StoreInteractionReference(RobotArmPlacement.Right, newInteraction);

        if (newInteraction.armInputMode == armInputMode.Press)
        {
            PlayerPhysics.Instance.LoadPhysicInteraction(newInteraction, RobotArmPlacement.Right);
        }
        else if (newInteraction.armInputMode == armInputMode.Hold)
        {
            if (PlayerPhysics.Instance.TestPhysicInteraction(newInteraction, RobotArmPlacement.Right))
            {
                PlayerPhysics.Instance.startAcceleratingRight = true;
            }
        }
        else Debug.Log("<color=red>Invalid arm input mode found</color>");
    }
    private void RightArm_canceled(InputAction.CallbackContext obj)
    {
        //read from a player definition what arm is equiped and send that info
        SO_PhysicsInteraction newInteraction = RobotManager.Instance.armManagement.rightArm.armData;
        if (newInteraction.armInputMode == armInputMode.Hold)
        {
            rightArmHeld = false;
            PlayerPhysics.Instance.startAcceleratingRight = false;
        }

        PhysicsInteractionManager.instance.ResetInteractionReference(RobotArmPlacement.Right);
    }

    private void OnDisable()
    {
        controls.RobotActions.Move.performed -= OnMove;
        controls.RobotActions.Move.canceled -= OnMove;

        controls.RobotActions.Rotate.performed -= OnRotate;
        controls.RobotActions.Rotate.canceled -= OnRotate;

        controls.RobotActions.Jump.performed -= OnJump;
        controls.RobotActions.Disable();
    }

    private void OnDestroy()
    {
        controls.Dispose();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        //Debug.Log("MoveInput: " + MoveInput);
    }

    private void OnRotate(InputAction.CallbackContext context)
    {
        RotateInput = context.ReadValue<Vector2>();
        //Debug.Log("RotateInput: " + RotateInput);
    }

    private void OnJump(InputAction.CallbackContext context) 
    {
        //Debug.Log("Jump!");
        if (PlayerPhysics.Instance.groundCheck.isGrounded && !RobotManager.Instance.physics.areWeAccelerating())
        {
            //load physic interaction
            PlayerPhysics.Instance.LoadPhysicInteraction(PhysicsInteractionManager.instance.interactionsList[(int)physicInteractions.Jump], RobotArmPlacement.Terminator);
            //remove ability to jump once you are jumping
            PlayerPhysics.Instance.groundCheck.isGrounded = false;
            PlayerPhysics.Instance.groundCheck.isJumping = true;
        }
    }
}
