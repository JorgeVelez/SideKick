using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SKDragSlider : MonoBehaviour
{
    public delegate void OnValueChangedEvent(float value);
    public event OnValueChangedEvent OnValueChanged;

    bool isDown = false;
    RectTransform val;

    [Range(-1f, 1f)]
    public float ChangeSpeed = 1;

    public float minValue = 0;
    public float maxValue = 1;
    public bool wholeNumbers = false;
    public float Value = 0;
    void Start()
    {
        val = transform.Find("Value").GetComponent<RectTransform>();
        transform.GetComponentInChildren<Text>().text = Value.ToString();
        val.GetComponentInChildren<Text>().text = Value.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDown(Vector3 value)
    {
        isDown = true;
        val.gameObject.SetActive(true);
        val.transform.position = worldToUISpace(val.GetComponentInParent<Canvas>(), value)-(Vector3.right*100);
    }

    public void OnUp(Vector3 value)
    {
        isDown = false;
        val.gameObject.SetActive(false);
    }

    public void OnDrag(Vector2 value)
    {
        if (isDown)
        {
            Debug.Log(value.ToString());
            val.transform.position = val.transform.position + ((Vector3)value);

            OnValueChanged(0);
        }
    }

    public Vector3 worldToUISpace(Canvas parentCanvas, Vector3 worldPos)
    {
        //Convert the world for screen point so that it can be used with ScreenPointToLocalPointInRectangle function
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 movePos;

        //Convert the screenpoint to ui rectangle local point
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out movePos);
        //Convert the local point to world point
        return parentCanvas.transform.TransformPoint(movePos);
    }
}
