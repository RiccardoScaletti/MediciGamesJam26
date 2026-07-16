using UnityEngine;

public class PlatformCollision : MonoBehaviour
{
    [SerializeField] Transform playfromPosition;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the moving platform.");
            other.gameObject.transform.parent = playfromPosition;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exit the moving platform.");
            other.gameObject.transform.parent = null;
        }
    }

}
