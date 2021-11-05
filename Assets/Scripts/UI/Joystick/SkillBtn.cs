using UnityEngine;
using UnityEngine.EventSystems;

public class SkillBtn : BaseBtu
{
    public bool isDrag;

    private void Update()
    {
        if (fingerExhale)
        {
            fingerExhale = false;
        }
    }

    
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (isDrag)
        {
            base.OnPointerDown(eventData);
        }
        else
        {
            pointDownEvent?.Invoke(baseTransform.name);
            Debug.Log(baseTransform.name+"OnPointerDown");
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (isDrag)
        {
            if (!_canHandle) return;
            var dis = eventData.position - _pointDownPos;
            var clamp = Mathf.Clamp(dis.magnitude, 0f, maxRadius);
            var normalized = clamp * dis.normalized;
            handleTransform.localPosition = normalized;
            _dir = normalized.normalized * (clamp/maxRadius);
            drawEvent?.Invoke(baseTransform.name);
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (isDrag)
        {
            base.OnPointerUp(eventData);
        }
        else
        {
            pointUpEvent?.Invoke(baseTransform.name);
            Debug.Log(baseTransform.name+"OnPointerUp");
        }
    }
}