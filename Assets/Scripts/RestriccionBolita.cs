using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestriccionBolita : MonoBehaviour
{    
    [SerializeField] private float radius;
    public FollowMouse cursor;
    void Awake()
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
    }
}
