using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ScreenButton : MonoBehaviour, IFocusInteractable
{
    [SerializeField] private SlidingDoor door;
    [SerializeField] private GlassBreak glass;
    [SerializeField] private string hologramMessage;
    [SerializeField][TextArea] private string endgameMessage;
    [SerializeField] private float fadeDuration;
    [SerializeField] private bool doorButton;
    [SerializeField] private EndingSequence endingSequence;
    [SerializeField] private AudioSource pressSound;

    private FocusGlow glow;
    private CanvasGroup canvasGroup;
    private Renderer buttonRenderer;
    private Material buttonMat;
    public bool escalatedPrivilege;
    private bool endingStarted = false;

    void Awake()
    {
        glow = GetComponent<FocusGlow>();
        canvasGroup = GetComponent<CanvasGroup>();
        buttonRenderer = GetComponent<Renderer>();
        buttonMat = buttonRenderer.material;
    }

    public void OnHoverEnter() => glow.Show();
    public void OnHoverExit() => glow.Hide();

    public void OnPress()
    {
        pressSound.Play();

        if (doorButton)
        {
            door.Unlock();
            if (!string.IsNullOrEmpty(hologramMessage))
                HologramDisplay.Instance.Show(hologramMessage);

            StartCoroutine(FadeOut());
        }
        else
        {
            if (glass.canTake && !escalatedPrivilege)
                return;

            if (!escalatedPrivilege)
            {
                glass.canTake = true;
                HologramDisplay.Instance.Show(hologramMessage);
                ObjectiveDisplay.Instance.Show("New Objective: Get the emergency key");
            }
            else
            {
                if (!endingStarted)
                {
                    endingStarted = true;
                    HologramDisplay.Instance.Show(endgameMessage);
                    StartCoroutine(FadeOut());
                    endingSequence.Begin();
                }
            }
        }
    }

    public void OnDrag(Ray mouseRay) { }
    public void OnRelease() { }

    private IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            Color c = buttonRenderer.material.GetColor("_BaseColor");
            c.a = Mathf.Lerp(1f, 0f, t);
            buttonRenderer.material.SetColor("_BaseColor", c);
            yield return null;
        }
        gameObject.SetActive(false);
    }

    public IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            Color c = buttonMat.GetColor("_BaseColor");
            c.a = Mathf.Lerp(0f, 1f, t);
            buttonMat.SetColor("_BaseColor", c);
            yield return null;
        }
    }
}
