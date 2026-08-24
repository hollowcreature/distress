using UnityEngine;

public class KeyPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject KeyObject;
    [SerializeField] private AudioSource pickupSound;
    public bool hasKey = false;
    public void Interact()
    {
        pickupSound.Play();
        KeyObject.SetActive(false);
        GetComponent<Collider>().enabled = false;
        hasKey = true;
        ObjectiveDisplay.Instance.Clear();
    }
}
