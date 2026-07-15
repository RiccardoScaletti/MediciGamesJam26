using UnityEngine;
using UnityEngine.InputSystem;

public class RobotController : MonoBehaviour
{
    private RobotControls controls;

    public Vector2 MoveInput { get; private set; }
    public Vector2 RotateInput { get; private set; }

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
        controls.RobotActions.RightArm.performed += RightArm_performed;
        controls.RobotActions.LeftArm.performed += LeftArm_performed;
    }

    private void LeftArm_performed(InputAction.CallbackContext obj)
    {
        //read from a player definition what arm is equiped and send that info
        PlayerPhysics.Instance.LoadPhysicInteraction(PhysicsInteractionManager.instance.interactionsList[(int)physicInteractions.Cannon]);
    }

    private void RightArm_performed(InputAction.CallbackContext obj)
    {
        PlayerPhysics.Instance.LoadPhysicInteraction(PhysicsInteractionManager.instance.interactionsList[(int)physicInteractions.Cannon]);
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
        if (PlayerPhysics.Instance.groundCheck.isGrounded)
        {
            //load physic interaction
            PlayerPhysics.Instance.LoadPhysicInteraction(PhysicsInteractionManager.instance.interactionsList[(int)physicInteractions.Jump]);
            //remove ability to jump once you are jumping
            PlayerPhysics.Instance.groundCheck.isGrounded = false;
            PlayerPhysics.Instance.groundCheck.isJumping = true;
        }
        
    }
}
