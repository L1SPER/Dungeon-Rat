using UnityEngine;

public class FloatingCombatTextManager : MonoBehaviour
{
    public static FloatingCombatTextManager Instance { get; private set; }

    [SerializeField] private GameObject floatingCombatTextPrefab;
    [SerializeField] private RectTransform damageTextLayer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ShowDamage(int damage, RectTransform targetAnchor, Color color)
    {
        if (damage <= 0 || targetAnchor == null || floatingCombatTextPrefab == null || damageTextLayer == null)
            return;

        GameObject spawnedObj = Instantiate(floatingCombatTextPrefab, damageTextLayer);
        FloatingCombatText floatingText = spawnedObj.GetComponent<FloatingCombatText>();

        if (floatingText == null)
        {
            Debug.LogError("Prefab root objesinde FloatingCombatText scripti yok.");
            Destroy(spawnedObj);
            return;
        }

        Vector2 anchoredPosition = GetAnchoredPositionInLayer(targetAnchor, damageTextLayer);
        floatingText.Initialize(damage, anchoredPosition, color);
    }

    public void ClearAllFloatingTexts()
    {
        if (damageTextLayer == null)
        {
            Debug.LogWarning("damageTextLayer null");
            return;
        }

        Debug.Log("ClearAllFloatingTexts çağrıldı. Child count: " + damageTextLayer.childCount);

        for (int i = damageTextLayer.childCount - 1; i >= 0; i--)
        {
            Debug.Log("Siliniyor: " + damageTextLayer.GetChild(i).name);
            Destroy(damageTextLayer.GetChild(i).gameObject);
        }
    }

    private Vector2 GetAnchoredPositionInLayer(RectTransform target, RectTransform layer)
    {
        Vector3 worldPos = target.position;
        Vector3 localPos = layer.InverseTransformPoint(worldPos);
        return localPos;
    }
}