using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenuControls : MonoBehaviour
{
    private RobotControls controls;

    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject controllerCanvas;
    [SerializeField] private GameObject tutorialCanvas;

    [Header("Audio")]
    [SerializeField] private AudioClip switchSound;
    [SerializeField] private AudioSource FXAudioSource;

    [SerializeField] private Button[] optionButtons;
    [SerializeField] private Button okTutorialButton;
    [SerializeField] private Button okControllerButton;
    [SerializeField] private int buttonsIndex = 0;

    private void Awake()
    {
        controls = new RobotControls();
        mainMenuCanvas.SetActive(true);
        tutorialCanvas.SetActive(false);
        controllerCanvas.SetActive(false);
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
        FXAudioSource.PlayOneShot(switchSound);
        optionButtons[buttonsIndex].onClick.Invoke(); 
    }

    private void OnSelectPrevious(InputAction.CallbackContext context)
    {
        FXAudioSource.PlayOneShot(switchSound);
        buttonsIndex++;
        if (buttonsIndex > optionButtons.Length)
        {
            buttonsIndex = 0;
        }
        optionButtons[buttonsIndex].Select();
    }

    private void OnSelectNext(InputAction.CallbackContext context)
    {
        FXAudioSource.PlayOneShot(switchSound);
        buttonsIndex--;
        if (buttonsIndex < 0)
        {
            buttonsIndex = 0;
        }
        optionButtons[buttonsIndex].Select();
    }

    public void OnTutorialPressed()
    {
        Debug.Log("OnTutorialPressed");
        FXAudioSource.PlayOneShot(switchSound);
        controllerCanvas.SetActive(true);
        tutorialCanvas.SetActive(false);
        mainMenuCanvas.SetActive(false);
        okControllerButton.Select();
    }
    public void OnOkControllerPressed()
    {
        Debug.Log("ok controller pressed");
        FXAudioSource.PlayOneShot(switchSound);
        controllerCanvas.SetActive(false);
        tutorialCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);
        okTutorialButton.Select();
    }

    public void OnOkTutorialPressed()
    {
        Debug.Log("ok tutorial pressed");
        FXAudioSource.PlayOneShot(switchSound);
        controllerCanvas.SetActive(false);
        tutorialCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);   
        optionButtons[0].Select();
    }

  

}
