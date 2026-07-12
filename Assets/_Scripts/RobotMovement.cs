using UnityEngine;

public class RobotMovement : MonoBehaviour
{

    [SerializeField] private Transform ballVisual;
    [SerializeField] private Transform bodyVisual;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float ballRadius = 0.5f;
    [SerializeField] private float rotationSpeed = 360f;

    private RobotController controllerScript;
    private Rigidbody robotRigidbody;

    private void Awake()
    {
        robotRigidbody = GetComponent<Rigidbody>();
        controllerScript = GetComponent<RobotController>();
    }

    private void FixedUpdate()
    {
        #region Movement

        Vector2 moveInput = controllerScript.MoveInput;

        //Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        //Vector3 NewbodyForward = robotRigidbody.transform.forward; //this works only if the robot is not tilted
       
        Vector3 bodyForward = Vector3.ProjectOnPlane(
            robotRigidbody.transform.forward,
            Vector3.up
        ).normalized;

        //this allows the ball to move laterally as well
        Vector3 bodyRight = Vector3.ProjectOnPlane(
            robotRigidbody.transform.right,
            Vector3.up
        ).normalized;

        Vector3 moveDirection =
            bodyRight * moveInput.x +
            bodyForward * moveInput.y;

        //this allows the ball to move only forward and backward
        //Vector3 moveDirection =
        //robotRigidbody.transform.forward * moveInput.y;


        if (moveDirection.sqrMagnitude > 0.01f) 
        {
            moveDirection.Normalize();

            float speedMultiplier = moveInput.magnitude; //Measures how far the stick is pushed: 0 at rest, roughly 1 at its edge.
            float distance = moveSpeed * speedMultiplier * Time.fixedDeltaTime;

            // Moves the Player Rigidbody, rather than applying torque to Ball.
            robotRigidbody.MovePosition(
                robotRigidbody.position + moveDirection * distance //current position, where you want to go, and how far you want to go in that direction (this frame alone)
            );

            // Visual rolling only.
            float rollAngle = (distance / ballRadius) * Mathf.Rad2Deg;
            Vector3 rollAxis = Vector3.Cross(Vector3.up, moveDirection);

            ballVisual.Rotate(rollAxis, rollAngle, Space.World);
        }
       
        #endregion

        #region Rotation

        Vector2 rotateInput = controllerScript.RotateInput;
        Vector3 rotateDirection = new Vector3(rotateInput.x, 0f, rotateInput.y);

        if (rotateDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation( //look rotation needs two direction, Forward and Up. Forward is the direction you want to look at, Up is the direction you want to be up.
                rotateDirection.normalized,
                Vector3.up
            );

            Quaternion newRotation = Quaternion.RotateTowards( //from, to, speed
                robotRigidbody.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );

            robotRigidbody.MoveRotation(newRotation);

            //bodyVisual.Rotate(, rollAngle, Space.World);
        }

        #endregion

    }
}