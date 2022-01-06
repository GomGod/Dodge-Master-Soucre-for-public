using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using TouchPhase = UnityEngine.TouchPhase;


public class InputManager : MonoBehaviour
{
    private readonly List<InputListener> _inputListeners = new(); 
    // Start is called before the first frame update
    private bool disableInputDetection = false;

    public void DisableInputDetection() => disableInputDetection = true;
    public void EnableInputDetection() => disableInputDetection = false;
    
    
    #region SwipeDetection

    private Vector2 _startPosition;
    private Vector2 _endPosition;
    
    [SerializeField] private float minimumDist = .2f;
    
    private void SwipeStart(Vector2 position)
    {
        _startPosition = position;
    }
    
    private void SwipeEnd(Vector2 position)
    {
        _endPosition = position;
        DetectSwipe();
    }

    private void DetectSwipe()
    {
        if (!(Vector3.Distance(_startPosition, _endPosition) >= minimumDist)) return;
        var dir = _endPosition - _startPosition;
        var dir2d = new Vector2(dir.x, dir.y).normalized;

        SwipeDirection(dir2d);
    }

    private float directionThreshold = .9f;
    private void SwipeDirection(Vector2 dir)
    {
        if (Vector2.Dot(Vector2.up, dir) > directionThreshold)
        {
            foreach (var listener in _inputListeners)
            {
                listener.Up();
            }
        }
        else if (Vector2.Dot(Vector2.left, dir) > directionThreshold)
        {
            foreach (var listener in _inputListeners)
            {
                listener.Left();
            }
        }
        else if (Vector2.Dot(Vector2.down, dir) > directionThreshold)
        {
            foreach (var listener in _inputListeners)
            {
                listener.Down();
            }
            
        }
        else if (Vector2.Dot(Vector2.right, dir) > directionThreshold)
        {
            foreach (var listener in _inputListeners)
            {
                listener.Right();
            }
        }
    }

    #endregion
    
    public void OnEnable()
    {
        TouchSimulation.Enable();
    }

    public void OnDisable()
    {
        TouchSimulation.Disable();
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (disableInputDetection)
            return;
        
        //#if UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    SwipeStart(touch.position);
                    break;
                case TouchPhase.Ended:
                    SwipeEnd(touch.position);
                    break;
                case TouchPhase.Moved:
                    break;
                case TouchPhase.Stationary:
                    break;
                case TouchPhase.Canceled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        //#endif
        
#if UNITY_EDITOR_WIN
        var keyboard = Keyboard.current;
        if (keyboard.leftArrowKey.isPressed)
        {
            foreach (var listener in _inputListeners)
            {
                listener.Left();
            }
        }

        if (keyboard.rightArrowKey.isPressed)
        {
            foreach (var listener in _inputListeners)
            {
                listener.Right();
            }
        }

        if (keyboard.upArrowKey.isPressed)
        {
            foreach (var listener in _inputListeners)
            {
                listener.Up();
            }
        }

        if (keyboard.downArrowKey.isPressed)
        {
            foreach (var listener in _inputListeners)
            {
                listener.Down();
            }
        }
#endif
    }

    public void AddListener(InputListener listener)
    {
        _inputListeners.Add(listener);
    }
    
    public void RemoveListener(InputListener listener)
    {
        _inputListeners.Remove(listener);
    }
}
