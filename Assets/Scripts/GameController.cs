using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameStates { IdlePhase, PowerPhase, SwipePhase, FlyingPhase, EndingPhase }

public class GameController : MonoBehaviour
{
    public BallTestScript Ball;
    public PowerBarScript PowerBar;

    public Transform OriginPoint;

    public Level[] Levels;
    public Level CurrLevel;

    public int tempCounter, levelCounter;
    public Text timerOrDistance, levelGoalText;

    private Vector2 fingerDown;
    private Vector2 fingerUp;
    public bool detectSwipeOnlyAfterRelease = false;

    public float SWIPE_THRESHOLD = 20f;
    public float TAP_TIME = 10f;
    public float timeRemaining;

    public bool WonLevel;

    public GameStates state;

    public static GameController Instance;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        state = GameStates.IdlePhase;
        PowerBar.gameObject.SetActive(false);
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
        levelGoalText.gameObject.SetActive(true);

        timerOrDistance.text = "Tap to start";

        levelGoalText.text = GetLevelText();

        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                PowerBar.gameObject.SetActive(true);
                PowerBar.Initialize();
                levelGoalText.gameObject.SetActive(false);
                ChangeStates();
                tempCounter = 0;
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

        if (timeRemaining <= 0)
        {
            ChangeStates();
        }
    }

    private void ControlSwiping()
    {
        timerOrDistance.text = "Swipe to launch!";
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

    public void SkipLevel()
    {
        state = GameStates.IdlePhase;

        levelCounter++;

        LoadLevel();
    
    }

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
                Ball.GetComponent<Renderer>().material.SetFloat("Power", 0);
                timeRemaining = TAP_TIME;
                state = GameStates.PowerPhase;
                break;
            case GameStates.PowerPhase:
                state = GameStates.SwipePhase;
                break;
            case GameStates.SwipePhase:
                PowerBar.gameObject.SetActive(false);
                Ball.GetComponent<Renderer>().material.SetFloat("Power", 0);
                state = GameStates.FlyingPhase;
                break;
            case GameStates.FlyingPhase:
                timerOrDistance.text = "Distance traveled: " + Ball.rb.position.x.ToString("F2");

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
                state = GameStates.IdlePhase;
                break;
            default:
                break;
        }

    }
}
