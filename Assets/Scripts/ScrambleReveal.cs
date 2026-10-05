using System.Collections;
using TMPro;
using UnityEngine;

public class ScrambleReveal : MonoBehaviour
{
    [SerializeField] private TMP_Text textComponent;
    [SerializeField] private float scrambleDuration = 0.05f;
    [SerializeField] private float startDelay = 0f;
    private const string charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789#@!%";
    [SerializeField] private string finalText;

    void OnEnable()
    {
        StartCoroutine(Scramble());
    }

    private IEnumerator Scramble()
    {
        textComponent.text = "";

        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        for (int i = 0; i < finalText.Length; i++)
        {
            float elapsed = 0f;
            while (elapsed < scrambleDuration)
            {
                textComponent.text = finalText.Substring(0, i) + RandomChar(finalText[i]);
                elapsed += Time.deltaTime;
                yield return null;
            }
            textComponent.text = finalText.Substring(0, i + 1);
        }
    }

    private char RandomChar(char original)
    {
        if (original == ' ' || original == '\n')
            return original;
        return charset[Random.Range(0, charset.Length)];
    }
}
