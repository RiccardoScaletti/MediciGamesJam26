using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenuControls : MonoBehaviour
{
    private RobotControls controls;

    [SerializeField]private Button[] optionButtons;
    private int buttonsIndex = 0;

    private void Awake()
    {
        controls = new RobotControls();
    }

    private void Start()
    {
        optionButtons[0].Select();
    }

    private void OnEnable()
    {
        controls.UIActions.Enable();

        controls.UIMainMenu.NextOption.performed += OnSelectNext;
        controls.UIMainMenu.PreviousOption.performed += OnSelectPrevious;
        controls.UIMainMenu.Confirm.performed += OnConfirm;

    }

    private void OnDisable()
    {
        controls.UIActions.Disable();

        controls.UIMainMenu.NextOption.performed -= OnSelectNext;
        controls.UIMainMenu.PreviousOption.performed -= OnSelectPrevious;
        controls.UIMainMenu.Confirm.performed -= OnConfirm;
    }

    private void OnConfirm(InputAction.CallbackContext context)
    {
        optionButtons[buttonsIndex].onClick.Invoke();
    }

    private void OnSelectPrevious(InputAction.CallbackContext context)
    {
        buttonsIndex++;
        if (buttonsIndex > optionButtons.Length)
        {
            buttonsIndex = 0;
        }
        optionButtons[buttonsIndex].Select();
    }

    private void OnSelectNext(InputAction.CallbackContext context)
    {
        buttonsIndex--;
        if (buttonsIndex < 0)
        {
            buttonsIndex = 0;
        }
        optionButtons[buttonsIndex].Select();
    }

}
