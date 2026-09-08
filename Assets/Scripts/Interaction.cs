using UnityEngine;

public class Interaction : MonoBehaviour
{
    public float interact_range = 3f;
    public float interact_range_hysteresis = 1f;
    public float loseCastRadius = 0.15f;
    public GameObject interact_prompt;
    IInteractable current_interactable = null;

    void LateUpdate()
    {
        if (FocusController.Instance.IsFocusing)
        {
            interact_prompt.SetActive(false);
            return;
        }

        float range = current_interactable != null ? interact_range + interact_range_hysteresis : interact_range;

        if (current_interactable == null)
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, range))
                current_interactable = hit.collider.GetComponent<IInteractable>();
        }
        else
        {
            bool stillLocked = Physics.SphereCast(transform.position, loseCastRadius, transform.forward, out RaycastHit sphereHit, range)
                && ReferenceEquals(sphereHit.collider.GetComponent<IInteractable>(), current_interactable);

            if (!stillLocked)
            {
                // Wide cast may have clipped nearby geometry instead of the target. Retry with a thin raycast before giving up.
                if (Physics.Raycast(transform.position, transform.forward, out RaycastHit thinHit, range))
                    current_interactable = thinHit.collider.GetComponent<IInteractable>();
                else
                    current_interactable = null;
            }
        }

        if (current_interactable is RepairTask task && task.IsRepaired && !task.AlwaysInteractable)
            current_interactable = null;

        interact_prompt.SetActive(current_interactable != null);

        if (Input.GetKeyDown(KeyCode.E) && current_interactable != null)
            current_interactable.Interact();
    }
}
