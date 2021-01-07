using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceMeterScript : MonoBehaviour
{
    public GameObject[] scaleMarks;
    private List<GameObject> activeMarks = new List<GameObject>();
    private float xspawn;
    public int numOfMarks = 3;
    public GameObject canvas;
    public GameObject ball;
    private float ballLastLocation = 0;
    private float ballDelta = 0;
    public float units = 50f;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector2(transform.position.x, Screen.height / 15);
        GameObject Distancebar = GameObject.Find("Canvas/Distancebar");
        var DistancebarRectTransform = Distancebar.transform as RectTransform;
        DistancebarRectTransform.sizeDelta = new Vector2((Screen.width/(numOfMarks+2)*(numOfMarks)), DistancebarRectTransform.sizeDelta.y);
        xspawn = Screen.width / (numOfMarks + 2)*2;
        for (int i = 0; i < numOfMarks; i++)
        {
            SpawnMark(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        moveMarks();
    }
    public void SpawnMark(int markIndex)
    {
        Debug.Log("spawned");
        GameObject MarkHolder = Instantiate(scaleMarks[markIndex],new Vector3(transform.position.z + xspawn, Screen.height/15,0), transform.rotation, canvas.transform);
        activeMarks.Add(MarkHolder);
        xspawn += Screen.width / (numOfMarks + 2);
    }
    private void moveMarks()
    {

        ballDelta = ball.transform.position.x - ballLastLocation;
        while (ballDelta>Screen.width/(numOfMarks+2))
        {
            ballDelta -= Screen.width / (numOfMarks + 2);
        }
        ballLastLocation = ball.transform.position.x;
        for (int i = 0; i < numOfMarks; i++)
        {
            if (activeMarks[i].transform.position.x < (Screen.width * (i + 1)) / (numOfMarks + 2))
            {
                activeMarks[i].transform.position = new Vector3(activeMarks[i].transform.position.x +(Screen.width/(numOfMarks+2)), activeMarks[i].transform.position.y, activeMarks[i].transform.position.z);
            }

            if (ballDelta != 0)
            {
                activeMarks[i].transform.position = new Vector3(activeMarks[i].transform.position.x - ((Screen.width * ballDelta) / (units * (numOfMarks + 2))), activeMarks[i].transform.position.y, activeMarks[i].transform.position.z);
            }
        }
    }
}
