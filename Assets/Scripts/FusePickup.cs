using UnityEngine;

public class FusePickup : MonoBehaviour, IInteractable
{
    [SerializeField] private PanelGenerator panel;

    public void Interact()
    {
        FuseSlot.PlayerHasFuse = true;
        ObjectiveDisplay.Instance.Clear();
        gameObject.SetActive(false);
    }
}
