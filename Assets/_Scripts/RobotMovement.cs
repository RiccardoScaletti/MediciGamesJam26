using UnityEngine;

public class RobotMovement : MonoBehaviour
{

    [SerializeField] private Transform ballVisual;
    [SerializeField] private Transform bodyVisual;
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float camLimit;
    [SerializeField] private float camSpeed;
    float currentCamTilt=0;
    public bool invertCam;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float ballRadius = 0.5f;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float maxBodyTilt = 20f;
    [SerializeField] private float bodyTiltSpeed = 180f;


    private RobotController controllerScript;
    private Rigidbody robotRigidbody;
    private Quaternion bodyBaseRotation;

    private void Awake()
    {
        robotRigidbody = GetComponent<Rigidbody>();
        controllerScript = GetComponent<RobotController>();
        bodyBaseRotation = bodyVisual.localRotation;
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
        Vector2 rotateInput = controllerScript.RotateInput;
        Vector3 rotateDirection = new Vector3(rotateInput.x, 0f, rotateInput.y);

        //if inputs are greater than deadzone
        if (Mathf.Abs(rotateInput.x) > 0.01f)
        {
            float turnAmount =
                rotateInput.x * rotationSpeed * Time.fixedDeltaTime;

            Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);

            robotRigidbody.MoveRotation(
                robotRigidbody.rotation * turnRotation
            );
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

        //Debug.Log(targetTilt);



        //iteration 1
        //find new camera rotation
        //Quaternion newCameraRotation = cameraRoot.localRotation * Quaternion.Euler(targetTilt, 0f, 0f);
        //limit rotation in x axis 
        //float newCameraX = Mathf.Clamp(newCameraRotation.x, -camLimit, camLimit);
        //newCameraRotation = Quaternion.Euler(new Vector3(newCameraRotation.x, newCameraRotation.y, newCameraRotation.z));

        //apply camera rotation
        //cameraRoot.localRotation = cameraRoot.localRotation * Quaternion.Euler(targetTilt * Time.fixedDeltaTime, 0f, 0f) ;

        int camDirection = 1;
        if (invertCam)
        {
            camDirection = -1;
        }

        currentCamTilt += rotateInput.y * Time.fixedDeltaTime * camSpeed * camDirection;
        currentCamTilt = Mathf.Clamp(currentCamTilt, -camLimit, camLimit);
        cameraRoot.localRotation = Quaternion.Euler(currentCamTilt, 0f, 0f);

        //if(rotateInput == Vector2.zero)
        //{
        //    targetTilt = 0f;
        //}

        //iteration 2
        //float newCameraX = cameraRoot.rotation.x + targetTilt * camSpeed * Time.deltaTime;
        //newCameraX = Mathf.Clamp(newCameraX, -camLimit, camLimit);

        //cameraRoot.Rotate(newCameraX, 0,0);

        //iteration 3
        //float newCameraX = cameraRoot.rotation.x + targetTilt * camSpeed*Time.fixedDeltaTime;
        //Debug.Log("before clamp"+newCameraX);
        //newCameraX = Mathf.Clamp(newCameraX, -targetTilt, targetTilt);
        //Debug.Log("after clamp" + newCameraX);
        //cameraRoot.rotation = Quaternion.Euler(newCameraX, cameraRoot.rotation.y, cameraRoot.rotation.z);

        #endregion

    }
}