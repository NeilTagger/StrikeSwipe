﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameStates { IdlePhase, PowerPhase, SwipePhase, FlyingPhase, EndingPhase }

public class GameController : MonoBehaviour
{
    public BallTestScript Ball;
    public PowerBarScript PowerBar;
    public TileManager tileManager;
    public ParallaxScript[] backgrounds;
    public DistanceMeterScript disMeter;
    public HeightBarScript hiMeter;
    private AudioSource sound;


    public Transform OriginPoint;

    public Level[] Levels;
    public Level CurrLevel;

    public int tempCounter, levelCounter;
    public Text timerOrDistance, levelGoalText;
    public Image ArrowImage;

    private Vector2 fingerDown;
    private Vector2 fingerUp;
    public bool detectSwipeOnlyAfterRelease = false;
    public bool waitForSwipe = true;

    public float SWIPE_THRESHOLD = 20f;
    public float TAP_TIME = 5f;
    public float timeRemaining;

    public bool WonLevel, FailedLevel;
    public bool gameStarted = false;
    public bool flying;

    public GameStates state;

    public static GameController Instance;

    float flyingHeight;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        state = GameStates.IdlePhase;
        PowerBar.gameObject.SetActive(false);
        sound = GetComponent<AudioSource>();
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

        waitForSwipe = true;
        levelGoalText.gameObject.SetActive(true);

        timerOrDistance.text = "Tap to start";

        levelGoalText.text = GetLevelText();
        if (gameStarted)
        {
            disMeter.drawMarks();
            hiMeter.drawMarks();
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    sound.Play();
                    PowerBar.gameObject.SetActive(true);
                    PowerBar.Initialize();
                    PowerBar.Initialize();
                    levelGoalText.gameObject.SetActive(false);
                    ChangeStates();
                    tempCounter = 0;
                }
            }
        }
    }

    private string GetLevelText()
    {
        string Text = "";

        if (CurrLevel.RequiresDistance)
        {
            Text += "Reach a distance of " + CurrLevel.DistanceMeasurement;
        }

        if (CurrLevel.RequiresHeight)
        {
            if (Text != "")
            {
                Text += " and ";
            }

            Text += "Reach a height of " + CurrLevel.HeightMeasurement;
        }
        else if (CurrLevel.HeightLock)
        {
            Text += " and Don't reach a height of " + CurrLevel.HeightMeasurement;
        }



        Text += "!";

        return Text;
    }

    private void ControlTapping()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerOrDistance.text = "Time remaining: " + timeRemaining.ToString("F1");
        }
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                PowerBar.AddPower();
            }

        }

        if (timeRemaining <= 0 && waitForSwipe)
        {
            waitForSwipe = false;
            Invoke("ChangeStates", 0.15f);
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

            if (angle > 90)
            {
                angle = 90;
            }

            angle = Mathf.Lerp(angle, 45, 0.05f);


            Debug.Log("angle is: "+angle);


            Vector3 Power = Quaternion.Euler(0, 0, angle) * Vector3.right;

            Power *= PowerBar.finalPower;
            Ball.PlaySound(1);
            Ball.rb.AddForce(Power);
            ChangeStates();

        }
    }

    public void ControlFlying()
    {
        bool xPassed = false, yPassed = false;

        if (!FailedLevel)
        {
            FailedLevel = (Ball.transform.position.y > CurrLevel.HeightMeasurement && (CurrLevel.HeightLock)) ||
                          (Ball.transform.position.x > CurrLevel.DistanceMeasurement && (CurrLevel.DistanceLock));

            xPassed = !CurrLevel.RequiresDistance || Ball.transform.position.x > CurrLevel.DistanceMeasurement;

            yPassed = !CurrLevel.RequiresHeight || Ball.transform.position.y > CurrLevel.HeightMeasurement;
        }

        if (Ball.transform.position.y > flyingHeight)
        {
            flyingHeight = Ball.transform.position.y;
        }

        if (xPassed && yPassed)
        {
            WonLevel = true;
        }

        if (Ball.rb.velocity.magnitude == 0 && flying)
        {
            ChangeStates();
        }
        else if (!flying)
        {
            flying = true;
        }
    }

    private void ControlEnding()
    {
        

        if (Input.touchCount > 0)
        {

            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                sound.Play();
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
            Debug.Log("swipevector normalized: "+swipeVector.normalized);

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

    public void SkipLevel()
    {
        state = GameStates.EndingPhase;

        PowerBar.powerBarGlow.gameObject.SetActive(false);
        PowerBar.gameObject.SetActive(false);

        levelCounter++;

        LoadLevel();
    
    }

    public void LoadLevel()
    {
        CurrLevel = Levels[levelCounter];
        waitForSwipe = false;
        Ball.rb.velocity = Vector3.zero;
        Ball.transform.position = OriginPoint.position;
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].resetBackground();
        }
        tileManager.ResetTiles();

    }

    #endregion


    public void ChangeStates()
    {
        switch (state)
        {
            case GameStates.IdlePhase:
                Ball.GetComponent<Renderer>().material.SetFloat("Power", 0);
                timeRemaining = TAP_TIME;
                state = GameStates.PowerPhase;
                PowerBar.powerBarGlow.gameObject.SetActive(true);
                PowerBar.powerBarGlow.color = Color.clear;
                break;
            case GameStates.PowerPhase:
                ArrowImage.gameObject.SetActive(true);
                PowerBar.powerBarGlow.gameObject.SetActive(false);


                PowerBar.gameObject.SetActive(false);
                timerOrDistance.text = "Swipe to launch!";

                flyingHeight = 0;

                state = GameStates.SwipePhase;
                break;
            case GameStates.SwipePhase:
                timerOrDistance.gameObject.SetActive(false);
                flying = false;

                ArrowImage.gameObject.SetActive(false);

                Ball.GetComponent<Renderer>().material.SetFloat("Power", 0);
                state = GameStates.FlyingPhase;
                break;
            case GameStates.FlyingPhase:
                timerOrDistance.gameObject.SetActive(true);
                timerOrDistance.text = "Distance traveled: " + Ball.rb.position.x.ToString("F2") + '\n' + "Height traveled: " + flyingHeight.ToString("F2");

                levelGoalText.gameObject.SetActive(true);

                if (WonLevel)
                {
                    levelCounter++;

                    levelGoalText.text = "You won! Tap to advance to the next level";
                }
                else
                {
                    levelGoalText.text = "You lost! Tap to restart the level";
                }

                state = GameStates.EndingPhase;
                break;
            case GameStates.EndingPhase:
                WonLevel = false;
                FailedLevel = false;
                state = GameStates.IdlePhase;
                break;
            default:
                break;
        }

    }
}
