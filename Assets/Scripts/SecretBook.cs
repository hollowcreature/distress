using System.Collections;
using UnityEngine;

[ExecuteAlways]
public class SecretBook : RepairTask
{
    [SerializeField] private Transform openPoint;
    [SerializeField] private float moveDuration = 1.2f;

    [SerializeField] private Transform leftPage;
    [SerializeField] private Transform rightPage;
    [SerializeField] private Vector3 leftPageOpenEuler = new Vector3(0f, 0f, -140f);
    [SerializeField] private Vector3 rightPageOpenEuler = new Vector3(0f, 0f, 140f);
    [SerializeField] private float pageOpenDuration = 0.6f;

    [SerializeField] private CanvasGroup bookTextGroup;
    [SerializeField] private float textFadeDuration = 1.5f;

    private Collider bookCol;
    private Vector3 closedPosition;
    private Quaternion closedRotation;
    private Quaternion leftClosedRot;
    private Quaternion leftOpenRot;
    private Quaternion rightClosedRot;
    private Quaternion rightOpenRot;
    private bool busy = false;

    public override bool AlwaysInteractable => true;

    protected override void Awake()
    {
        base.Awake();
        bookCol = GetComponent<Collider>();
        bookTextGroup.alpha = 0f;
    }

    void OnEnable()
    {
        closedPosition = transform.position;
        closedRotation = transform.rotation;

        leftClosedRot = leftPage.localRotation;
        leftOpenRot = leftClosedRot * Quaternion.Euler(leftPageOpenEuler);
        rightClosedRot = rightPage.localRotation;
        rightOpenRot = rightClosedRot * Quaternion.Euler(rightPageOpenEuler);
    }

    protected override bool AttemptStep() => false;

    public override void Interact()
    {
        if (busy) return;
        busy = true;
        bookCol.enabled = false;
        StartCoroutine(OpenSequence());
    }

    private IEnumerator OpenSequence()
    {
        FocusController.Instance.EnterFocus(this);
        yield return StartCoroutine(OpenAnimation());
    }

    private IEnumerator OpenAnimation()
    {
        yield return StartCoroutine(MoveBook(openPoint.position, openPoint.rotation, moveDuration));
        StartCoroutine(RotatePage(leftPage, leftClosedRot, leftOpenRot, pageOpenDuration));
        yield return StartCoroutine(RotatePage(rightPage, rightClosedRot, rightOpenRot, pageOpenDuration));
        StartCoroutine(FadeCanvas(bookTextGroup, 0f, 1f, textFadeDuration));
    }

    [ContextMenu("Test: Open Animation")]
    private void TestOpenAnimation()
    {
        StopAllCoroutines();
        StartCoroutine(OpenAnimation());
    }

    [ContextMenu("Test: Close Animation")]
    private void TestCloseAnimation()
    {
        StopAllCoroutines();
        StartCoroutine(CloseAnimation());
    }

    public override void OnFocusEnter()
    {
        bookTextGroup.gameObject.SetActive(true);
        bookTextGroup.alpha = 0f;
    }

    public override void OnFocusExit()
    {
        StopAllCoroutines();
        bookTextGroup.alpha = 0f;
        bookTextGroup.gameObject.SetActive(false);
        StartCoroutine(CloseSequence());
    }

    private IEnumerator CloseSequence()
    {
        yield return StartCoroutine(CloseAnimation());
        bookCol.enabled = true;
        busy = false;
    }

    private IEnumerator CloseAnimation()
    {
        StartCoroutine(RotatePage(leftPage, leftPage.localRotation, leftClosedRot, pageOpenDuration));
        yield return StartCoroutine(RotatePage(rightPage, rightPage.localRotation, rightClosedRot, pageOpenDuration));
        yield return StartCoroutine(MoveBook(closedPosition, closedRotation, moveDuration));
    }

    private IEnumerator MoveBook(Vector3 targetPos, Quaternion targetRot, float duration)
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float e = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t));
            transform.SetPositionAndRotation(Vector3.Lerp(startPos, targetPos, e), Quaternion.Slerp(startRot, targetRot, e));
            yield return null;
        }
        transform.SetPositionAndRotation(targetPos, targetRot);
    }

    private IEnumerator RotatePage(Transform page, Quaternion start, Quaternion target, float duration)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            page.localRotation = Quaternion.Slerp(start, target, Mathf.Clamp01(t));
            yield return null;
        }
        page.localRotation = target;
    }

    private IEnumerator FadeCanvas(CanvasGroup group, float from, float to, float duration)
    {
        group.alpha = from;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            group.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }
        group.alpha = to;
    }
}
