using UnityEngine;

public class ModuleTask : RepairTask
{
    [SerializeField] private Collider moduleCollider;

    protected override void Awake()
    {
        base.Awake();
        moduleCollider.enabled = false;
    }
    public void OnPanelOpened()
    {
        moduleCollider.enabled = true;
    }

    public void OnModuleReseated() => TryRepair();
    protected override bool AttemptStep() => true;
}
