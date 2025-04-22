using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BufferTransitionController : MonoBehaviour
{
    [SerializeField] List<string> coolFactoids;
    [SerializeField] GameObject prefab;
    [SerializeField] float fadeDuration = 1.0f;
    [SerializeField] TMP_FontAsset tmpFontAsset;

    public static BufferTransitionController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Coroutine for transitioning to another scene
    public IEnumerator TransitionScene(string to, float wait, Sprite bufferSprite)
    {
        Debug.Log("Transitioning to " + to);

        // Select random factoid
        int i = UnityEngine.Random.Range(0, coolFactoids.Count);
        string factoid = coolFactoids[i];
        Debug.Log("Selected factoid: " + factoid);

        // Instantiate game object
        GameObject buffer = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        Canvas canvas = buffer.GetComponent<Canvas>();

        // Create a single image that will change colors/sprites
        GameObject transitionImage = new GameObject("Transition Image");
        transitionImage.transform.SetParent(canvas.transform, false);
        Image image = transitionImage.AddComponent<Image>();

        // Set to fill screen
        RectTransform rectTransform = image.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        // Create factoid text object (initially invisible)
        GameObject factoidObject = new GameObject("Factoid Text");
        factoidObject.transform.SetParent(canvas.transform, false);

        // Configure the text component
        TextMeshProUGUI textComponent = factoidObject.AddComponent<TextMeshProUGUI>();
        textComponent.text = factoid;
        textComponent.fontSize = 24;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.color = new Color(1f, 1f, 1f, 0f); // Start transparent
        textComponent.font = tmpFontAsset;

        // Position the text at the top of the screen
        RectTransform textRect = factoidObject.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.85f); // Position near top
        textRect.anchorMax = new Vector2(0.5f, 0.85f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.sizeDelta = new Vector2(800f, 100f); // Width and height of text area

        // STEP 1: Fade from transparent to black
        image.color = new Color(0f, 0f, 0f, 0f); // Start transparent
        yield return StartCoroutine(FadeImageToColor(image, Color.black, fadeDuration));

        // STEP 2: Fade from black to buffer image
        image.sprite = bufferSprite; // Set buffer sprite
        yield return StartCoroutine(CrossFadeToSprite(image, Color.black, Color.white, fadeDuration));
        yield return StartCoroutine(FadeText(textComponent, 0f, 1f, fadeDuration));

        // Wait for the specified time to show buffer image and factoid
        yield return new WaitForSeconds(wait);

        // STEP 3: Fade from buffer image back to black
        yield return StartCoroutine(FadeText(textComponent, 1f, 0f, fadeDuration));
        yield return StartCoroutine(CrossFadeToColor(image, Color.white, Color.black, fadeDuration));

        // STEP 4: Load the new scene
        SceneManager.LoadScene(to);
    }

    // Fade from current alpha to target color
    private IEnumerator FadeImageToColor(Image image, Color targetColor, float duration)
    {
        Color startColor = image.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / duration);

            image.color = Color.Lerp(startColor, targetColor, normalizedTime);

            yield return null;
        }

        // Ensure we end at exactly the target color
        image.color = targetColor;
    }

    // Cross fade from solid color to sprite
    private IEnumerator CrossFadeToSprite(Image image, Color startColor, Color targetColor, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / duration);

            image.color = Color.Lerp(startColor, targetColor, normalizedTime);

            yield return null;
        }

        // Ensure we end at exactly the target color
        image.color = targetColor;
    }

    // Cross fade from sprite to solid color
    private IEnumerator CrossFadeToColor(Image image, Color startColor, Color targetColor, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / duration);

            image.color = Color.Lerp(startColor, targetColor, normalizedTime);

            yield return null;
        }

        // Ensure we end at exactly the target color
        image.color = targetColor;
        image.sprite = null; // Remove sprite when fully black
    }

    // Fade text alpha (TextMeshPro version)
    private IEnumerator FadeText(TMPro.TextMeshProUGUI text, float startAlpha, float targetAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color startColor = text.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / duration);

            text.color = Color.Lerp(
                new Color(startColor.r, startColor.g, startColor.b, startAlpha),
                targetColor,
                normalizedTime
            );

            yield return null;
        }

        // Ensure we end at exactly the target alpha
        text.color = targetColor;
    }

    // Fade text alpha (Legacy UI version)
    private IEnumerator FadeText(Text text, float startAlpha, float targetAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color startColor = text.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / duration);

            text.color = Color.Lerp(
                new Color(startColor.r, startColor.g, startColor.b, startAlpha),
                targetColor,
                normalizedTime
            );

            yield return null;
        }

        // Ensure we end at exactly the target alpha
        text.color = targetColor;
    }
}