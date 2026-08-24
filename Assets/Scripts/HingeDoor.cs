using System.Collections;
using UnityEngine;

public class HingeDoor : MonoBehaviour
{
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private AudioSource openSound;

    private bool isOpen = false;
    private bool isMoving = false;
    private bool locked = false;
    private Quaternion closedRot;

    void Awake()
    {
        closedRot = transform.localRotation;
    }

    [SerializeField] private Collider hingeCollider;

    public void Open()
    {
        if (isOpen || isMoving || locked) return;
        isOpen = true;
        openSound.Play();
        hingeCollider.enabled = false;
        StartCoroutine(Rotate(Quaternion.Euler(0, openAngle, 0)));
    }

    public void Close()
    {
        if (!isOpen || isMoving) return;
        openSound.Play();
        isOpen = false;
        StartCoroutine(Rotate(closedRot));
    }

    public void CloseAndLock()
    {
        locked = true;
        Close();
    }

    public void Toggle()
    {
        if (isOpen) Close();
        else Open();
    }

    private IEnumerator Rotate(Quaternion target)
    {
        isMoving = true;
        Quaternion start = transform.localRotation;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.localRotation = Quaternion.Slerp(start, target, t);
            yield return null;
        }
        transform.localRotation = target;
        isMoving = false;
    }
}
