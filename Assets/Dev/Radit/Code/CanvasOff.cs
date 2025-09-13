using UnityEngine;

public class CanvasOff : MonoBehaviour
{
    [SerializeField] private GameObject Panel;

    public void onclick()
    {
        Panel.SetActive(false);
        Time.timeScale = 1;
    }
}
