using UnityEngine;

public class KeyHandle : MonoBehaviour, IFocusInteractable
{
    [SerializeField] private KeySlotTask task;
    [SerializeField] private Transform pivot;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    private float angle;
    private FocusGlow glow;
    private Quaternion initialLocalRot;
    private float initialAngle;
    private Vector3 grabWorldPoint;
    private Plane grabPlane;

    void Awake()
    {
        glow = GetComponent<FocusGlow>();
        initialLocalRot = pivot.localRotation;
        angle = 0f;
    }

    public void OnHoverEnter() => glow.Show();
    public void OnHoverExit() => glow.Hide();

    public void OnPress()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        grabWorldPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : pivot.position;
        grabPlane = new Plane(-Camera.main.transform.forward, grabWorldPoint);
        initialAngle = angle;
    }

    public void OnDrag(Ray mouseRay)
    {
        if (!grabPlane.Raycast(mouseRay, out float enter)) return;

        Vector3 currentHit = mouseRay.GetPoint(enter);
        Vector3 worldAxis = pivot.TransformDirection(rotationAxis.normalized);
        Vector3 fromVec = Vector3.ProjectOnPlane(grabWorldPoint - pivot.position, worldAxis).normalized;
        Vector3 toVec = Vector3.ProjectOnPlane(currentHit - pivot.position, worldAxis).normalized;

        if (fromVec == Vector3.zero || toVec == Vector3.zero) return;

        float angleDelta = Vector3.SignedAngle(fromVec, toVec, worldAxis);
        angle = Mathf.Clamp(initialAngle + angleDelta, 0f, 90f);
        pivot.localRotation = initialLocalRot * Quaternion.AngleAxis(angle, rotationAxis);
    }

    public void OnRelease()
    {
        if (angle >= 90f)
            task.TryRepair();
    }
}
