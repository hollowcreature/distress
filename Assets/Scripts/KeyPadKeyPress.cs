using System.Collections;
using UnityEngine;

[RequireComponent(typeof(FocusGlow))]
public class KeyPadKeyPress : MonoBehaviour, IFocusInteractable
{
    [SerializeField] private int keyValue;
    [SerializeField] KeypadTask task;
    [SerializeField] private Transform buttonTop;
    [SerializeField] private Vector3 pressDirection = Vector3.up;
    [SerializeField] private float pressDepth = 0.02f;
    [SerializeField] private float pressDuration = 0.1f;
    [SerializeField] private float returnDuration = 0.15f;

    private FocusGlow glow;
    private Vector3 restLocalPos;

    void Awake()
    {
        glow = GetComponent<FocusGlow>();
        restLocalPos = buttonTop.localPosition;
    }

    public void OnHoverEnter() => glow.Show();
    public void OnHoverExit() => glow.Hide();

    public void OnPress()
    {

        StopAllCoroutines();
        StartCoroutine(PressAnim());

        task.AppendChar((char)('0' + keyValue));
    }

    public void OnDrag(Ray mouseRay) { }
    public void OnRelease() { }

    private IEnumerator PressAnim()
    {
        Vector3 pressedPos = restLocalPos + pressDirection.normalized * pressDepth;

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
