using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SKHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent onTouchOver = new UnityEvent();
    public UnityEvent onTouchOut = new UnityEvent();

    bool isOver = false;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
        onTouchOver.Invoke();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isOver)
            onTouchOut.Invoke();
        isOver = false;
    }

}
