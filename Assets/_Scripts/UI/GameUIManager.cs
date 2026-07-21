using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button[] choiceButtons = new Button[6]; //ok button is last in the array
    [SerializeField] private Button okButton;

    [Header("Optional - one highlight/outline per choice button")]
    [SerializeField] private GameObject[] selectionMarkers = new GameObject[6];

    // Runs only when the player presses OK.
    // Values are the indexes of the two selected choices.
    public Action<int, int> OnChoicesConfirmed;

    private RobotControls controls;
    private readonly List<int> selectedChoices = new List<int>(2);

    // 0-5 = choice buttons, 6 = OK button.
    private int currentNavigationIndex;

    private void Awake()
    {
        controls = new RobotControls();
        menuPanel.SetActive(false);

        // Only the first six buttons are attachment choices.
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int choiceIndex = i;
            choiceButtons[i].onClick.AddListener(() => ToggleChoice(choiceIndex));
        }

        okButton.onClick.AddListener(ConfirmSelections);
    }

    private void OnEnable()
    {
        controls.UIActions.SelectNext.performed += GoToNextChoice;
        controls.UIActions.SelectPrevious.performed += GoToPreviousChoice;
        controls.UIActions.Confirm.performed += ConfirmCurrent;

        controls.UIActions.Enable();
    }

    private void OnDisable()
    {
        controls.UIActions.SelectNext.performed -= GoToNextChoice;
        controls.UIActions.SelectPrevious.performed -= GoToPreviousChoice;
        controls.UIActions.Confirm.performed -= ConfirmCurrent;

        controls.UIActions.Disable();
    }

    private void OnDestroy()
    {
        controls.Dispose();
    }

    public void ShowChoiceMenu()
    {
        //visuals
        selectedChoices.Clear();
        menuPanel.SetActive(true);
        for (int i = 0; i < selectionMarkers.Length; i++)
        {
            selectionMarkers[i].SetActive(false);
        }

        //conditions
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            //XXX Example availability condition:
            // choiceButtons[i].interactable = i < 4;
        }

        UpdateVisuals();

        currentNavigationIndex = 0;
        SelectCurrentButton();

        //disable player controls while in menu
        GameManager.instance.DisableControls();
    }

    public void HideChoiceMenu()
    {
        menuPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);

        //give controls back to character
        GameManager.instance.EnableControls();
    }

    #region selection across buttons
    private void SelectCurrentButton()
    {
        choiceButtons[currentNavigationIndex].Select();
    }

    private int FindFirstAvailableChoice()
    {
        return 0;
        //XXX need to implement a check for the first available choice button if some are disabled. For now, we assume all are available.
    }

    private void GoToNextChoice(InputAction.CallbackContext context)
    {
        if (menuPanel.activeSelf) MoveSelection(1);
        Debug.Log("Next Choice");
    }

    private void GoToPreviousChoice(InputAction.CallbackContext context)
    {
        if (menuPanel.activeSelf) MoveSelection(-1);
        Debug.Log("Previous choice");
    }

    private void MoveSelection(int direction)
    {
        int nextIndex = currentNavigationIndex;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            nextIndex = (nextIndex + direction + choiceButtons.Length)
                        % choiceButtons.Length;

            if (choiceButtons[nextIndex].interactable)
            {
                currentNavigationIndex = nextIndex;
                SelectCurrentButton();

                Debug.Log($"Current selection index: {currentNavigationIndex}");
                return;
            }
        }
    }

    #endregion

    private void ConfirmCurrent(InputAction.CallbackContext context)
    {
        if (!menuPanel.activeSelf)
            return;

        ToggleChoice(currentNavigationIndex);
    }


    private void ToggleChoice(int choiceIndex)
    {
        if (!choiceButtons[choiceIndex].interactable)
            return;

        if (selectedChoices.Contains(choiceIndex))
        {
            selectedChoices.Remove(choiceIndex);
        }
        else
        { 
            // Player must choose exactly two.
            if (selectedChoices.Count >= 2)
                return;

            selectedChoices.Add(choiceIndex);
        }

        UpdateVisuals();
    }

    private void ConfirmSelections()
    {
        if (selectedChoices.Count != 2)
            return;

        OnChoicesConfirmed?.Invoke(selectedChoices[0], selectedChoices[1]);

        // Put your own post-OK logic here, or subscribe to OnChoicesConfirmed elsewhere.
        Debug.Log($"Confirmed choices: {selectedChoices[0] + 1} and {selectedChoices[1] + 1}");

        HideChoiceMenu();
    }

    private void UpdateVisuals()
    {
        bool choicesDone;
        if (selectedChoices.Count == 2) choicesDone = true;
        else choicesDone = false;

        okButton.interactable = choicesDone;//activate OK button only when two choices are selected

        for (int i = 0; i < selectionMarkers.Length; i++)
        {
            if (selectionMarkers[i] != null)
                selectionMarkers[i].SetActive(selectedChoices.Contains(i));
        }
    }


}