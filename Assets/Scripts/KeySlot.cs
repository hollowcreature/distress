using System.Collections;
using UnityEngine;

public class KeySlot : MonoBehaviour, IFocusInteractable
{
    [SerializeField] private KeyHandle keyHandle;
    [SerializeField] private KeyPickup key;
    [SerializeField] private GameObject keyObject;
    [SerializeField] private Transform insertAnchor;
    [SerializeField] private float insertDuration = 2f;
    [SerializeField] private AudioSource keyInsertSound;

    private FocusGlow glow;
    private Collider keyCollider;
    private Renderer[] keyRenderers;
    private Vector3 keyStartPos;

    private bool inserted = false;

    void Awake()
    {
        glow = GetComponent<FocusGlow>();
        keyCollider = keyObject.GetComponent<Collider>();
        keyRenderers = keyObject.GetComponentsInChildren<Renderer>();

        keyStartPos = keyObject.transform.localPosition;
        keyCollider.enabled = false;
        foreach (var r in keyRenderers)
            r.enabled = false;
    }

    public void OnHoverEnter() => glow.Show();
    public void OnHoverExit() => glow.Hide();

    public void OnPress()
    {
        if (!key.hasKey)
        {
            ThoughtDisplay.Instance.Show("I don't have the key to that...");
            return;
        }

        if (inserted)
            return;

        inserted = true;
        foreach (var r in keyRenderers)
            r.enabled = true;

        keyInsertSound.Play();
        StartCoroutine(InsertKey());
    }

    public void OnDrag(Ray mouseRay) { }
    public void OnRelease() { }

    private IEnumerator InsertKey()
    {
        yield return new WaitForSeconds(0.5f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / insertDuration;
            keyObject.transform.localPosition = Vector3.Lerp(keyStartPos, insertAnchor.localPosition, t);
            yield return null;
        }
        keyObject.transform.localPosition = insertAnchor.localPosition;
        keyCollider.enabled = true;

        keyHandle.isInserted = true;
    }
}
