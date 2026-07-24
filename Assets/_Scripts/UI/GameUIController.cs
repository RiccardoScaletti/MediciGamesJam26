using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum ArmChoice
{
    Cannon,
    Sticky,
    Saw,
    Empty
}

public class GameUIController : MonoBehaviour
{

    [SerializeField] private GameObject menuPanel;

    [Header("Arm choices")]
    [SerializeField] private GameObject leftArmsGO;
    [SerializeField] private GameObject rightArmsGO;
    [SerializeField] private Button[] leftArmChoices;
    [SerializeField] private Button[] rightArmChoices;

    [SerializeField] private Image robotImage;
    [SerializeField] private Image[] leftArmImages;
    [SerializeField] private Image[] rightArmImages;

    [SerializeField] private SO_PhysicsInteraction[] armsSO;

    private ArmChoice currentArmChoice;
    public ArmChoice chosenLeftArm = ArmChoice.Empty;
    public ArmChoice chosenRightArm = ArmChoice.Empty;

    private RobotControls controls;

    private void Awake()
    {
        controls = new RobotControls();

        //setup robot image and arms
        foreach (Image img in leftArmImages)
        {
            img.gameObject.SetActive(false);
        }
        foreach (Image img in rightArmImages)
        {
            img.gameObject.SetActive(false);
        }
    }

    #region Controls Setup
    private void OnEnable()
    {
        controls.UIActions.SelectRight.performed += SelectRightChoice;
        controls.UIActions.SelectLeft.performed += SelectLeftChoice;
        controls.UIActions.SelectUp.performed += SelectUpChoice;
        controls.UIActions.Confirm.performed += MakeChoice;

        controls.UIActions.Enable();
    }


    private void OnDisable()
    {
        controls.UIActions.SelectRight.performed -= SelectRightChoice;
        controls.UIActions.SelectLeft.performed -= SelectLeftChoice;
        controls.UIActions.SelectUp.performed -= SelectUpChoice;
        controls.UIActions.Confirm.performed -= MakeChoice;

        controls.UIActions.Disable();
    }

    private void OnDestroy()
    {
        controls.Dispose();
    }

    #endregion

    #region Visuals
    public void ShowChoiceMenu()
    {
        //Check if not first time
        if (chosenLeftArm != ArmChoice.Empty)
        {
            switch (chosenLeftArm)
            {
                case ArmChoice.Cannon:
                    leftArmChoices[0].interactable = false;
                    break;
                case ArmChoice.Sticky:
                    leftArmChoices[1].interactable = false;
                    break;
                case ArmChoice.Saw:
                    leftArmChoices[2].interactable = false;
                    break;
                default:
                    Debug.LogError("Invalid arm choice: " + chosenLeftArm);
                    break;
            }
        }
        else { Debug.Log("First time choosing arms"); }


        if (chosenRightArm != ArmChoice.Empty)
        {
            switch (chosenRightArm)
            {
                case ArmChoice.Cannon:
                    rightArmChoices[0].interactable = false;
                    break;
                case ArmChoice.Sticky:
                    rightArmChoices[1].interactable = false;
                    break;
                case ArmChoice.Saw:
                    rightArmChoices[2].interactable = false;
                    break;
                default:
                    Debug.LogError("Invalid arm choice: " + chosenLeftArm);
                    break;
            }
        }else { Debug.Log("First time choosing arms"); }

        //visuals setup
        menuPanel.SetActive(true);
        leftArmsGO.SetActive(true);
        rightArmsGO.SetActive(false);

        chosenLeftArm = ArmChoice.Empty;
        chosenRightArm = ArmChoice.Empty;

        //disable player controls while in menu
        GameManager.instance.DisableControls();
    }

    public void HideChoiceMenu()
    {
        menuPanel.SetActive(false);

        //resetting interactable buttons for next time
        foreach (Button button in leftArmChoices)
        {
            button.interactable = true;
        }
        foreach (Button button in rightArmChoices)
        {
            button.interactable = true;
        }

        //give controls back to character
        GameManager.instance.EnableControls();
    }

    private void CleanLeftArmsSelection()
    {
        foreach (Image img in leftArmImages)
        {
            img.gameObject.SetActive(false);
        }
    }

    private void CleanRightArmsSelection()
    { 
        foreach (Image img in rightArmImages)
        {
            img.gameObject.SetActive(false);
        }
    }


    private void ActivateLeftArmImage(int index)
    {
        leftArmImages[index].gameObject.SetActive(true);
    }

    private void ActivateRightArmImage(int index)
    {
        rightArmImages[index].gameObject.SetActive(true);
    }

    #endregion  

    private void SelectLeftChoice(InputAction.CallbackContext context)
    {
        //Debug.Log("Select left choice");
        if (chosenLeftArm == ArmChoice.Empty)
        {
            if (leftArmChoices[0].interactable)
            {
                leftArmChoices[0].Select();
                CleanLeftArmsSelection();
                ActivateLeftArmImage(0);
            }
            else return; // If the choice is not interactable, do nothing
        }
        else if(chosenRightArm == ArmChoice.Empty)
        {
            if (rightArmChoices[0].interactable)
            { 
                rightArmChoices[0].Select();
                CleanRightArmsSelection();
                ActivateRightArmImage(0);
            }
            else return; // If the choice is not interactable, do nothing
        }

        currentArmChoice = ArmChoice.Cannon;
    }
    private void SelectUpChoice(InputAction.CallbackContext context)
    {
        //Debug.Log("Select up choice");
        if (chosenLeftArm == ArmChoice.Empty)
        {
            if (leftArmChoices[1].interactable)
            {
                leftArmChoices[1].Select();
                CleanLeftArmsSelection(); 
                ActivateLeftArmImage(1);
            }
            else return; // If the choice is not interactable, do nothing
        }
        else if (chosenRightArm == ArmChoice.Empty)
        {
            if (rightArmChoices[1].interactable)
            {
                rightArmChoices[1].Select();
                CleanRightArmsSelection();
                ActivateRightArmImage(1);
            }

            else return; // If the choice is not interactable, do nothing
        }

        currentArmChoice = ArmChoice.Sticky;
    }
    private void SelectRightChoice(InputAction.CallbackContext context)
    {
        //Debug.Log("Select right choice");
        if (chosenLeftArm == ArmChoice.Empty)
        {
            if (leftArmChoices[2].interactable)
            { 
                leftArmChoices[2].Select();
                CleanLeftArmsSelection();
                ActivateLeftArmImage(2);
            }
            else return; // If the choice is not interactable, do nothing
        }
        else if (chosenRightArm == ArmChoice.Empty)
        {
            if (rightArmChoices[2].interactable)
            {
                rightArmChoices[2].Select();
                CleanRightArmsSelection();
                ActivateRightArmImage(2);
            }
            else return; // If the choice is not interactable, do nothing
        }

        currentArmChoice = ArmChoice.Saw;
    }

    private void MakeChoice(InputAction.CallbackContext context)
    {
        if (chosenLeftArm == ArmChoice.Empty) //skip if arm was already chosen
        {
            chosenLeftArm = currentArmChoice;
            //Debug.Log("Left arm choice: " + chosenLeftArm);

            leftArmsGO.SetActive(false);
            rightArmsGO.SetActive(true);

            return; // Keep menu open for right arm
        }

        if (chosenRightArm == ArmChoice.Empty)
        {

            chosenRightArm = currentArmChoice;
            //Debug.Log("Right arm choice: " + chosenRightArm);

            HideChoiceMenu();
        }

        AssignArmsAfterChoice();
    }

    private void AssignArmsAfterChoice()
    {
        switch (chosenLeftArm)
        {
            case ArmChoice.Cannon:
                RobotManager.Instance.armManagement.UpdateArmChoice(RobotArmPlacement.Left, armsSO[0]);
                break;
            case ArmChoice.Sticky:
                RobotManager.Instance.armManagement.UpdateArmChoice(RobotArmPlacement.Left, armsSO[1]);
                break;
            case ArmChoice.Saw:
                RobotManager.Instance.armManagement.UpdateArmChoice(RobotArmPlacement.Left, armsSO[2]);
                break;
            case ArmChoice.Empty:
                Debug.LogWarning("NO LEFT ARM SPAWNED");
                break;
            default:
                break;
        }

        switch (chosenRightArm)
        {
            case ArmChoice.Cannon:
                RobotManager.Instance.armManagement.UpdateArmChoice(RobotArmPlacement.Right, armsSO[0]);
                break;
            case ArmChoice.Sticky:
                RobotManager.Instance.armManagement.UpdateArmChoice(RobotArmPlacement.Right, armsSO[1]);
                break;
            case ArmChoice.Saw:
                RobotManager.Instance.armManagement.UpdateArmChoice(RobotArmPlacement.Right, armsSO[2]);
                break;
            case ArmChoice.Empty:
                Debug.LogWarning("NO RIGHT ARM SPAWNED");
                break;
            default:
                break;
        }

    }
}
