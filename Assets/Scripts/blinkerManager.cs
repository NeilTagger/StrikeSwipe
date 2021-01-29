using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class blinkerManager : MonoBehaviour
{
    Image ArrowImage;

    float time = 0.5f;
    float currTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currTime += Time.deltaTime;

        if (currTime > time)
        {
            currTime = 0;
            ArrowImage.enabled = !ArrowImage.enabled;
        }
    }
}
