using UnityEngine;

[RequireComponent(typeof(FocusGlow))]
public class ConfirmLever : MonoBehaviour, IFocusInteractable
{
    [SerializeField] private Transform pivot;
    [SerializeField] private Vector3 rotationAxis = Vector3.right;
    [SerializeField] private float minAngle = 0f;
    [SerializeField] private float maxAngle = 90f;
    [SerializeField] private float activationThreshold = 0.7f;
    [SerializeField] private RepairTask task;

    private FocusGlow glow;
    private Quaternion initialLocalRot;
    private float currentAngle;
    private float initialAngle;
    private Vector3 grabWorldPoint;
    private Plane grabPlane;

    void Awake()
    {
        glow = GetComponent<FocusGlow>();
        initialLocalRot = pivot.localRotation;
        currentAngle = 0f;
    }

    public void OnHoverEnter() => glow.Show();
    public void OnHoverExit() => glow.Hide();

    public void OnPress()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        grabWorldPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : pivot.position;
        grabPlane = new Plane(-Camera.main.transform.forward, grabWorldPoint);
        initialAngle = currentAngle;
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
        currentAngle = Mathf.Clamp(initialAngle + angleDelta, minAngle, maxAngle);
        pivot.localRotation = initialLocalRot * Quaternion.AngleAxis(currentAngle, rotationAxis);
    }

    public void OnRelease()
    {
        float activationAngle = minAngle + (maxAngle - minAngle) * activationThreshold;
        if (currentAngle >= activationAngle)
            task.TryRepair();
    }
}
