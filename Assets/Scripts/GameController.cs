﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStates { IdlePhase, PowerPhase, SwipePhase, FlyingPhase, EndingPhase }

public class GameController : MonoBehaviour
{
    public BallTestScript Ball;
    public PowerBarScript PowerBar;

    public Transform OriginPoint;

    public Level[] Levels;
    public Level CurrLevel;

    public int tempCounter, levelCounter;

    private Vector2 fingerDown;
    private Vector2 fingerUp;
    public bool detectSwipeOnlyAfterRelease = false;

    public float SWIPE_THRESHOLD = 20f;

    public bool WonLevel;

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
                ControlFlying();
                break;
            case GameStates.EndingPhase:
                ControlEnding();
                break;
            default:
                break;
        }
    }

    

    private void ControlStart()
    {
        if (Input.touchCount > 0) 
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                ChangeStates();
                tempCounter = 0;
            }
        }
        
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

    public void ControlFlying()
    {
        bool xPassed = !CurrLevel.RequiresDistance || Ball.transform.position.x > CurrLevel.DistanceMeasurement;

        bool yPassed = !CurrLevel.RequiresHeight || Ball.transform.position.y > CurrLevel.HeightMeasurement;

        if (xPassed && yPassed)
        {
            WonLevel = true;
        }

        if (Ball.rb.velocity.magnitude == 0)
        {
            ChangeStates();
        }
    }

    private void ControlEnding()
    {
        if (WonLevel)
        {
            levelCounter++;

            //Show Level Won logic
        }
        else
        {
            //Show Level Lost Logic
        }

        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                LoadLevel();
                ChangeStates();
            }
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

    #region Level Methods
    public void LoadLevel()
    {
        CurrLevel = Levels[levelCounter];

        Ball.rb.velocity = Vector3.zero;
        Ball.transform.position = OriginPoint.position;


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
