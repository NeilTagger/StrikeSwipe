using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceMeterScript : MonoBehaviour
{
    public MarkScript[] scaleMarks;
    private List<MarkScript> activeMarks = new List<MarkScript>();
    private float xspawn;
    public int numOfMarks = 3;
    public GameObject canvas;
    public GameObject ball;
    private float ballLastLocation = 0;
    private float ballDelta = 0;
    public float units = 50f;
    private float firstmark = 0;
    public bool draw = false;
    private bool Initiated = false;

    void Start()
    {

    }

    void Update()
    {
        if (draw)
        {
            moveMarks();
        }
    }
    public void SpawnMark(int markIndex)
    {
        MarkScript MarkHolder = Instantiate(scaleMarks[markIndex], new Vector3(transform.position.z + xspawn, Screen.height / 15, 0), transform.rotation, canvas.transform);
        activeMarks.Add(MarkHolder);
        activeMarks[activeMarks.Count-1].currentAmount = firstmark+((activeMarks.Count - 1 )* units);
        activeMarks[activeMarks.Count - 1].SetMark();
        xspawn += Screen.width / (numOfMarks + 2);
    }
    private void moveMarks()
    {

        ballDelta = ball.transform.position.x - ballLastLocation;
        ballLastLocation = ball.transform.position.x;
        for (int i = 0; i < numOfMarks; i++)
        {
            if (activeMarks[i].transform.position.x < (Screen.width * (i + 1)) / (numOfMarks + 2))
            {
                activeMarks[i].transform.position = new Vector3(activeMarks[i].transform.position.x +(Screen.width/(numOfMarks+2)), activeMarks[i].transform.position.y, activeMarks[i].transform.position.z);
                activeMarks[i].currentAmount += units;
                activeMarks[i].SetMark();
            }
            if (activeMarks[i].transform.position.x > (Screen.width * (i + 2)) / (numOfMarks + 2))
            {
                activeMarks[i].transform.position = new Vector3(activeMarks[i].transform.position.x - (Screen.width / (numOfMarks + 2)), activeMarks[i].transform.position.y, activeMarks[i].transform.position.z);
                activeMarks[i].currentAmount -= units;
                activeMarks[i].SetMark();
            }

            if (ballDelta != 0)
            {
                activeMarks[i].transform.position = new Vector3(activeMarks[i].transform.position.x - ((Screen.width * ballDelta) / (units * (numOfMarks + 2))), activeMarks[i].transform.position.y, activeMarks[i].transform.position.z);
            }
        }
    }

    public void drawMarks()
    {
        if (!Initiated)
        {
            if (numOfMarks % 2 == 0)
            {
                firstmark = ((numOfMarks / 2) - 1) * (units * -1f);
            }
            else
            {
                firstmark = (((numOfMarks / 2) - 0.5f) * (units * -1f));
            }

            transform.position = new Vector2(transform.position.x, Screen.height / 15);
            GameObject Distancebar = GameObject.Find("Canvas/Distancebar");
            var DistancebarRectTransform = Distancebar.transform as RectTransform;
            DistancebarRectTransform.sizeDelta = new Vector2((Screen.width / (numOfMarks + 2) * (numOfMarks)), DistancebarRectTransform.sizeDelta.y);
            xspawn = Screen.width / (numOfMarks + 2) * 2;
            for (int i = 0; i < numOfMarks; i++)
            {
                SpawnMark(0);
            }
            draw = true;
            Initiated = true;
        }
    }
}
