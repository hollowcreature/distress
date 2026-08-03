using UnityEngine;

public class HingeDoorInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private HingeDoor door;

    public void Interact() => door.Open();
}
