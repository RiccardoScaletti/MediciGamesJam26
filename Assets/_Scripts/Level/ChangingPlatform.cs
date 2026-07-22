using System.Collections;
using UnityEngine;

public class ChangingPlatform : MonoBehaviour
{
    [SerializeField] private Transform[] checkpoints;
    [SerializeField] private float speed = 15f;
    [SerializeField] private float rotationSpeed = 90f;

    private Rigidbody rb;
    private bool isMoving;
    private int currentCheckpointIndex = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (checkpoints == null || checkpoints.Length == 0)
        {
            Debug.LogWarning("ChangingPlatform has no checkpoints assigned.");
            enabled = false;
            return;
        }

        rb.position = checkpoints[0].position;
        rb.rotation = checkpoints[0].rotation;
    }

    private void OnEnable()
    {
        GameEvents.LevelCompleted += MoveToRandomCheckpoint;
    }

    private void OnDisable()
    {
        GameEvents.LevelCompleted -= MoveToRandomCheckpoint;
    }

    // Call this once when the level is completed.
    public void MoveToRandomCheckpoint()
    {
        if (isMoving || checkpoints.Length < 2)
            return;

        int nextIndex;
        do
        {
            nextIndex = Random.Range(0, checkpoints.Length);
        }
        while (nextIndex == currentCheckpointIndex);

        StartCoroutine(MovePlatform(checkpoints[nextIndex], nextIndex));
    }

    private IEnumerator MovePlatform(Transform target, int targetIndex)
    {
        isMoving = true;

        while (Vector3.Distance(rb.position, target.position) > 0.01f ||
               Quaternion.Angle(rb.rotation, target.rotation) > 0.1f)
        {
            rb.MovePosition(Vector3.MoveTowards(
                rb.position,
                target.position,
                speed * Time.fixedDeltaTime
            ));

            rb.MoveRotation(Quaternion.RotateTowards(
                rb.rotation,
                target.rotation,
                rotationSpeed * Time.fixedDeltaTime
            ));

            yield return new WaitForFixedUpdate();
        }

        rb.position = target.position;
        rb.rotation = target.rotation;
        currentCheckpointIndex = targetIndex;
        isMoving = false;
    }
}