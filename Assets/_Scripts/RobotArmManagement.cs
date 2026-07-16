using System;
using UnityEngine;

public class RobotArmManagement : MonoBehaviour
{ 
    //have properties for left arm and right arm
    //each load in a different model, animation and physic interaction

    [Header("Initialize Arms")]
    public RobotArm leftArm;
    public RobotArm rightArm;

    [Header("Visuals")]
    [SerializeField] GameObject leftShoulderPivot;
    [SerializeField] GameObject rightShoulderPivot;

}

public enum RobotArmPlacement { Left, Right };
[Serializable]
public class RobotArm
{
    public GameObject model;
    public SO_PhysicsInteraction physicInteraction;
}
