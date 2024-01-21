using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Tool_PositionningTerrain : MonoBehaviour
{
    public bool activeDebugDisplay;
    public bool activeDebugDisplayParent;

    public LayerMask layerGround;
    public Vector3 directionPosition;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(activeDebugDisplay)
        {
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(directionPosition), out hit, Mathf.Infinity, layerGround))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(directionPosition) * hit.distance, Color.yellow);
                Debug.Log("Did Hit");
                transform.position = hit.point;
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(directionPosition) * 1000, Color.white);
                Debug.Log("Did not Hit");
            }
            activeDebugDisplay = false;
        }


        if(activeDebugDisplayParent)
        {
            
            for(int i = 0; i < transform.childCount; i++)
            {
                RaycastHit hit;
                Transform childTransform = transform.GetChild(i);
                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(childTransform.position, directionPosition, out hit, Mathf.Infinity, layerGround))
                {
                    Debug.DrawRay(childTransform.position, directionPosition * hit.distance, Color.yellow);
                    Debug.Log("Did Hit");
                    childTransform.position = hit.point;
                }
            }


            activeDebugDisplayParent = false;
        }

        Debug.DrawRay(transform.position, transform.TransformDirection(directionPosition) * 1000, Color.white);
        Debug.Log("Did not Hit");
    }
}
