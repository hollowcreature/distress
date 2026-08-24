using UnityEngine;

public class NecklacePickup : MonoBehaviour, IInteractable
{
    [SerializeField] private string thought = "A small reminder of why I'm out here.";

    public static bool Found { get; private set; }

    public void Interact()
    {
        Found = true;
        ThoughtDisplay.Instance.Show(thought);
        gameObject.SetActive(false);
    }
}
