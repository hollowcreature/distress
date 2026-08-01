using UnityEngine;

public class SecretTrigger : MonoBehaviour
{
    [SerializeField] private SlidingDoor door;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.Open();
            this.gameObject.SetActive(false);
        }
    }
}
