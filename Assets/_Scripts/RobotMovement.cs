using UnityEngine;

public class RobotMovement : MonoBehaviour
{
    
    [SerializeField] private Transform ballVisual;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float ballRadius = 0.5f;

    private CharacterController controllerScript;
    private Rigidbody robotRigidbody;

    private void Awake()
    {
        robotRigidbody = GetComponent<Rigidbody>();
        controllerScript = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        Vector2 moveInput = controllerScript.MoveInput;

        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);

        if (moveDirection.sqrMagnitude <= 0.01f)
            return;

        moveDirection.Normalize();

        float speedMultiplier = moveInput.magnitude;
        float distance = moveSpeed * speedMultiplier * Time.fixedDeltaTime;

        // Moves the Player Rigidbody, rather than applying torque to Ball.
        robotRigidbody.MovePosition(
            robotRigidbody.position + moveDirection * distance
        );

        // Visual rolling only.
        float rollAngle = (distance / ballRadius) * Mathf.Rad2Deg;
        Vector3 rollAxis = Vector3.Cross(Vector3.up, moveDirection);

        ballVisual.Rotate(rollAxis, rollAngle, Space.World);
    }
}