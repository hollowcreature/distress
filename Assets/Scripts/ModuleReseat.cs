using UnityEngine;

public class ModuleReseat : MonoBehaviour, IFocusInteractable
{
    [SerializeField] private ModuleTask module;
    [SerializeField] private Transform moduleTransform;
    [SerializeField] private Vector3 pushAxis = Vector3.forward;
    [SerializeField] private float pushDistance = 0.3f;
    [SerializeField] private float snapThreshhold = 0.8f;

    private FocusGlow glow;
    private bool completed;
    private Vector3 initialLocalPos;
    private Vector3 grabWorldPoint;
    private Plane grabPlane;
    private float currentPush;
    private float pushAtPress;

    void Awake()
    {
        glow = GetComponent<FocusGlow>();
        initialLocalPos = moduleTransform.localPosition;
        currentPush = 0f;
    }

    public void OnHoverEnter() => glow.Show();
    public void OnHoverExit() => glow.Hide();

    public void OnPress()
    {
        if (completed)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        grabWorldPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : moduleTransform.position;
        grabPlane = new Plane(-Camera.main.transform.forward, grabWorldPoint);

        pushAtPress = currentPush;
    }

    public void OnDrag(Ray mouseRay)
    {
        if (completed)
            return;

        if (!grabPlane.Raycast(mouseRay, out float enter)) return;

        Vector3 currentHit = mouseRay.GetPoint(enter);
        Vector3 worldDelta = currentHit - grabWorldPoint;
        Vector3 localDelta = moduleTransform.InverseTransformVector(worldDelta);
        float pushDelta = Vector3.Dot(localDelta, pushAxis.normalized);

        currentPush = Mathf.Clamp(pushAtPress + pushDelta, 0f, pushDistance);
        moduleTransform.localPosition = initialLocalPos + pushAxis.normalized * currentPush;
    }

    public void OnRelease()
    {
        if (!completed && currentPush >= snapThreshhold * pushDistance)
        {
            currentPush = pushDistance;
            moduleTransform.localPosition = initialLocalPos + pushAxis.normalized * currentPush;
            completed = true;
            module.OnModuleReseated();
        }
    }
}
