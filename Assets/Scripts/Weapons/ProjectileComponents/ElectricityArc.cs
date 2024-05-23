using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class ElectricityArc : MonoBehaviour
{
    public Transform shooter;
    public Transform target;
    public Transform startPoint;
    public Transform endPoint;
    public int segmentCount = 20;
    public float arcHeight = 2;
    public float updateFrequency = 0.05f;

    private LineRenderer lineRenderer;
    private List<Vector3> arcPoints = new List<Vector3>();

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segmentCount;
        StartCoroutine(UpdateArc());
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
    public void SetShooter(Transform shooter)
    {
        this.shooter = shooter;
    }

    IEnumerator UpdateArc()
    {
        

        while (true)
        {
            if (target != null && shooter != null)
            {
                startPoint.position = shooter.position;
                endPoint.position = target.position;
            }
            else if (target != null && shooter == null)
            {
                startPoint.position = transform.position;
                endPoint.position = target.position;

            }
            else if (target == null && shooter != null)
            {
                startPoint.position = shooter.position;
                endPoint.position = transform.position;
            }
            else if (target == null && shooter == null)
            {
                startPoint.position = transform.position;
                endPoint.position = transform.position;
            }
            GenerateArcPoints();
            lineRenderer.SetPositions(arcPoints.ToArray());
            yield return new WaitForSeconds(updateFrequency);
        }
    }

    void GenerateArcPoints()
    {
        arcPoints.Clear();
        for (int i = 0; i < segmentCount; i++)
        {
            float t = (float)i / (segmentCount - 1);
            Vector3 point = Vector3.Lerp(startPoint.position, endPoint.position, t);
            // 1 out of every 10 points will have a scale factor of 8
            float adjArcSize = i % 5 == 0 ? arcHeight * 3 : arcHeight;
            point.y += Random.Range(-adjArcSize, adjArcSize); // Simple arc with sine wave
            point.x += Random.Range(-adjArcSize, adjArcSize); // Small random variation
            point.z += Random.Range(-adjArcSize, adjArcSize); // Small random variation
            arcPoints.Add(point);
        }
    }
}