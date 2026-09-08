using System.Collections;
using UnityEngine;

[ExecuteAlways]
public class ShutterController : MonoBehaviour
{
    [System.Serializable]
    public struct ShutterPanel
    {
        public Transform panel;
        public Vector3 openOffset;
        public float duration;
        public float delayBefore;
    }

    [SerializeField] private ShutterPanel[] panels;

    private Vector3[] closedPositions;
    private bool[] positionsCaptured;
    private Coroutine activeCoroutine;

    void OnEnable()
    {
        CaptureClosedPositions();
    }

    [ContextMenu("Capture Closed Positions")]
    public void CaptureClosedPositions()
    {
        closedPositions = new Vector3[panels.Length];
        positionsCaptured = new bool[panels.Length];
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i].panel != null)
            {
                closedPositions[i] = panels[i].panel.localPosition;
                positionsCaptured[i] = true;
            }
        }
    }

    [ContextMenu("Open")]
    public void Open()
    {
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(OpenSequence());
    }

    [ContextMenu("Close")]
    public void Close()
    {
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(CloseSequence());
    }

    private IEnumerator OpenSequence()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i].panel == null) continue;
            if (!positionsCaptured[i])
            {
                closedPositions[i] = panels[i].panel.localPosition;
                positionsCaptured[i] = true;
            }
            yield return new WaitForSeconds(panels[i].delayBefore);
            StartCoroutine(SlidePanel(panels[i].panel, panels[i].panel.localPosition, closedPositions[i] + panels[i].openOffset, panels[i].duration));
        }
    }

    private IEnumerator CloseSequence()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i].panel == null || !positionsCaptured[i]) continue;
            yield return new WaitForSeconds(panels[i].delayBefore);
            StartCoroutine(SlidePanel(panels[i].panel, panels[i].panel.localPosition, closedPositions[i], panels[i].duration));
        }
    }

    private IEnumerator SlidePanel(Transform panel, Vector3 start, Vector3 target, float duration)
    {
        if (duration <= 0f) { panel.localPosition = target; yield break; }
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            panel.localPosition = Vector3.Lerp(start, target, Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t)));
            yield return null;
        }
        panel.localPosition = target;
    }
}
