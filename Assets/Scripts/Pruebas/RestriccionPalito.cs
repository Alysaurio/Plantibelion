using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestriccionPalito : MonoBehaviour
{
    public Transform target;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direccion = target.position - transform.position;
        float angle = Mathf.Atan2(direccion.y, direccion.x)*Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }
}
