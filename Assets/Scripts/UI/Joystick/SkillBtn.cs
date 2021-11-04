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
            base.OnDrag(eventData);
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