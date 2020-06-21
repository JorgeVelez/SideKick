using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TouchHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public  Grid grid;
    [SerializeField]
    private float minDistanceForSwipe = 20f;

    public delegate void SwipeAction(DraggedDirection DD);
    public event SwipeAction OnSwipeAction;
    public enum DraggedDirection
    {
        Up,
        Down,
        Right,
        Left
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        float distance = Vector2.Distance(eventData.pressPosition, eventData.position);

        if (distance < minDistanceForSwipe)
            return;

        Vector3 dragVectorDirection = (eventData.position - eventData.pressPosition).normalized;
        float positiveX = Mathf.Abs(dragVectorDirection.x);
        float positiveY = Mathf.Abs(dragVectorDirection.y);
        DraggedDirection draggedDir;

        if (positiveX > positiveY)
        {
            draggedDir = (dragVectorDirection.x > 0) ? DraggedDirection.Right : DraggedDirection.Left;
        }
        else
        {
            draggedDir = (dragVectorDirection.y > 0) ? DraggedDirection.Up : DraggedDirection.Down;
        }
        if (OnSwipeAction != null)
            OnSwipeAction(draggedDir);
    }

    //It must be implemented otherwise IEndDragHandler won't work 
    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        //Output the name of the GameObject that is being clicked
        Debug.Log(name + "Game Object Click in Progress");
    }

    //Detect if clicks are no longer registering
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        Debug.Log(name + "No longer being clicked");
    }

}