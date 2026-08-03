using UnityEngine;

public class HingeDoorCloseTrigger : MonoBehaviour
{
    [SerializeField] private HingeDoor door;
    [SerializeField] private Collider doorBlock;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.CloseAndLock();
            doorBlock.enabled = true;
            gameObject.SetActive(false);
        }
    }
}
