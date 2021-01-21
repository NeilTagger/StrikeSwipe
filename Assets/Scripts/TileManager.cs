using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject[] tilePrefabs;
    public float xspawn = 10;
    public float tileLength = 10;
    public int numOfTiles = 4;
    private int nextToMove = 0;
    public Transform ballTransform;
    private List<GameObject> activeTiles = new List<GameObject>();
    
    
    void Start()
    {

        for (int i = 0; i < numOfTiles; i++)
        {
            SpawnTile(0);
        }
    }

    void Update()
    {
        if (ballTransform.position.x > xspawn - ((numOfTiles-1) * tileLength))
        {
            MoveTile();
        }
    }

    public void SpawnTile(int tileIndex)
    {
        GameObject tileHolder = Instantiate(tilePrefabs[tileIndex], transform.right * xspawn, transform.rotation);
        activeTiles.Add(tileHolder);
        xspawn += tileLength;
    }

    public void MoveTile()
    {
        activeTiles[nextToMove].transform.position += Vector3.right * 50;
        nextToMove= (nextToMove+1) % numOfTiles;
        xspawn += tileLength;
    }
    public void ResetTiles()
    {
        xspawn =0;
        for (int i = 0; i < numOfTiles; i++)
        {
            activeTiles[nextToMove].transform.position=new Vector3(xspawn,transform.position.y,transform.position.z);
            nextToMove = (nextToMove + 1) % numOfTiles;
            xspawn += tileLength;
        }
       
        

    }
}
