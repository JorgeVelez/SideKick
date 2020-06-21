using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;


public class SKButton : MonoBehaviour
{
    public UnityEvent onTouchDown = new UnityEvent();
    public UnityEvent onTouchUp = new UnityEvent();

    bool isDown = false;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDown(Vector3 value)
    {
        isDown = true;
        onTouchDown.Invoke();
    }

    public void OnUp(Vector3 value)
    {
        if(isDown)
            onTouchUp.Invoke();
        isDown = false;
    }

   
}
