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
    }

    private void OnDisable()
    {
        controls.RobotActions.Move.performed -= OnMove;
        controls.RobotActions.Move.canceled -= OnMove;

        controls.RobotActions.Rotate.performed -= OnRotate;
        controls.RobotActions.Rotate.canceled -= OnRotate;

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
        PlayerPhysics.Instance.LoadPhysicInteraction(PhysicsInteractionManager.instance.interactionsList[(int)physicInteractions.Jump]);
    }
}
