using System.Collections;
using UnityEngine;

[RequireComponent(typeof(FocusGlow))]
public class ConfirmButton : MonoBehaviour, IFocusInteractable
{
    [SerializeField] private Transform buttonTop;
    [SerializeField] private float pressDepth = 0.02f;
    [SerializeField] private float pressDuration = 0.1f;
    [SerializeField] private float returnDuration = 0.15f;

    [SerializeField] private RepairTask requiredTask;
    [SerializeField] private RepairTask generatorTask;
    [SerializeField] private RepairTask sensorTask;
    [SerializeField] private string blockedThought;

    private FocusGlow glow;
    private RepairTask task;
    private Vector3 restLocalPos;

    void Awake()
    {
        glow = GetComponent<FocusGlow>();
        task = GetComponentInParent<RepairTask>();
        restLocalPos = buttonTop.localPosition;
    }

    public void OnHoverEnter() => glow.Show();
    public void OnHoverExit() => glow.Hide();

    public void OnPress()
    {
        if (!generatorTask.IsRepaired)
            ThoughtDisplay.Instance.Show("I need to get the power on first");
        else if (sensorTask != null && !sensorTask.IsRepaired)
            ThoughtDisplay.Instance.Show("I need to fix the sensors");
        else if (requiredTask != null && !requiredTask.IsRepaired)
            ThoughtDisplay.Instance.Show(blockedThought);

        StopAllCoroutines();
        StartCoroutine(PressAnim());
        task.TryRepair();
    }

    public void OnDrag(Ray mouseRay) { }
    public void OnRelease() { }

    private IEnumerator PressAnim()
    {
        Vector3 pressedPos = restLocalPos - Vector3.up * pressDepth;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / pressDuration;
            buttonTop.localPosition = Vector3.Lerp(restLocalPos, pressedPos, t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / returnDuration;
            buttonTop.localPosition = Vector3.Lerp(pressedPos, restLocalPos, t);
            yield return null;
        }

        buttonTop.localPosition = restLocalPos;
    }
}
