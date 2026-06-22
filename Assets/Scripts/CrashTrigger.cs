using UnityEngine;

public class CrashTrigger : MonoBehaviour
{
    [SerializeField] private IntroSequence intro;
    [SerializeField] private bool isMidpoint = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (isMidpoint)
            intro.OnHallwayMidPoint();
        else
            intro.OnExitCryoRoom();
    }
}
