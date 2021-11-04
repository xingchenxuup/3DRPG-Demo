using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackBtu : BaseBtu
{
    private void Update()
    {
        if (fingerExhale)
        {
            fingerExhale = false;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("attackOnPointerDown");
        pointDownEvent?.Invoke(baseTransform.name);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("attackOnPointerUp");
        pointUpEvent?.Invoke(baseTransform.name);
    }
}
