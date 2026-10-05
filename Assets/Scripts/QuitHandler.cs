using UnityEngine;

public class QuitHandler : MonoBehaviour
{
    private bool readyToQuit = false;

    void Awake() => DontDestroyOnLoad(gameObject);

    void OnEnable() => Application.wantsToQuit += HandleWantsToQuit;
    void OnDisable() => Application.wantsToQuit -= HandleWantsToQuit;

    private bool HandleWantsToQuit()
    {
        if (readyToQuit)
            return true;

        readyToQuit = true;
        Application.Quit();
        return false;
    }
}
