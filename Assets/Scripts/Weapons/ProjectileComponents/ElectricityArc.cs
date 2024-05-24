using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ElectricityArc : MonoBehaviour
{
    public Transform shooter;
    public Transform target;
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
        if (target == null){
            if (lineRenderer != null){
                lineRenderer.enabled = false;
            }
            
            return;
        }
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
                GenerateArcPoints();
                lineRenderer.SetPositions(arcPoints.ToArray());
            }
            yield return new WaitForSeconds(updateFrequency);
        }
    }

    void GenerateArcPoints()
    {
        arcPoints.Clear();
        for (int i = 0; i < segmentCount; i++)
        {
            float t = (float)i / (segmentCount - 1);
            Vector3 point = Vector3.Lerp(shooter.position, target.position, t);
            float adjArcSize = i % 5 == 0 ? arcHeight * 3 : arcHeight;
            point.y += Random.Range(-adjArcSize, adjArcSize);
            point.x += Random.Range(-adjArcSize, adjArcSize);
            point.z += Random.Range(-adjArcSize, adjArcSize);
            arcPoints.Add(point);
        }
    }
}
