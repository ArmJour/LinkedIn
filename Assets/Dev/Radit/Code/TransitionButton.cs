using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TransitionButton : MonoBehaviour
{
    public string SceneName;
    protected Button btn;

    protected virtual void Awake()
    {
        btn = GetComponent<Button>();
        if (btn == null) return;

        btn.onClick.AddListener(OnClick);
    }

    protected virtual void OnClick()
    {
        if (TransitionScript.instance != null)
        {
            TransitionScript.instance.TransitionTo(SceneName);
        }
        else
        {
            Debug.Log("TransitionScript not found");
        }
    }

    protected virtual void OnDestroy()
    {
        if (btn != null) btn.onClick.RemoveAllListeners();
    }
}
