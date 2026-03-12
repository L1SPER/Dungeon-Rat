using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonHoverScale : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    [SerializeField] private Vector3 normalScale = new Vector3(2.5f, 13f, 2.5f);
    [SerializeField] private Vector3 hoverScale = new Vector3(2.5f, 17f, 2.5f);

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = hoverScale;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = normalScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        transform.localScale = normalScale;
    }
}
