using UnityEngine;

public class PanelGenerator : RepairTask
{
    [SerializeField] private PanelSlider[] sliders;

    public bool HasFuse { get; private set; }

    public void InsertFuse()
    {
        HasFuse = true;
        foreach (var slider in sliders)
            slider.Activate();
    }

    protected override bool AttemptStep()
    {
        if (!HasFuse) return false;
        foreach (var slider in sliders)
            if (!slider.IsCorrect) return false;
        return true;
    }
}
