using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDropGeneration : MonoBehaviour
{
    public LayerMask groundLayer;
    [Range(1, 4)]
    public int dropQuantity;
    public int random = 1;
    public float rangeFromCenter = 25;

    private Vector3 raycastdirection;
    public Vector3 m_DropAreaPosition;

    public GameObject[] cristalDropObject;
    // Start is called before the first frame update
    void Start()
    {
        raycastdirection = new Vector3(0, -25, 0);
        int dropToGenerate = dropQuantity + Random.Range(-random, random);


        if(dropToGenerate > 0)
        {
            for(int i = 0; i < dropToGenerate; i++)
            {
                Vector2 rnd = Random.insideUnitCircle * rangeFromCenter;
                int randomCristalType = Random.Range(0, 3);
                RaycastHit hit;
                Vector3 newPosition = transform.position + new Vector3(500 + rnd.x, 100, 500 + rnd.y);
                if (Physics.Raycast(newPosition, raycastdirection * 150, out hit, Mathf.Infinity, groundLayer))
                {
                    Debug.DrawRay(newPosition, raycastdirection * hit.distance, Color.cyan);
                    m_DropAreaPosition = hit.point + new Vector3(0,-5,0);
                    Instantiate(cristalDropObject[randomCristalType], m_DropAreaPosition, transform.rotation);
                    Debug.Log("Did Hit");
                }
                else
                {
                    Debug.DrawRay(newPosition, raycastdirection * 1000, Color.white);
                    Debug.Log("Did not Hit");
                }
            }
        }
    }
    // Update is called once per frame
}