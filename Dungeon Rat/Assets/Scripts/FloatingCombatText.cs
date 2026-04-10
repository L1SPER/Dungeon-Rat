using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingCombatText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private float lifetime = 0.8f;
    [SerializeField] private float floatDistance = 60f;
    [SerializeField] private Vector2 uiOffset = new Vector2(0f, 20f);

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Initialize(int damage, Vector2 startAnchoredPosition, Color color)
    {
        if (damageText == null)
        {
            Debug.LogError("FloatingCombatText > damageText referansı boş.");
            return;
        }

        damageText.text = damage.ToString();
        damageText.color = color;

        Vector2 startPos = startAnchoredPosition + uiOffset;
        rectTransform.anchoredPosition = startPos;

        StartCoroutine(Play(startPos));
    }

    private IEnumerator Play(Vector2 startPosition)
    {
        Vector2 endPosition = startPosition + Vector2.up * floatDistance;
        float timer = 0f;

        while (timer < lifetime)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / lifetime;

            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t);

            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        Destroy(gameObject);
    }
}