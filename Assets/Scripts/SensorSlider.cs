using UnityEngine;

public class SensorSlider : MonoBehaviour, IFocusInteractable
{
    [SerializeField] private float slideDistance = 0.15f;
    [SerializeField] private float targetNormalized = 0.5f;
    [SerializeField] private float tolerance = 0.1f;
    [SerializeField] private Transform sliderTransform;
    [SerializeField] private Vector3 slideDirection = Vector3.up;
    [SerializeField] private Light indicatorLight;
    [SerializeField] private float minBlinkInterval = 0.1f;
    [SerializeField] private float maxBlinkInterval = 0.8f;
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color incorrectColor = Color.red;
    [SerializeField] private AudioSource beepSource;

    private FocusGlow glow;
    private Vector3 initialLocalPos;
    private Plane grabPlane;
    private Vector3 grabWorldPoint;
    private float initialDragOffset;
    private float dragOffset;
    private float blinkTimer;
    private bool lightOn;

    public bool IsCorrect => Mathf.Abs(dragOffset / slideDistance - targetNormalized) <= tolerance;

    void Awake()
    {
        glow = GetComponent<FocusGlow>();
        initialLocalPos = sliderTransform.localPosition;
    }

    void Update()
    {
        if (indicatorLight == null)
            return;

        if (IsCorrect)
        {
            indicatorLight.color = correctColor;
            indicatorLight.enabled = true;
            return;
        }

        indicatorLight.color = incorrectColor;

        float error = Mathf.Clamp01(Mathf.Abs(dragOffset / slideDistance - targetNormalized) / 0.5f);
        float interval = Mathf.Lerp(maxBlinkInterval, minBlinkInterval, error);

        blinkTimer += Time.deltaTime;
        if (blinkTimer >= interval)
        {
            blinkTimer = 0f;
            lightOn = !lightOn;
            indicatorLight.enabled = lightOn;
            if (lightOn)
                beepSource.Play();
        }
    }

    public void OnHoverEnter() => glow.Show();
    public void OnHoverExit() => glow.Hide();

    public void OnPress()
    {
        initialDragOffset = dragOffset;

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

        dragOffset = Mathf.Clamp(initialDragOffset + projected, 0f, slideDistance);
        sliderTransform.localPosition = initialLocalPos + slideDirection.normalized * dragOffset;
    }

    public void OnRelease() { }
}
