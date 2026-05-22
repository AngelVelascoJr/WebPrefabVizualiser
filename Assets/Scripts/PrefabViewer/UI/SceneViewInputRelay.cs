using UnityEngine;
using UnityEngine.EventSystems;

namespace PrefabViewer.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class SceneViewInputRelay : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler, IDragHandler, IScrollHandler
    {
        public SceneViewPanel panel;

        public void OnPointerDown(PointerEventData eventData) => panel?.OnPointerDown(eventData);
        public void OnPointerUp(PointerEventData eventData) => panel?.OnPointerUp(eventData);
        public void OnDrag(PointerEventData eventData) => panel?.OnDrag(eventData);
        public void OnScroll(PointerEventData eventData) => panel?.OnScroll(eventData);
    }
}
