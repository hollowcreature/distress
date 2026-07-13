using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class CommsReadout : MonoBehaviour
{
    [SerializeField] private AntennaDial dial_left;
    [SerializeField] private AntennaDial dial_right;
    [SerializeField] private RectTransform cursor_left;
    [SerializeField] private RectTransform cursor_right;
    [SerializeField] private RectTransform marker_left;
    [SerializeField] private RectTransform marker_right;
    [SerializeField] private RectTransform track_left;
    [SerializeField] private RectTransform track_right;

    void Start()
    {
        float halfLeft = track_left.rect.width * 0.5f;
        float halfRight = track_right.rect.width * 0.5f;
        marker_left.anchoredPosition = new Vector2(track_left.rect.width * (dial_left.TargetAngle / 360f) - halfLeft, 0f);
        marker_right.anchoredPosition = new Vector2(track_right.rect.width * (dial_right.TargetAngle / 360f) - halfRight, 0f);
    }

    void Update()
    {
        float t_left = Mathf.Repeat(dial_left.Angle, 360f) / 360f;
        cursor_left.anchoredPosition = new Vector2(track_left.rect.width * t_left - track_left.rect.width * 0.5f, 0f);

        float t_right = Mathf.Repeat(dial_right.Angle, 360f) / 360f;
        cursor_right.anchoredPosition = new Vector2(track_right.rect.width * t_right - track_right.rect.width * 0.5f, 0f);
    }
}
