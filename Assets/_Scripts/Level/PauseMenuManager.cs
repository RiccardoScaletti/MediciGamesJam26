
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private Button[] pauseMenuButtons;

    private int pauseMenuIndex = 0;
    private RobotControls controls;

    private void Awake()
    {
        controls = new RobotControls();
    }

    #region Controls Setup
    private void OnEnable()
    {
        Time.timeScale = 0f;
        pauseMenuButtons[0].Select();
        pauseMenuIndex = 0;

        controls.UIActions.SelectRight.performed += SelectNextChoice;
        controls.UIActions.SelectLeft.performed += SelectPreviousChoice;
        controls.UIActions.Confirm.performed += MakeChoice;

        controls.UIActions.Enable();
    }


    private void OnDisable()
    {
        controls.UIActions.SelectRight.performed -= SelectNextChoice;
        controls.UIActions.SelectLeft.performed -= SelectPreviousChoice;
        controls.UIActions.Confirm.performed -= MakeChoice;

        controls.UIActions.Disable();
    }

    private void SelectNextChoice(InputAction.CallbackContext context)
    {
        pauseMenuIndex++;
        if (pauseMenuIndex > pauseMenuButtons.Length) pauseMenuIndex = 0;
    }

    private void SelectPreviousChoice(InputAction.CallbackContext context)
    {
        pauseMenuIndex--;
        if (pauseMenuIndex < 0) pauseMenuIndex = 0;
    }

    private void MakeChoice(InputAction.CallbackContext context)
    {
        pauseMenuButtons[pauseMenuIndex].onClick.Invoke();
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void ResumeGame()
    {
        gameObject.SetActive(false); // Hide the menu UI
        Time.timeScale = 1f; // Reset time to normal speed
    }

    #endregion
}
