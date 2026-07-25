using System.Collections.Generic;
using UnityEngine;

public enum physicInteractions { Jump, Cannon, StickyHand, SawHand, SawHandJump}
public class PhysicsInteractionManager : MonoBehaviour
{
    public static PhysicsInteractionManager instance;
    public List<SO_PhysicsInteraction> interactionsList = new List<SO_PhysicsInteraction>();

    public SO_PhysicsInteraction interactionLoadedLeft;
    public SO_PhysicsInteraction interactionLoadedRight;

    private void Start()
    {
        if(instance == null) { instance = this;}
        else { Destroy(this.gameObject); }
    }

    public void ResetInteractionReference(RobotArmPlacement armPosition)
    {
        switch (armPosition)
        {
            case RobotArmPlacement.Left:
                interactionLoadedLeft = null;
                break;
            case RobotArmPlacement.Right:
                interactionLoadedRight = null;
                break;
            default:
            case RobotArmPlacement.Terminator:
                return;
        }
        
    }

    public void StoreInteractionReference(RobotArmPlacement armPosition, SO_PhysicsInteraction interaction)
    {
        switch (armPosition)
        {
            case RobotArmPlacement.Left:
                interactionLoadedLeft = interaction;
                break;
            case RobotArmPlacement.Right:
                interactionLoadedRight = interaction;
                break;
            default:
            case RobotArmPlacement.Terminator:
                return ;
        }

    }
}
