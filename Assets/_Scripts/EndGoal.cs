using UnityEngine;

public class EndGoal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameManager.instance.GameOver();    
    }
}
