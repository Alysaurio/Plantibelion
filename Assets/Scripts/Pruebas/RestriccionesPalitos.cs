using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IkMode
{
    none,
    gusanito,
    FABRIK,
}

public class RestriccionesPalitos : RestriccionBolita
{
    public IkMode mode;

    public GameObject segmentPrefab;
    public int segmentCount;
    public List<Transform> segments = new List<Transform>();
    public Transform ancla;
    private Vector3 mousePos;
    void Start()
    {
        for (int i = 0; i < segmentCount; i++)
        {
            GameObject segment = Instantiate(segmentPrefab, transform.position, Quaternion.identity);
            segments.Add(segment.transform);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        mousePos = cursor.transform.position;
        switch (mode)
        {
            case IkMode.gusanito:
                IkForward();
                break;
            case IkMode.FABRIK:                
                IkForward();
                IkBackward();
                break;
        }

    }

    private void IkForward()
    {
        // FORWARD
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
    private void IkBackward()
    {
        // BACKWARD
        segments[segments.Count - 1].position = ancla.position;
        for (int i = segments.Count - 1; i > 0; i--)
        {
            RotateTowardsPrevious(i);
            segments[i - 1].position = ConstraintDistance(segments[i - 1].position, segments[i].position, radius);
        }
    }

    private void RotateTowardsPrevious(int i)
    {
        Vector3 dir = segments[i - 1].position - segments[i].position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        segments[i].rotation = Quaternion.Euler(0, 0, angle- 90);
    }
}
