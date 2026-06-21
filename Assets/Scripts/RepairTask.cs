using UnityEngine;
using System;

public abstract class RepairTask : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform cameraAnchor;
    [SerializeField] private RepairTask prerequisiteTask;
    private Collider col;
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

    protected virtual void Awake()
    {
        col = GetComponent<Collider>();
        if (prerequisiteTask != null)
        {
            if (col != null)
                col.enabled = false;

            prerequisiteTask.OnRepaired += _ =>
            {
                if (col != null)
                    col.enabled = true;
            };
        }
    }
}
