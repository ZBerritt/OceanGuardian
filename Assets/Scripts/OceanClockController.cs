using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ClockController : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] TMP_Text endDayText;
    [SerializeField] AudioClip dayEndSfx;
    private int hour;
    private int minutes;
    private float timer;
    private bool paused = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Ensure text starts hidden
        endDayText.enabled = false;
        GameManager.Instance.OnDayEnd += EndDay;
        // Start at 8am
        hour = 8;
        minutes = 0;
        timer = 0f;
        UpdateText();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnDayEnd -= EndDay;
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused)
        {
            timer += Time.deltaTime;

            // Increment minutes by 10 every second
            if (timer >= 1f)
            {
                timer = 0f;
                minutes += 10;

                // When minutes reach 60, increment hour
                if (minutes >= 60)
                {
                    minutes = 0;
                    hour++;

                    // Check if it's 2 PM (14:00) to end the day
                    if (hour >= 14)
                    {
                        GameManager.Instance.EndDay();
                    }
                }

                UpdateText();
            }
        }
    }

    private void UpdateText()
    {
        StringBuilder stringBuilder = new();

        // Add leading zero for hour if less than 10
        if (hour < 10)
        {
            stringBuilder.Append("0");
        }
        stringBuilder.Append(hour);

        stringBuilder.Append(":");

        // Add leading zero for minutes if less than 10
        if (minutes < 10)
        {
            stringBuilder.Append("0");
        }
        stringBuilder.Append(minutes);

        text.text = stringBuilder.ToString();
    }

    private void EndDay()
    {
        paused = true;
        endDayText.enabled = true;
    }
}