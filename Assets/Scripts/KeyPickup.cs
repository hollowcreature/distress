using UnityEngine;

public class KeyPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject KeyObject;
    public bool hasKey = false;
    public void Interact()
    {
        KeyObject.SetActive(false);
        GetComponent<Collider>().enabled = false;
        hasKey = true;
        ObjectiveDisplay.Instance.Clear();
    }
}
