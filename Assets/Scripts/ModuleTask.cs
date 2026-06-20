using UnityEngine;

public class ModuleTask : RepairTask
{
    [SerializeField] private Collider moduleCollider;

    void Awake()
    {
        moduleCollider.enabled = false;
    }
    public void OnPanelOpened()
    {
        moduleCollider.enabled = true;
    }

    public void OnModuleReseated() => TryRepair();
    protected override bool AttemptStep() => true;
}
