using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class VirtualPad : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler,
    IEndDragHandler
{
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private RectTransform handler;
    [SerializeField] private RectTransform thisRectTransform;
    private bool _isDrag = false;

    public void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(InputProcessor());
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SetHandlerInputPosition();
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetHandlerInputPosition();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDrag = false;
        handler.anchoredPosition = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDrag = true;
        SetHandlerInputPosition();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDrag = false;
        handler.anchoredPosition = Vector2.zero;
    }
    
    private void SetHandlerInputPosition()
    {
        var inputPos = Vector2.zero;
#if UNITY_EDITOR_WIN
        inputPos = Input.mousePosition;
#endif

#if UNITY_ANDROID
        if(Input.touchCount > 0)
            inputPos = Input.GetTouch(0).position;
#endif
        RectTransformUtility.ScreenPointToLocalPointInRectangle(thisRectTransform, inputPos,
            mainCanvas.worldCamera, out var localPosition);

        handler.anchoredPosition = localPosition;
    }

    [SerializeField] private float dirThreshold;
    [SerializeField] private float minDist;

    private IEnumerator InputProcessor()
    {
        while (true)
        {
            yield return new WaitUntil(() => _isDrag);
            if (minDist > handler.anchoredPosition.magnitude) continue;
            
            var dir = handler.anchoredPosition.normalized;
            if (Vector2.Dot(Vector2.up, dir) > dirThreshold)
            {
                InputManager.Instance.InputUp();
            }
            else if (Vector2.Dot(Vector2.left, dir) > dirThreshold)
            {
                InputManager.Instance.InputLeft();
            }
            else if (Vector2.Dot(Vector2.down, dir) > dirThreshold)
            {
                InputManager.Instance.InputDown();
            }
            else if (Vector2.Dot(Vector2.right, dir) > dirThreshold)
            {
                InputManager.Instance.InputRight();
            }
            yield return null;
        }
    }
}
