using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightBarScript : MonoBehaviour
{
    public MarkScript[] scaleMarks;
    private List<MarkScript> activeMarks = new List<MarkScript>();
    private float yspawn;
    public int numOfMarks = 3;
    public GameObject canvas;
    public GameObject ball;
    private float ballLastLocation = 0;
    private float ballDelta = 0;
    public float units = 50f;
    private float firstmark = 0;

    void Start()
    {
        if (numOfMarks % 2 == 0)
        {
            firstmark = ((numOfMarks / 2) - 1) * (units * -1f);
        }
        else
        {
            firstmark = (((numOfMarks / 2) - 0.5f) * (units * -1f));
        }

        transform.position = new Vector2(Screen.width / 15, transform.position.y);
        GameObject Distancebar = GameObject.Find("Canvas/Heightbar");
        var DistancebarRectTransform = Distancebar.transform as RectTransform;
        DistancebarRectTransform.sizeDelta = new Vector2(DistancebarRectTransform.sizeDelta.x,(Screen.height / (numOfMarks + 2) * (numOfMarks)));
        yspawn = Screen.height / (numOfMarks + 2) * 2;
        for (int i = 0; i < numOfMarks; i++)
        {
            SpawnMark(0);
        }
    }

 
    void Update()
    {
        moveMarks();
    }
    public void SpawnMark(int markIndex)
    {
        MarkScript MarkHolder = Instantiate(scaleMarks[markIndex], new Vector3( Screen.width / 15, transform.position.z + yspawn, 0), transform.rotation, canvas.transform);
        activeMarks.Add(MarkHolder);
        activeMarks[activeMarks.Count - 1].currentAmount = firstmark + ((activeMarks.Count - 1) * units);
        activeMarks[activeMarks.Count - 1].SetMark();
        yspawn += Screen.height / (numOfMarks + 2);
    }
    private void moveMarks()
    {

        ballDelta = ball.transform.position.y - ballLastLocation;
        ballLastLocation = ball.transform.position.y;
        for (int i = 0; i < numOfMarks; i++)
        {
            if (activeMarks[i].transform.position.y < (Screen.height * (i + 1)) / (numOfMarks + 2))
            {
                activeMarks[i].transform.position = new Vector3(activeMarks[i].transform.position.x, activeMarks[i].transform.position.y + (Screen.height / (numOfMarks + 2)), activeMarks[i].transform.position.z);
                activeMarks[i].currentAmount += units;
                activeMarks[i].SetMark();
            }
            if (activeMarks[i].transform.position.y > (Screen.height * (i + 2)) / (numOfMarks + 2))
            {
                activeMarks[i].transform.position = new Vector3(activeMarks[i].transform.position.x, activeMarks[i].transform.position.y - (Screen.height / (numOfMarks + 2)), activeMarks[i].transform.position.z);
                activeMarks[i].currentAmount -= units;
                activeMarks[i].SetMark();
            }

            if (ballDelta != 0)
            {
                activeMarks[i].transform.position = new Vector3( activeMarks[i].transform.position.x, activeMarks[i].transform.position.y - ((Screen.height * ballDelta) / (units * (numOfMarks + 2))), activeMarks[i].transform.position.z);
            }
        }
    }
}
