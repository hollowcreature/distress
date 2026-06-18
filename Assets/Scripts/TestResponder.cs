using NUnit.Framework.Internal;
using UnityEngine;

public class TestResponder : MonoBehaviour
{
    [SerializeField] private RepairTask test_task;

    void Awake()
    {
        test_task.OnRepaired += _ => LogWrite();
    }

    void LogWrite()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }
}
