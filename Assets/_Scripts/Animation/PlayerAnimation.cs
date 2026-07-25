using System.Collections;
using UnityEngine;

public enum armAnimationStates { Idle, Cannon, Claw, Saw}
public class PlayerAnimation : MonoBehaviour
{
    //take input -1 to 1
    //decide to apply backwards if less than zero, apply forwards at greater than zero

    //stretch goals: scale speed to input value

    [SerializeField] Animator leftArmAnimator, rightArmAnimator;
    [SerializeField] float maxBallTilt;
    [SerializeField] float ballTiltSpeed;
    [SerializeField] GameObject torso_Root;
    public int leftArmState, rightArmState;

    private float pitchValue = 0;
    private float yawValue = 0;
    [SerializeField] private float testFloat;
    [SerializeField] private float cannonTimeDelay;

    private void Update()
    {
        Vector2 moveInput = RobotManager.Instance.robotController.MoveInput;

        #region torso tilt
        if (moveInput.y != 0)
        {
            pitchValue += -1* moveInput.y * ballTiltSpeed * Time.deltaTime;
            pitchValue = Mathf.Clamp(pitchValue, -maxBallTilt, maxBallTilt);
        }
        else if(moveInput.y == 0)
        {
            pitchValue = Mathf.Lerp(pitchValue, 0, ballTiltSpeed* Time.deltaTime);
        }

        if (moveInput.x != 0) 
        {
            yawValue +=  moveInput.x * ballTiltSpeed * Time.deltaTime;
            yawValue = Mathf.Clamp(yawValue, -maxBallTilt, maxBallTilt);
        }else if ( moveInput.x==0)
        {
            yawValue = Mathf.Lerp(yawValue, 0, ballTiltSpeed * Time.deltaTime);
        }

        torso_Root.transform.localRotation = Quaternion.Euler(pitchValue, 0, yawValue);
        #endregion


        //testFloat = robotArmLeft.MoveInput.y;
        //animator.SetFloat("Direction", testFloat);
    }

    public void UpdateArmAnimationState(RobotArmPlacement armPlacement, int armState, bool isStateActive)
    {
        Animator thisAnimator;
        switch (armPlacement)
        {
            case RobotArmPlacement.Left:
                thisAnimator = leftArmAnimator;
                break;
            case RobotArmPlacement.Right:
                thisAnimator = rightArmAnimator;
                break;
            default:
            case RobotArmPlacement.Terminator:
                thisAnimator = null;
                break;
        }

        if(thisAnimator != null)
        {
            thisAnimator.SetBool("StateActive", isStateActive);
            thisAnimator.SetInteger("CurrentState", armState);
        }
       //thisAnimator.SetInteger("CurrentState", 0);
    }

    IEnumerator delayUpdateAnimationState(RobotArmPlacement armPlacement, int armState, bool isStateActive)
    {
        yield return new WaitForSeconds(cannonTimeDelay);
        UpdateArmAnimationState(armPlacement,armState, isStateActive);
    }

    public void callDelayedAnimationUpdate(RobotArmPlacement armPlacement, int armState, bool isStateActive)
    {
        StartCoroutine(delayUpdateAnimationState(armPlacement, armState, isStateActive));
    }
}
