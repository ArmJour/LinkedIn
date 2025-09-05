using UnityEngine;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    private bool isPaused = false;

    public void togglePause()
    {
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
