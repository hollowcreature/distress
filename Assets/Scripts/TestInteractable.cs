using UnityEngine;

public class TestInteractable : RepairTask
{
    private bool active;

    public override void OnFocusEnter() => active = true;
    public override void OnFocusExit() => active = false;

    void Update()
    {
        if (!active)
            return;

        if (Input.GetKeyDown(KeyCode.F))
            TryRepair();
    }

    protected override bool AttemptStep() => true;
}
