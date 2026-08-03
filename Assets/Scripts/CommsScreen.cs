using UnityEngine;
using UnityEngine.Rendering;

public class CommsScreen : RepairTask
{
    [SerializeField] public GameObject inboxGroup;
    [SerializeField] public GameObject calibrateGroup;

    protected override void Awake()
    {
        base.Awake();
        inboxGroup.SetActive(false);
        calibrateGroup.SetActive(false);
    }
    protected override bool AttemptStep() => false;
}
