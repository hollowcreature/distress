using System.Collections;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [SerializeField] private Vector3 openOffset;
    [SerializeField] private AudioSource openSound;
    [SerializeField] private float duration;
    [SerializeField] private bool isUnlocked = false;
    private Vector3 closedPosition;
    private bool isOpen = false;
    private bool isMoving = false;

    void Awake()
    {
        closedPosition = transform.localPosition;
    }

    public void Unlock()
    {
        isUnlocked = true;
    }
    public void Open()
    {
        if (!isUnlocked || isMoving)
            return;

        if (openSound != null)
            openSound.Play();
        StartCoroutine(Slide(closedPosition + openOffset));
        isOpen = true;
    }

    public void Close()
    {
        if (!isUnlocked || isMoving)
            return;

        openSound.Play();
        StartCoroutine(Slide(closedPosition));
        isOpen = false;
    }

    public bool Toggle()
    {
        if (!isUnlocked)
        {
            ThoughtDisplay.Instance.Show("It's locked...");
            return false;
        }

        if (isOpen) Close();
        else Open();
        return true;
    }

    public void OpenBroken()
    {
        if (isMoving) return;
        openSound.Play();
        StartCoroutine(SlideBroken());
        isOpen = true;
    }

    private IEnumerator SlideBroken()
    {
        isMoving = true;
        Vector3 halfOpen = closedPosition + openOffset * 0.5f;
        Vector3 wobbleA = halfOpen + openOffset.normalized * 0.06f;
        Vector3 wobbleB = halfOpen - openOffset.normalized * 0.06f;

        yield return SlideStep(transform.localPosition, halfOpen, duration);

        while (true)
        {
            Vector3 target = halfOpen + openOffset.normalized * Random.Range(-0.08f, 0.08f);
            yield return SlideStep(transform.localPosition, target, Random.Range(0.08f, 0.25f));
        }
    }

    private IEnumerator SlideStep(Vector3 from, Vector3 to, float dur)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / dur;
            transform.localPosition = Vector3.Lerp(from, to, t);
            yield return null;
        }
    }

    private IEnumerator Slide(Vector3 target)
    {
        isMoving = true;
        Vector3 start = transform.localPosition;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.localPosition = Vector3.Lerp(start, target, t);
            yield return null;
        }
        transform.localPosition = target;
        isMoving = false;
    }
}
