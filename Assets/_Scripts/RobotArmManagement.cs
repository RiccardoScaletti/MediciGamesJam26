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

    public void UpdateArmChoice(RobotArmPlacement armPlacement, SO_PhysicsInteraction robotArmData)
    {
        //TLDR; use robot arm data to store physic data and what prefab to load
        ClearArmChoice(armPlacement);
        //clear out previous arm Data and model

        //use armPlacement to determine whether to update left or right arm
        switch (armPlacement)
        {
            case RobotArmPlacement.Left:
                leftArm.armData = robotArmData;
                break;
            case RobotArmPlacement.Right:
                rightArm.armData = robotArmData;
                break;
            default:
            case RobotArmPlacement.Terminator:
                Debug.Log("<color=red> invalid arm placement passed<color>");
                return;
        }

        //update model and store its reference in the manager.
        UpdateArmModel(armPlacement, robotArmData);
    }

    private void UpdateArmModel(RobotArmPlacement armPlacement, SO_PhysicsInteraction robotArmData)
    {
        //read armdata and use arm placement to spawn a model at the corresponding location.

        Transform spawnTransform;
        GameObject prefab2Spawn;
        RobotArm targetRobotArm;
        switch (armPlacement)
        {
            case RobotArmPlacement.Left:
                spawnTransform = leftShoulderPivot.transform;
                prefab2Spawn = robotArmData.LeftArmModel;
                targetRobotArm = leftArm;
                break;
            case RobotArmPlacement.Right:
                spawnTransform = rightShoulderPivot.transform;
                prefab2Spawn = robotArmData.RightArmModel;
                targetRobotArm = rightArm;
                break;
            default:
            case RobotArmPlacement.Terminator:
                Debug.Log("<color=red> invalid arm placement passed<color>");
                return;
        }
        targetRobotArm.prefab = Instantiate(prefab2Spawn, spawnTransform.position,Quaternion.Euler(Vector3.zero));
        targetRobotArm.prefab.transform.SetParent(spawnTransform);  //set arm pivot as parent
        targetRobotArm.prefab.transform.localEulerAngles = Vector3.zero;  //reset rotation to 0
    }

    public void ClearArmChoice(RobotArmPlacement armPlacement)
    {
        //clear the arm passed stored in memory 
        switch (armPlacement)
        {
            case RobotArmPlacement.Left:
                Destroy(leftArm.prefab);
                leftArm.armData = null;
                leftArm.prefab = null;
                break;
            case RobotArmPlacement.Right:
                Destroy(rightArm.prefab);
                rightArm.armData = null;
                rightArm.prefab = null;
                break;
            case RobotArmPlacement.Terminator:
                break;
            default:
                break;
        }
    }
}

public enum RobotArmPlacement { Left, Right , Terminator};
[Serializable]
public class RobotArm
{
    //Have Reference to a instance of the prefab while its initiallized
    public GameObject prefab;
    //have field to load in any and all armData
    public SO_PhysicsInteraction armData;
    
}
