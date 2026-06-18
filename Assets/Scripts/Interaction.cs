using UnityEngine;

public class Interaction : MonoBehaviour
{
    public float interact_range = 3f;
    public GameObject interact_prompt;
    IInteractable current_interactable = null;

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interact_range))
            current_interactable = hit.collider.GetComponent<IInteractable>();
        else
            current_interactable = null;

        interact_prompt.SetActive(current_interactable != null);

        if (Input.GetKeyDown(KeyCode.E) && current_interactable != null)
            current_interactable.Interact();
    }
}
