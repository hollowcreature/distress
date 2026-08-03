using System.Collections;
using UnityEngine;

public class ModulePanel : MonoBehaviour, IFocusInteractable
{
    [SerializeField] private ModuleTask module;
    [SerializeField] private Transform pivot;
    [SerializeField] private Vector3 rotationAxis = Vector3.right;
    [SerializeField] private float minAngle = 0f;
    [SerializeField] private float maxAngle = 120f;
    [SerializeField] private float unlockThreshold = 0.5f;
    [SerializeField] private float activationThreshold = 0.95f;
    [SerializeField] private float springBackDuration = 1f;
    [SerializeField] public float closeDuration = 2f;
    [SerializeField] private float resistanceMultiplier = 0.4f;

    private FocusGlow glow;
    private Quaternion initialLocalRot;
    private float currentAngle;
    private float initialAngle;
    private Vector3 grabWorldPoint;
    private Plane grabPlane;
    private bool unlocked = false;
    private bool activated = false;

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
        if (activated)
            return;

        StopAllCoroutines();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        grabWorldPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : pivot.position;
        grabPlane = new Plane(-Camera.main.transform.forward, grabWorldPoint);
        initialAngle = currentAngle;
    }

    public void OnDrag(Ray mouseRay)
    {
        if (activated)
            return;

        if (!grabPlane.Raycast(mouseRay, out float enter)) return;

        float resistance = unlocked ? 1f : resistanceMultiplier;

        Vector3 currentHit = mouseRay.GetPoint(enter);
        Vector3 worldAxis = pivot.TransformDirection(rotationAxis.normalized);
        Vector3 fromVec = Vector3.ProjectOnPlane(grabWorldPoint - pivot.position, worldAxis).normalized;
        Vector3 toVec = Vector3.ProjectOnPlane(currentHit - pivot.position, worldAxis).normalized;

        if (fromVec == Vector3.zero || toVec == Vector3.zero) return;

        float angleDelta = Vector3.SignedAngle(fromVec, toVec, worldAxis);
        currentAngle = Mathf.Clamp(initialAngle + angleDelta * resistance, minAngle, maxAngle);
        pivot.localRotation = initialLocalRot * Quaternion.AngleAxis(currentAngle, rotationAxis);

        if (!unlocked && currentAngle >= maxAngle * unlockThreshold)
        {
            unlocked = true;
            module.OnPanelOpened();
        }

        if (!activated && currentAngle >= maxAngle * activationThreshold)
            activated = true;
    }

    public void OnRelease()
    {
        if (!unlocked)
            StartCoroutine(SpringBack(springBackDuration));
    }

    public IEnumerator SpringBack(float duration)
    {
        float startAngle = currentAngle;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            currentAngle = Mathf.Lerp(startAngle, 0f, t);
            pivot.localRotation = initialLocalRot * Quaternion.AngleAxis(currentAngle, rotationAxis);
            yield return null;
        }

        currentAngle = 0f;
        pivot.localRotation = initialLocalRot;
    }

}
