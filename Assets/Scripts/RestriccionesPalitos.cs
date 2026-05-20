using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestriccionesPalitos : RestriccionBolita
{
    public GameObject segmentPrefab;
    public int segmentCount;
    public List<Transform> segments = new List<Transform>();
    void Start()
    {
        for (int i = 0; i < segmentCount; i++)
        {
            GameObject segment = Instantiate(segmentPrefab, transform.position, Quaternion.identity);
            segments.Add(segment.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = cursor.transform.position;
        segments[0].position = ConstraintDistance(segments[0].position, mousePos, radius);
        Vector3 dir = mousePos - segments[0].position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        segments[0].rotation = Quaternion.Euler(0, 0, angle - 90);
        for (int i = 1; i < segments.Count; i++)
        {
            RotateTowardsPrevious(i);
            segments[i].position = ConstraintDistance(segments[i].position, segments[i - 1].position, radius);
        }
    }

    private void RotateTowardsPrevious(int i)
    {
        Vector3 dir = segments[i - 1].position - segments[i].position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        segments[i].rotation = Quaternion.Euler(0, 0, angle- 90);
    }
}
