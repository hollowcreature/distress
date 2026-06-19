using System.Collections;
using UnityEngine;

public class FocusController : MonoBehaviour
{
    public static FocusController Instance { get; private set; }

    [SerializeField] private Camera cam;
    [SerializeField] private MonoBehaviour[] controlsToDisable;
    [SerializeField] private float moveDuration = 0.6f;
    [SerializeField] private Transform homeAnchor;
    [SerializeField] private MeshRenderer playerMesh;

    private bool isFocusing;
    private bool isAtAnchor;
    private RepairTask current;
    private IFocusInteractable hovered;
    private IFocusInteractable pressed;

    void Awake() => Instance = this;

    public void EnterFocus(RepairTask task)
    {
        if (isFocusing || task.IsRepaired)
            return;

        isFocusing = true;
        current = task;
        playerMesh.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SetControls(false);
        task.OnRepaired += HandleRepaired;

        StopAllCoroutines();
        StartCoroutine(MoveCameraTo(task.CameraAnchor.position, task.CameraAnchor.rotation, () =>
        {
            isAtAnchor = true;
            task.OnFocusEnter();
        }));
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

        isAtAnchor = false;
        StopAllCoroutines();
        StartCoroutine(MoveCameraTo(homeAnchor.position, homeAnchor.rotation, () =>
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            SetControls(true);
            isFocusing = false;
            playerMesh.enabled = true;
            current = null;
        }));
    }

    private void HandleRepaired(RepairTask t) => ExitFocus();

    void Update()
    {
        if (isFocusing && Input.GetKeyDown(KeyCode.Escape))
            ExitFocus();

        if (isAtAnchor)
            HandleFocusInput();
    }

    private void HandleFocusInput()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!Input.GetMouseButton(0))
        {
            IFocusInteractable hit = null;
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 10f))
                hit = hitInfo.collider.GetComponent<IFocusInteractable>();

            if (hit != hovered)
            {
                hovered?.OnHoverExit();
                hovered = hit;
                hovered?.OnHoverEnter();
            }
        }

        if (Input.GetMouseButtonDown(0) && hovered != null)
        {
            pressed = hovered;
            pressed.OnPress();
        }

        if (Input.GetMouseButton(0) && pressed != null)
            pressed.OnDrag(ray);

        if (Input.GetMouseButtonUp(0) && pressed != null)
        {
            pressed.OnRelease();
            pressed = null;
        }
    }

    void LateUpdate()
    {
        if (isAtAnchor && current != null)
            cam.transform.SetPositionAndRotation(current.CameraAnchor.position, current.CameraAnchor.rotation);
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
