using UnityEngine;

[RequireComponent(typeof(FocusGlow))]
public class PanelSlider : MonoBehaviour, IFocusInteractable
{
    [SerializeField] private int targetPosition;
    [SerializeField] private Light statusLight;
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color incorrectColor = Color.red;
    [SerializeField] private Transform sliderTransform;
    [SerializeField] private Vector3 slideDirection = Vector3.up;
    [SerializeField] private float slideStep = 0.05f;

    public int CurrentPosition { get; private set; }
    public bool IsCorrect => CurrentPosition == targetPosition;

    private FocusGlow glow;
    private Vector3 initialLocalPos;
    private Plane grabPlane;
    private Vector3 grabWorldPoint;
    private float initialDragOffset;
    private float dragOffset;

    void Awake()
    {
        glow = GetComponent<FocusGlow>();
        if (sliderTransform != null)
            initialLocalPos = sliderTransform.localPosition;
        if (statusLight != null)
            statusLight.enabled = false;
    }

    public void Activate()
    {
        if (statusLight != null)
            statusLight.enabled = true;
        UpdateVisuals();
    }

    public void OnHoverEnter() => glow.Show();
    public void OnHoverExit() => glow.Hide();

    public void OnPress()
    {
        initialDragOffset = CurrentPosition * slideStep;
        dragOffset = initialDragOffset;

        grabPlane = new Plane(-Camera.main.transform.forward, sliderTransform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        grabPlane.Raycast(ray, out float enter);
        grabWorldPoint = ray.GetPoint(enter);
    }

    public void OnDrag(Ray mouseRay)
    {
        if (!grabPlane.Raycast(mouseRay, out float enter)) return;

        Vector3 worldHit = mouseRay.GetPoint(enter);
        Vector3 worldDelta = worldHit - grabWorldPoint;

        Transform parent = sliderTransform.parent;
        Vector3 localDelta = parent != null ? parent.InverseTransformVector(worldDelta) : worldDelta;
        float projected = Vector3.Dot(localDelta, slideDirection.normalized);

        dragOffset = Mathf.Clamp(initialDragOffset + projected, 0f, 3f * slideStep);
        sliderTransform.localPosition = initialLocalPos + slideDirection.normalized * dragOffset;
    }

    public void OnRelease()
    {
        CurrentPosition = Mathf.Clamp(Mathf.RoundToInt(dragOffset / slideStep), 0, 3);
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (sliderTransform != null)
            sliderTransform.localPosition = initialLocalPos + slideDirection.normalized * (CurrentPosition * slideStep);

        if (statusLight != null)
            statusLight.color = IsCorrect ? correctColor : incorrectColor;
    }
}
