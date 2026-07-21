using UnityEngine;

public class LevelRotation : MonoBehaviour
{
    [SerializeField] private Transform arenaObj;
    [SerializeField] private float rotationSpeed = 5f;
    bool needsRotation;
    Quaternion targetRotation;

    public void RotateLevel()
    {
        Debug.Log("Rotate Level!");
        targetRotation = Quaternion.Euler(0f, 0f, arenaObj.transform.eulerAngles.z + 90f);

        needsRotation = true;
 
    }

    private void Update()
    {
        Debug.Log("needs rotation: " + needsRotation);
        if (needsRotation)
        {
            arenaObj.rotation = Quaternion.Lerp(
                arenaObj.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            float remainingAngle = Quaternion.Angle(targetRotation, arenaObj.rotation);
            if (remainingAngle <= 0.1f)
            {
                needsRotation = false;
                arenaObj.rotation = targetRotation; // Snap to the exact target rotation
            }
        }

    }
}
