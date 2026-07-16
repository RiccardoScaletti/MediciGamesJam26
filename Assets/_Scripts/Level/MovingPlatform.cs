using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform[] checkpoints;
    [SerializeField] private float speed = 15f;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float waitTime = 3f;

    private void Start()
    {
        if (checkpoints == null || checkpoints.Length == 0)
        {
            Debug.LogWarning("MovingPlatform has no checkpoints assigned.");
            enabled = false;
            return;
        }

        transform.position = checkpoints[0].position;
        transform.rotation = checkpoints[0].rotation;

        StartCoroutine(MovePlatform());
    }

    private IEnumerator MovePlatform()
    {
        int currentCheckpointIndex = 1 % checkpoints.Length; // Start from the second checkpoint since the platform is already at the first one.

        while (true)
        {
            Transform target = checkpoints[currentCheckpointIndex];

            while (transform.position != target.position || transform.rotation != target.rotation)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target.position,
                    speed * Time.deltaTime
                );
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    target.rotation,
                    rotationSpeed * Time.deltaTime
                );

                yield return null;
            }


            yield return new WaitForSeconds(waitTime);

            currentCheckpointIndex = (currentCheckpointIndex + 1) % checkpoints.Length; //advances to the next checkpoint and returns to 0 after the last one.

        }
    }
}