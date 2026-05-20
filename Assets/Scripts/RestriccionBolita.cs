using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestriccionBolita : MonoBehaviour
{    
    public float radius;
    public FollowMouse cursor;
    /* void Awake()
    {
        if (cursor != null)
            radius = cursor.radius;
        else
            Debug.LogError("El cursor no está asignado en RestriccionBolita");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direccion = transform.position - cursor.transform.position;
        if (direccion.magnitude > radius)
        {
            direccion = direccion.normalized * radius;
            transform.position = cursor.transform.position + direccion;
        }
    } */
    void Awake()
    {
        if (cursor != null)
            radius = cursor.radius;
        else
            Debug.LogError("El cursor no está asignado en RestriccionBolita");
    }
    void Update()
    {
        Vector3 direccion= transform.position - cursor.transform.position;
        if (direccion.magnitude > radius)
            transform.position = ConstraintDistance(transform.position, cursor.transform.position, radius);
    }
    public Vector3 ConstraintDistance(Vector3 point, Vector3 anchor, float distance)
    {
        return ((point - anchor).normalized * distance) + anchor;
    }
}
