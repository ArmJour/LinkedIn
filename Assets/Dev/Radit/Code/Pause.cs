using UnityEngine;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject winPanel;   
    [SerializeField] private GameObject losePanel;
    private bool isPaused = false;

    public void togglePause()
    {
        // Prevent pause if win or lose panel is active
        if ((winPanel != null && winPanel.activeSelf) || (losePanel != null && losePanel.activeSelf))
        {
            Debug.Log("Cannot pause: Win or Lose panel is active.");
            return;
        }

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        pauseMenu.SetActive(isPaused);
        Debug.Log("Toggle Pause Method");
    }

    #region Input System
    public void OnPause()
    {
        togglePause();
    }
    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
            togglePause();
    }
    #endregion
}
