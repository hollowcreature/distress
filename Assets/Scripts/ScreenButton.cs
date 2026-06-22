using UnityEngine;

public class ScreenButton : MonoBehaviour, IFocusInteractable
{
    [SerializeField] private GameObject door;

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
    }

    public void OnDrag(Ray mouseRay) { }
    public void OnRelease() { }
}
