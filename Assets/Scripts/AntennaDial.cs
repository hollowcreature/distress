using UnityEngine;

public class AntennaDial : MonoBehaviour, IFocusInteractable
{
    [SerializeField] private Transform pivot;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private float targetAngle;
    [SerializeField] private float tolerance = 15f;

    public float Angle { get; private set; }
    public float TargetAngle => targetAngle;
    public bool IsAligned => Mathf.Abs(Mathf.DeltaAngle(Angle, targetAngle)) <= tolerance;

    private FocusGlow glow;
    private Quaternion initialLocalRot;
    private float initialAngle;
    private Vector3 grabWorldPoint;
    private Plane grabPlane;

    void Awake()
    {
        glow = GetComponent<FocusGlow>();
        initialLocalRot = pivot.localRotation;
        Angle = 0f;
    }

    public void OnHoverEnter() => glow.Show();
    public void OnHoverExit() => glow.Hide();

    public void OnPress()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        grabWorldPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : pivot.position;
        grabPlane = new Plane(-Camera.main.transform.forward, grabWorldPoint);
        initialAngle = Angle;
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
        Angle = initialAngle + angleDelta;
        pivot.localRotation = initialLocalRot * Quaternion.AngleAxis(Angle, rotationAxis);
    }

    public void OnRelease() { }
}
