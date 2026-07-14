using UnityEngine;

public class RobotMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform ballVisual;
    [SerializeField] private Transform bodyVisual;
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float camLimitY;
    [SerializeField] private float camLimitX;
    [SerializeField] private float camSpeed;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float deceleration = 3f;

    [Header("Rotation")]
    [SerializeField] private float ballRadius = 0.5f;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float maxBodyTilt = 20f;
    [SerializeField] private float bodyTiltSpeed = 180f;

    float currentCamTiltY=0;
    public bool invertCamY;
    float currentCamtiltX = 0;
    public bool invertCamX;
    [SerializeField]float turnSpeed;

    private RobotController controllerScript;
    private Rigidbody robotRigidbody;
    private Quaternion bodyBaseRotation;
    private Vector3 glideVelocity;

    Vector2 rotateInput;

    private void Awake()
    {
        robotRigidbody = GetComponent<Rigidbody>();
        controllerScript = GetComponent<RobotController>();
        bodyBaseRotation = bodyVisual.localRotation;

        Vector3 initialRotation = cameraRoot.localEulerAngles;
        currentCamTiltY = ToSignedAngle(initialRotation.x);
        currentCamtiltX = ToSignedAngle(initialRotation.y);
    }

    private static float ToSignedAngle(float angle)
    {
        return angle > 180f ? angle - 360f : angle;
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
        currentCamtiltX = Mathf.Clamp(currentCamtiltX, -camLimitX, camLimitX);

        cameraRoot.localRotation = Quaternion.Euler(currentCamTiltY, currentCamtiltX, 0f);

        #endregion
    }

    private void FixedUpdate()
    {
        #region Movement

        Vector2 moveInput = controllerScript.MoveInput;

        Vector3 bodyForward = Vector3.ProjectOnPlane(
            robotRigidbody.transform.forward,
            Vector3.up
        ).normalized;

        Vector3 bodyRight = Vector3.ProjectOnPlane(
            robotRigidbody.transform.right,
            Vector3.up
        ).normalized;

        Vector3 moveDirection =
            bodyRight * moveInput.x +
            bodyForward * moveInput.y;

        // The speed we want to eventually reach.
        Vector3 targetVelocity = moveDirection * moveSpeed;

        // Accelerate faster while pushing input; coast down more slowly on release.
        float rate = moveDirection.sqrMagnitude > 0.01f
            ? acceleration
            : deceleration;

        glideVelocity = Vector3.MoveTowards(
            glideVelocity,
            targetVelocity,
            rate * Time.fixedDeltaTime
        );

        // Move using the manufactured velocity.
        robotRigidbody.MovePosition(
            robotRigidbody.position + glideVelocity * Time.fixedDeltaTime
        );

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