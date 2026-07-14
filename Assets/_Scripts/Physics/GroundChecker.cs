using System.Collections;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public bool isGrounded;
    PlayerPhysics physicsScr;
    public bool isJumping;

    private void OnTriggerEnter(Collider other)
    {
        //when landing, reset y velocity to 0
        physicsScr.ResetFallingVelocity();
    }

    private void OnTriggerStay(Collider other)
    {
        //when not jumping, check to see if you are grounded
        //in the case of crashing or clipping, this lets you have access to jumps to maybe get out
        if (!isJumping)
        {
            isGrounded = true;
        }
        
        
    }

    private void OnTriggerExit(Collider other)
    {
        //when leaving the ground, start a coyote timer to be able to jump with input wiggle room
        StartCoroutine(nameof(CoyoteTimer));
    }

    public void InitializeScript( PlayerPhysics scr)
    {
        physicsScr = scr;
    }

    private IEnumerator CoyoteTimer()
    {
        yield return new WaitForSeconds(physicsScr.coyoteTimer);
        isGrounded = false;
    }
}
