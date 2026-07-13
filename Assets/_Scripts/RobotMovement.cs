using UnityEngine;

public class RobotMovement : MonoBehaviour
{

    [SerializeField] private Transform ballVisual;
    [SerializeField] private Transform bodyVisual;
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float camLimitY;
    [SerializeField] private float camLimitX;
    [SerializeField] private float camSpeed;
    float currentCamTiltY=0;
    public bool invertCamY;
    float currentCamtiltX = 0;
    public bool invertCamX;
    [SerializeField]float turnSpeed;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float ballRadius = 0.5f;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float maxBodyTilt = 20f;
    [SerializeField] private float bodyTiltSpeed = 180f;


    private RobotController controllerScript;
    private Rigidbody robotRigidbody;
    private Quaternion bodyBaseRotation;

    Vector2 rotateInput;

    private void Awake()
    {
        robotRigidbody = GetComponent<Rigidbody>();
        controllerScript = GetComponent<RobotController>();
        bodyBaseRotation = bodyVisual.localRotation;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        //read input from controller script
        rotateInput = controllerScript.RotateInput;

        #region camera
        //follow player 
        cameraRoot.transform.position = robotRigidbody.transform.position;

        //rotate around player
        int camDirectionY = 1;
        if (invertCamY)
        {
            camDirectionY = -1;
        }
        int camDirectionX = 1;
        if (invertCamX)
        {
            camDirectionX = -1;
        }


        //add camera tilt input into cam pitch
        currentCamTiltY += rotateInput.y * Time.deltaTime * camSpeed * camDirectionY;
        currentCamTiltY = Mathf.Clamp(currentCamTiltY, -camLimitY, camLimitY);
        
        //add camera tilt input into cam yaw
        currentCamtiltX += rotateInput.x * Time.deltaTime * camSpeed * camDirectionX;
        currentCamtiltX += Mathf.Clamp(currentCamtiltX, -camLimitX, camLimitX);

        cameraRoot.localRotation = Quaternion.Euler(currentCamTiltY, currentCamtiltX, 0f);

        #endregion
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
        //read inputs
        
        Vector3 rotateDirection = new Vector3(rotateInput.x, 0f, rotateInput.y);

        //if inputs are greater than deadzone
        if (Mathf.Abs(rotateInput.x) > 0.01f)
        {
            float turnAmount =
                rotateInput.x * rotationSpeed * Time.fixedDeltaTime;

            Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);

            //robotRigidbody.MoveRotation(
            //    robotRigidbody.rotation * turnRotation
            //);

            //find difference between player rotation and camera rotation
            /*Vector3 rotationTarget = cameraRoot.transform.rotation - robotRigidbody.transform.rotation*/
            float step = turnSpeed * Time.fixedDeltaTime;
            robotRigidbody.transform.rotation = Quaternion.RotateTowards(transform.rotation, cameraRoot.transform.rotation, step);
        }

        //find tilt value
        float targetTilt = -rotateInput.y * maxBodyTilt;
        //tilt body
        Quaternion targetBodyRotation =
            bodyBaseRotation * Quaternion.Euler(targetTilt, 0f, 0f);
        //tilt body
        bodyVisual.localRotation = Quaternion.RotateTowards(
            bodyVisual.localRotation,
            targetBodyRotation,
            bodyTiltSpeed * Time.fixedDeltaTime
        );
        #endregion

        

    }
}