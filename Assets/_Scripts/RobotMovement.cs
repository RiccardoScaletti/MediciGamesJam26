using Unity.VisualScripting;
using UnityEngine;

public class RobotMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform ballVisual;
    [SerializeField] private Transform bodyVisual;
    [SerializeField] private Transform cameraRoot;

    [Header("Camera")]
    [SerializeField] private float camLimitY;
    //[SerializeField] private float camLimitX;
    [SerializeField] private float camSpeed;
    float currentCamTiltY = 0;
    public bool invertCamY;
    float currentCamtiltX = 0;
    public bool invertCamX;
    //[SerializeField] float turnSpeed;
    [SerializeField] Vector3 cameraOffset;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float deceleration = 3f;

    [Header("Rotation")]
    [SerializeField] private float ballRadius = 0.5f;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float maxBodyTilt = 20f;
    [SerializeField] private float bodyTiltSpeed = 180f;

   

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

    private void Update()
    {
        //read input from controller script
        rotateInput = controllerScript.RotateInput;

        #region camera
        //follow player 
        cameraRoot.transform.position = robotRigidbody.transform.position + cameraOffset;

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
        //currentCamtiltX = Mathf.Clamp(currentCamtiltX, -camLimitX, camLimitX);

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

        // Player input controls the horizontal Rigidbody velocity. Keep the existing
        // vertical velocity so gravity, jumping, and knockback on the Y axis still work.
        Vector3 inputVelocity = moveDirection * moveSpeed;

        //// Accelerate faster while pushing input; coast down more slowly on release.
        //float rate = moveDirection.sqrMagnitude > 0.01f
        //    ? acceleration
        //    : deceleration;

        //glideVelocity = Vector3.MoveTowards(
        //    glideVelocity,
        //    targetVelocity,
        //    rate * Time.fixedDeltaTime
        //);

        robotRigidbody.linearVelocity = new Vector3(
            inputVelocity.x,
            robotRigidbody.linearVelocity.y,
            inputVelocity.z
        );

        // Visual rolling only.
        float rollAngle = ballRadius * Mathf.Rad2Deg;
        Vector3 rollAxis = Vector3.Cross(Vector3.up, moveDirection);
        ballVisual.Rotate(rollAxis, rollAngle, Space.World);

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
            float step = camSpeed * Time.fixedDeltaTime;
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

    public void OnSpawnResetVelocity()
    {
        Debug.Log("Reset called\n velocity = " + robotRigidbody.linearVelocity);

        controllerScript.ResetMoveInput();

        // Clear movement speed (Use rb.linearVelocity for Unity 6+)
        robotRigidbody.linearVelocity = Vector3.zero;

        // Clear rotation/spin speed
        robotRigidbody.angularVelocity = Vector3.zero;
    }
}
