using UnityEngine;

public class FusePickup : MonoBehaviour, IInteractable
{
    [SerializeField] private PanelGenerator panel;

    public void Interact()
    {
        FuseSlot.PlayerHasFuse = true;
        gameObject.SetActive(false);
    }
}
