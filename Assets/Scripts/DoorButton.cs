using System.Collections;
using UnityEngine;

public class DoorButton : MonoBehaviour, IInteractable
{
    [SerializeField] private SlidingDoor door;
    [SerializeField] private Transform buttonTop;
    [SerializeField] private Vector3 pressDirection;
    [SerializeField] private float pressDepth = 0.02f;
    [SerializeField] private float pressDuration = 0.1f;
    [SerializeField] private float returnDuration = 0.15f;

    private Vector3 restLocalPos;

    void Awake()
    {
        restLocalPos = buttonTop.localPosition;
    }

    public void Interact()
    {
        door.Toggle();
        StopAllCoroutines();
        StartCoroutine(PressAnim());
    }

    private IEnumerator PressAnim()
    {
        Vector3 pressedPos = restLocalPos - pressDirection * pressDepth;

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
