using System.Collections;
using UnityEngine;

public class CardAnimator : MonoBehaviour
{
    public static CardAnimator Instance { get; private set; }

    [Header("Animation Settings")]
    public float drawDuration = 0.5f;
    public float playDuration = 0.3f;
    public float flipDuration = 0.4f;
    public float hoverScale = 1.2f;
    public float hoverDuration = 0.2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AnimateDraw(CardUI card, Vector3 startPos, Vector3 endPos, System.Action onComplete = null)
    {
        StartCoroutine(DrawAnimation(card, startPos, endPos, onComplete));
    }

    private IEnumerator DrawAnimation(CardUI card, Vector3 start, Vector3 end, System.Action onComplete)
    {
        RectTransform rect = card.GetComponent<RectTransform>();
        float elapsed = 0f;

        // Start from deck position
        rect.position = start;
        card.GetComponent<CanvasGroup>().alpha = 0f;

        while (elapsed < drawDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / drawDuration;

            // Ease out cubic
            t = 1f - Mathf.Pow(1f - t, 3f);

            rect.position = Vector3.Lerp(start, end, t);
            card.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0f, 1f, t / 0.5f);

            yield return null;
        }

        rect.position = end;
        card.GetComponent<CanvasGroup>().alpha = 1f;

        onComplete?.Invoke();
    }

    public void AnimatePlay(CardUI card, Vector3 targetPosition, System.Action onComplete = null)
    {
        StartCoroutine(PlayAnimation(card, targetPosition, onComplete));
    }

    private IEnumerator PlayAnimation(CardUI card, Vector3 target, System.Action onComplete)
    {
        RectTransform rect = card.GetComponent<RectTransform>();
        Vector3 start = rect.position;
        float elapsed = 0f;

        while (elapsed < playDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / playDuration;

            // Ease in out
            t = t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;

            rect.position = Vector3.Lerp(start, target, t);

            // Add slight arc
            float arcHeight = 50f;
            float arc = Mathf.Sin(t * Mathf.PI) * arcHeight;
            rect.position += new Vector3(0, arc, 0);

            yield return null;
        }

        rect.position = target;
        onComplete?.Invoke();
    }

    public void AnimateFlip(CardUI card, System.Action onMidpoint = null, System.Action onComplete = null)
    {
        StartCoroutine(FlipAnimation(card, onMidpoint, onComplete));
    }

    private IEnumerator FlipAnimation(CardUI card, System.Action onMidpoint, System.Action onComplete)
    {
        RectTransform rect = card.GetComponent<RectTransform>();
        Vector3 originalScale = rect.localScale;
        float elapsed = 0f;

        // Phase 1: Scale down to 0
        while (elapsed < flipDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (flipDuration / 2f);

            float scaleX = Mathf.Lerp(1f, 0f, t);
            rect.localScale = new Vector3(scaleX * originalScale.x, originalScale.y, originalScale.z);

            yield return null;
        }

        // Midpoint - change card face
        onMidpoint?.Invoke();
        rect.localScale = new Vector3(0f, originalScale.y, originalScale.z);

        // Phase 2: Scale up from 0
        elapsed = 0f;
        while (elapsed < flipDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (flipDuration / 2f);

            float scaleX = Mathf.Lerp(0f, 1f, t);
            rect.localScale = new Vector3(scaleX * originalScale.x, originalScale.y, originalScale.z);

            yield return null;
        }

        rect.localScale = originalScale;
        onComplete?.Invoke();
    }

    public void AnimateHover(CardUI card, bool isHovering)
    {
        StartCoroutine(HoverAnimation(card, isHovering));
    }

    private IEnumerator HoverAnimation(CardUI card, bool isHovering)
    {
        RectTransform rect = card.GetComponent<RectTransform>();
        Vector3 targetScale = isHovering ? Vector3.one * hoverScale : Vector3.one;
        Vector3 originalScale = Vector3.one;
        float elapsed = 0f;

        while (elapsed < hoverDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / hoverDuration;

            rect.localScale = Vector3.Lerp(rect.localScale, targetScale, t);

            yield return null;
        }

        rect.localScale = targetScale;
    }

    public void AnimateDamage(CardUI card, System.Action onComplete = null)
    {
        StartCoroutine(DamageAnimation(card, onComplete));
    }

    private IEnumerator DamageAnimation(CardUI card, System.Action onComplete)
    {
        RectTransform rect = card.GetComponent<RectTransform>();
        Vector3 originalPos = rect.anchoredPosition;
        float shakeDuration = 0.3f;
        float shakeIntensity = 10f;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float x = Random.Range(-shakeIntensity, shakeIntensity);
            float y = Random.Range(-shakeIntensity, shakeIntensity);

            rect.anchoredPosition = originalPos + new Vector2(x, y);

            yield return null;
        }

        rect.anchoredPosition = originalPos;
        onComplete?.Invoke();
    }

    public void AnimateDestroy(CardUI card, System.Action onComplete = null)
    {
        StartCoroutine(DestroyAnimation(card, onComplete));
    }

    private IEnumerator DestroyAnimation(CardUI card, System.Action onComplete)
    {
        RectTransform rect = card.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = card.GetComponent<CanvasGroup>();
        Vector3 originalScale = rect.localScale;
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Scale up and fade out
            rect.localScale = Vector3.Lerp(originalScale, originalScale * 1.5f, t);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        onComplete?.Invoke();
    }

    public void AnimateScoreChange(TextMesh scoreText, int oldScore, int newScore, System.Action onComplete = null)
    {
        StartCoroutine(ScoreChangeAnimation(scoreText, oldScore, newScore, onComplete));
    }

    private IEnumerator ScoreChangeAnimation(TextMesh scoreText, int oldScore, int newScore, System.Action onComplete)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        int displayScore = oldScore;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            displayScore = (int)Mathf.Lerp(oldScore, newScore, t);
            scoreText.text = displayScore.ToString();

            yield return null;
        }

        scoreText.text = newScore.ToString();
        onComplete?.Invoke();
    }
}
