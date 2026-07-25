using UnityEngine;

public class SpaceStationRotation : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 0f, 50f);
    
    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
