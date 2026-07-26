using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUDCanvas : MonoBehaviour
{
    public Color inactiveColor, activeColor;
    public Image lefArmIcon, rightArmIcon, leftArmFill, rightArmFill;
    public TMP_Text leftArmtext, rightArmText;

    private void Start()
    {
        RobotManager.Instance.gameHUDCanvas = this;
        this.gameObject.SetActive(false);
    }

    public void UpdateArmData()
    {
        RobotArm leftArm = RobotManager.Instance.armManagement.leftArm;
        RobotArm rightArm = RobotManager.Instance.armManagement.rightArm;

        lefArmIcon.sprite = leftArm.armData.spriteIcon;
        leftArmtext.text = leftArm.armData.armName;

        rightArmIcon.sprite = rightArm.armData.spriteIcon;
        rightArmText.text = rightArm.armData.armName;
    }

    public void MakeIconInactive(RobotArmPlacement armPlacement, bool toggle)
    {
        Image image2Alter;
        switch (armPlacement)
        {
            case RobotArmPlacement.Left:
                image2Alter = lefArmIcon;
                break;
            case RobotArmPlacement.Right:
                image2Alter = rightArmIcon;
                break;
            default:
            case RobotArmPlacement.Terminator:
                return;
         
         
        }

        if (toggle) 
        {
            image2Alter.color = activeColor;
        }
        else
        {
            image2Alter.color = inactiveColor;
        }
    }
}
