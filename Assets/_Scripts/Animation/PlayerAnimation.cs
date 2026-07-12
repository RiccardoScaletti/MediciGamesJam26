using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    //take input -1 to 1
    //decide to apply backwards if less than zero, apply forwards at greater than zero

    //stretch goals: scale speed to input value

    private Animator animator;
    [SerializeField] RobotController robotController;

    [SerializeField] private float testFloat;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if(animator == null) { Debug.Log("<color=red>animator component not found!</color>"); }
    }

    private void Update()
    {
        testFloat = robotController.MoveInput.y;
        animator.SetFloat("Direction", testFloat);
    }
}
