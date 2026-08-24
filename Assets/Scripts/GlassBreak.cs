using UnityEngine;

public class GlassBreak : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject brokenGlass;
    [SerializeField] private AudioSource breakSound;
    [SerializeField] private Collider keyCollider;

    public bool canTake = false;

    public void Interact()
    {
        if (!canTake)
        {
            ThoughtDisplay.Instance.Show("I don't need that...");
            return;
        }

        breakSound.Play();
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        brokenGlass.SetActive(true);
        keyCollider.enabled = true;
    }
}
