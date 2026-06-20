using UnityEngine;

public class AntennaTask : RepairTask
{
    [SerializeField] private AntennaDial azimuthDial;
    [SerializeField] private AntennaDial elevationDial;
    [SerializeField] private Light alignmentLight;
    [SerializeField] private ModuleTask prerequisite;

    private bool isPowered = false;

    void Awake()
    {
        prerequisite.OnRepaired += _ => isPowered = true;
    }

    void Update()
    {
        alignmentLight.enabled = isPowered && azimuthDial.IsAligned && elevationDial.IsAligned;
    }

    protected override bool AttemptStep()
    {
        return isPowered && azimuthDial.IsAligned && elevationDial.IsAligned;
    }
}
