using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClockController : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    private int hour;
    private int minutes;
    private float timer;
    private bool paused = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Start at 8am
        hour = 8;
        minutes = 0;
        timer = 0f;
        UpdateText();
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
                        EndDay();
                    }
                }

                UpdateText();
            }
        }
    }

    private void UpdateText()
    {
        StringBuilder stringBuilder = new StringBuilder();

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
        Debug.Log("Day has ended!");
        paused = true;
        SceneManager.LoadScene("TrashFacilityScene");
    }
}