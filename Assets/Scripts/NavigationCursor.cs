using System.Collections;
using UnityEngine;

public class NavigationCursor : MonoBehaviour, IFocusInteractable
{
    [SerializeField] private Transform cursorTransform;
    [SerializeField] private Transform targetPosition;
    [SerializeField] private float snapRadius = 0.05f;
    [SerializeField] private ComputerTerminal terminal;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private float fadeDuration;
    [SerializeField] private AudioSource courseSetSound;

    private FocusGlow glow;
    private Plane dragPlane;
    private Vector3 grabWorldPoint;
    private Vector3 grabOffset;
    private CanvasGroup canvasGroup;
    private bool completed;

    void Awake()
    {
        glow = GetComponent<FocusGlow>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnHoverEnter() => glow.Show();
    public void OnHoverExit() => glow.Hide();

    public void OnPress()
    {
        dragPlane = new Plane(-canvas.forward, canvas.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        dragPlane.Raycast(ray, out float enter);
        grabWorldPoint = ray.GetPoint(enter);
        grabOffset = canvas.InverseTransformPoint(cursorTransform.position)
                - canvas.InverseTransformPoint(grabWorldPoint);
    }

    public void OnDrag(Ray mouseRay)
    {
        if (completed)
            return;

        if (!dragPlane.Raycast(mouseRay, out float enter))
            return;

        Vector3 hitPoint = mouseRay.GetPoint(enter);
        Vector3 localHit = canvas.InverseTransformPoint(hitPoint);
        Vector3 newLocal = localHit + grabOffset;

        float halfW = canvas.rect.width * 0.5f;
        float halfH = canvas.rect.height * 0.5f;
        newLocal.x = Mathf.Clamp(newLocal.x, -halfW, halfW);
        newLocal.y = Mathf.Clamp(newLocal.y, -halfH, halfH);
        newLocal.z = 0f;

        cursorTransform.position = canvas.TransformPoint(newLocal);
    }

    public void OnRelease()
    {
        if (completed)
            return;

        if (Vector3.Distance(cursorTransform.position, targetPosition.position) <= snapRadius)
        {
            cursorTransform.position = targetPosition.position;
            completed = true;
            courseSetSound.Play();
            terminal.TryRepair();
        }
    }

    public IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
