using System.Collections;
using UnityEngine;

public class FuseSlot : MonoBehaviour, IFocusInteractable
{
    [SerializeField] private PanelGenerator panel;
    [SerializeField] private FocusGlow ghostGlow;
    [SerializeField] private GameObject ghostMesh;
    [SerializeField] private GameObject fuseMesh;
    [SerializeField] private AudioSource insertSound;

    public static bool PlayerHasFuse;
    private bool inserted = false;
    public void OnHoverEnter() => ghostGlow.Show();
    public void OnHoverExit() => ghostGlow.Hide();
    public void OnDrag(Ray mouseRay) { }
    public void OnRelease() { }

    public void OnPress()
    {
        if (inserted)
            return;

        if (PlayerHasFuse)
        {
            insertSound.Play();
            inserted = true;
            ghostMesh.SetActive(false);
            fuseMesh.SetActive(true);
            PlayerHasFuse = false;
            panel.InsertFuse();
        }
        else
        {
            StartCoroutine(ShowPrompts());
        }

    }

    private IEnumerator ShowPrompts()
    {
        yield return ThoughtDisplay.Instance.Show("Something's missing here.");
        ObjectiveDisplay.Instance.Show("New Objective: Find the fuse");
    }

    void Awake()
    {
        ghostMesh.SetActive(true);
        fuseMesh.SetActive(false);
    }
}
