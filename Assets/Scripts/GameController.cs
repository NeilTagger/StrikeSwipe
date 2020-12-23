using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStates { IdlePhase, PowerPhase, SwipePhase, FlyingPhase, EndingPhase }

public class GameController : MonoBehaviour
{
    public BallTestScript Ball;
    public PowerBarScript PowerBar;

    public int tempCounter;

    private Vector2 fingerDown;
    private Vector2 fingerUp;
    public bool detectSwipeOnlyAfterRelease = false;

    public float SWIPE_THRESHOLD = 20f;

    public GameStates state;

    // Start is called before the first frame update
    void Start()
    {
        state = GameStates.IdlePhase;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case GameStates.IdlePhase:
                ControlStart();
                break;
            case GameStates.PowerPhase:
                ControlTapping();
                break;
            case GameStates.SwipePhase:
                ControlSwiping();
                break;
            case GameStates.FlyingPhase:
                break;
            case GameStates.EndingPhase:
                break;
            default:
                break;
        }
    }

    private void ControlStart()
    {
        ChangeStates();
        tempCounter = 0;
    }

    private void ControlTapping()
    {
        if (Input.touchCount > 0) // replace if with: Input.touchCount > 0
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                tempCounter++;
                PowerBar.AddPower();
            }

        }

        if (tempCounter > 10)
        {
            ChangeStates();
        }
    }

    private void ControlSwiping()
    {
        float angle = 0;

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUp = touch.position;
                fingerDown = touch.position;
            }

            //Detects Swipe while finger is still moving
            if (touch.phase == TouchPhase.Moved)
            {
                if (!detectSwipeOnlyAfterRelease)
                {
                    fingerDown = touch.position;
                    angle = checkSwipe();
                }
            }

            //Detects swipe after finger is released
            if (touch.phase == TouchPhase.Ended)
            {
                fingerDown = touch.position;
                angle = checkSwipe();
            }
        }

        if (angle > 0)
        {

            Debug.Log(angle);
            print("Use stored power");

            Vector3 Power = Quaternion.Euler(0, 0, angle) * Vector3.right;

            Power *= PowerBar.finalPower;

            Ball.rb.AddForce(Power);
            ChangeStates();

        }
    }

    #region Swipe Methods
    float checkSwipe()
    {
        Vector2 swipeVector;
        Vector2 baseDir = Vector2.right;

        swipeVector.x = verticalMove();

        swipeVector.y = horizontalValMove();

        if (swipeVector.magnitude > SWIPE_THRESHOLD * SWIPE_THRESHOLD)
        {
            Debug.Log(swipeVector.normalized);

            return FindDegree(swipeVector.x, swipeVector.y);
        }

        return -1;
    }

    public float FindDegree(float x, float y)
    {
        float value = (float)((Mathf.Atan2(x, y) / Math.PI) * 180f);
        if (value < 0) value += 360f;

        return value;
    }


    float verticalMove()
    {
        return fingerDown.y - fingerUp.y;
    }

    float horizontalValMove()
    {
        return fingerDown.x - fingerUp.x;
    }
    #endregion

    public void ChangeStates()
    {
        switch (state)
        {
            case GameStates.IdlePhase:
                state = GameStates.PowerPhase;
                break;
            case GameStates.PowerPhase:
                state = GameStates.SwipePhase;
                break;
            case GameStates.SwipePhase:
                state = GameStates.FlyingPhase;
                break;
            case GameStates.FlyingPhase:
                state = GameStates.EndingPhase;
                break;
            case GameStates.EndingPhase:
                state = GameStates.IdlePhase;
                break;
            default:
                break;
        }

    }
}
