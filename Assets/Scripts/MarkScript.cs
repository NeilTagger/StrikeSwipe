using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarkScript : MonoBehaviour
{
    public Text text;
    public float currentAmount;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SetMark()
    {
        text.text = currentAmount.ToString();
    }
   

    // Update is called once per frame
    void Update()
    {
        
    }
}
