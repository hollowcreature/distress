using UnityEngine;

public interface IFocusInteractable
{
    void OnHoverEnter();
    void OnHoverExit();
    void OnPress();
    void OnDrag(Ray mouseRay);
    void OnRelease();
}
