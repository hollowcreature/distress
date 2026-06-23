using UnityEngine;

public class KeyPickup : MonoBehaviour, IInteractable
{
    public bool hasKey = false;
    public void Interact()
    {
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        hasKey = true;
    }
}
