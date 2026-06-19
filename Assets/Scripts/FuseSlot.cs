using UnityEngine;

public class FuseSlot : MonoBehaviour, IFocusInteractable
{
    [SerializeField] private PanelGenerator panel;
    [SerializeField] private FocusGlow ghostGlow;
    [SerializeField] private GameObject ghostMesh;
    [SerializeField] private GameObject fuseMesh;

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
            inserted = true;
            ghostMesh.SetActive(false);
            fuseMesh.SetActive(true);
            PlayerHasFuse = false;
            panel.InsertFuse();
        }
        else
            ThoughtDisplay.Instance.Show("Something's missing here.");

    }

    void Awake()
    {
        ghostMesh.SetActive(true);
        fuseMesh.SetActive(false);
    }
}
