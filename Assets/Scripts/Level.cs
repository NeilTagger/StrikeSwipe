using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/New Level", order = 1)]
public class Level : ScriptableObject
{
    public bool RequiresDistance, RequiresHeight;

    public float DistanceMeasurement, HeightMeasurement;
}
