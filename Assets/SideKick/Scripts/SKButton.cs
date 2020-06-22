using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;


public class SKButton : MonoBehaviour
{
    public UnityEvent onTouchDown = new UnityEvent();
    public UnityEvent onTouchUp = new UnityEvent();

    public Color colouDown = Color.white;
    public Color colouOriginal = Color.white;

    bool isDown = false;
    void Awake()
    {
        colouOriginal = transform.GetComponentInChildren<Image>().color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDown(Vector3 value)
    {
       
        transform.GetComponentInChildren<Image>().color = colouDown;

        isDown = true;
        onTouchDown.Invoke();
    }

    public void OnUp(Vector3 value)
    {
       


        if (isDown)
        {
            transform.GetComponentInChildren<Image>().color = colouOriginal;

            onTouchUp.Invoke();
        }
           
        isDown = false;
    }

   
}
