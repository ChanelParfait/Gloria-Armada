using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FormationPoint : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update

    public GameObject points;
    [SerializeField] GameObject[] pointPositions;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(65, 0, 0);
        pointPositions = new GameObject[points.transform.childCount];
        for (int i = 0; i < points.transform.childCount; i++)
        {
            
            pointPositions[i] = points.transform.GetChild(i).gameObject;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "LevelTrigger" && pointPositions.Contains(other.gameObject))
        {
            //Debug.Log("Hit Point: " + other.gameObject.name); 
            //If this is the last point, go right at 50
            if (pointPositions[pointPositions.Length - 1] == other.gameObject)
            {
                rb.velocity = new Vector3(50, 0, 0);
            }
            else
            {
                //Get the next point
                int index = Array.IndexOf(pointPositions, other.gameObject);
                //Debug.Log("Current index: " + index + " Length: " + pointPositions.Length);
                GameObject nextPoint = pointPositions[index + 1];
                //Debug.Log("Going to:" + nextPoint.name);
                //Get the direction to the next point
                Vector3 direction = nextPoint.transform.position - transform.position;
                //Set the velocity to the direction
                rb.velocity = direction.normalized * 58;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
