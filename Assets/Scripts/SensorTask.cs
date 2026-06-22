using UnityEngine;

public class SensorTask : RepairTask
{
    [SerializeField] private SensorSlider[] sliders;

    protected override bool AttemptStep()
    {
        foreach (var slider in sliders)
            if (!slider.IsCorrect) return false;

        HologramDisplay.Instance.Clear();
        return true;
    }
}
