using UnityEngine;
using System;
using UnityEngine.UIElements;

public abstract class RepairTask : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform cameraAnchor;
    public Transform CameraAnchor => cameraAnchor;
    public event Action<RepairTask> OnRepaired;
    public bool IsRepaired { get; private set; }

    public void Interact() => FocusController.Instance.EnterFocus(this);

    public bool TryRepair()
    {
        if (IsRepaired) return false;
        if (!AttemptStep()) return false;

        IsRepaired = true;
        OnRepaired?.Invoke(this);
        return true;
    }

    protected abstract bool AttemptStep();

    public virtual void OnFocusEnter() { }
    public virtual void OnFocusExit() { }
}
