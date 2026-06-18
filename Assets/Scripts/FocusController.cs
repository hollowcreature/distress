using System.Collections;
using UnityEngine;

public class FocusController : MonoBehaviour
{
    public static FocusController Instance { get; private set; }

    [SerializeField] private Camera cam;
    [SerializeField] private MonoBehaviour[] controlsToDisable;
    [SerializeField] private float moveDuration = 0.6f;

    private bool isFocusing;
    private RepairTask current;
    private Vector3 homePos;
    private Quaternion homeRot;

    void Awake() => Instance = this;

    public void EnterFocus(RepairTask task)
    {
        if (isFocusing || task.IsRepaired)
            return;

        isFocusing = true;
        current = task;
        homePos = cam.transform.position;
        homeRot = cam.transform.rotation;

        SetControls(false);
        task.OnRepaired += HandleRepaired;

        StopAllCoroutines();
        StartCoroutine(MoveCameraTo(task.CameraAnchor.position, task.CameraAnchor.rotation, () => task.OnFocusEnter()));
    }

    public void ExitFocus()
    {
        if (!isFocusing)
            return;

        if (current != null)
        {
            current.OnFocusExit();
            current.OnRepaired -= HandleRepaired;
        }

        StopAllCoroutines();
        StartCoroutine(MoveCameraTo(homePos, homeRot, () =>
        {
            SetControls(true);
            isFocusing = false;
            current = null;
        }));
    }

    private void HandleRepaired(RepairTask t) => ExitFocus();

    void Update()
    {
        if (isFocusing && Input.GetKeyDown(KeyCode.Escape))
            ExitFocus();
    }

    private IEnumerator MoveCameraTo(Vector3 targetPos, Quaternion targetRot, System.Action onArrive)
    {
        Vector3 startPos = cam.transform.position;
        Quaternion startRot = cam.transform.rotation;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / moveDuration;
            float e = Mathf.SmoothStep(0f, 1f, t);
            cam.transform.position = Vector3.Lerp(startPos, targetPos, e);
            cam.transform.rotation = Quaternion.Slerp(startRot, targetRot, e);
            yield return null;
        }
        cam.transform.position = targetPos;
        cam.transform.rotation = targetRot;
        onArrive?.Invoke();
    }

    private void SetControls(bool on)
    {
        foreach (var c in controlsToDisable)
            if (c != null)
                c.enabled = on;
    }
}
