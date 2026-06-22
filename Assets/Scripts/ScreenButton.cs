using UnityEngine;

public class ScreenButton : MonoBehaviour, IFocusInteractable
{
    [SerializeField] private GameObject door;
    [SerializeField] private string hologramMessage;

    private FocusGlow glow;

    void Awake()
    {
        glow = GetComponent<FocusGlow>();
    }

    public void OnHoverEnter() => glow.Show();
    public void OnHoverExit() => glow.Hide();

    public void OnPress()
    {
        door.SetActive(false);
        if (!string.IsNullOrEmpty(hologramMessage))
            HologramDisplay.Instance.Show(hologramMessage);
    }

    public void OnDrag(Ray mouseRay) { }
    public void OnRelease() { }
}
