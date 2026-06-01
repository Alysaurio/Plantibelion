using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RestriccionBolita : MonoBehaviour
{   
    public float radius;
    public float bolitaRadius = 0.5f;
    public FollowMouse cursor;
    private Vector3 direccion;

    public GameObject bolitaPrefab;
    public int maxBolitas = 20;
    public List<Transform> bolitas = new List<Transform>();
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
    void Start()
    {
        GenerarBolitas();
    }
    void Awake()
    {
        if (cursor != null)
            radius = cursor.radio;
        else
            Debug.LogError("El cursor no está asignado en RestriccionBolita");
    }
    void Update()
    {
        direccion= transform.position - cursor.transform.position;
        BolaFuera();
        BolaDentro();
    }

    private void BolaFuera()
    {
        SeparardelRaton();
        SepararEntreBolitas();
    }

    private void BolaDentro()
    {
        if (direccion.magnitude > radius)
            transform.position = ConstraintDistance(transform.position, cursor.transform.position, radius);
    }


    public Vector3 ConstraintDistance(Vector3 point, Vector3 anchor, float distance)
    {
        return ((point - anchor).normalized * distance) + anchor;
    }

    private void SeparardelRaton()
    {
        float combinedRadius = cursor.radio + bolitaRadius;
        foreach (Transform bolita in bolitas)
        {
            Vector3 desplazamiento = bolita.position - cursor.transform.position;
            if (desplazamiento.magnitude < combinedRadius)
            {
                bolita.position = ConstraintDistance(bolita.position, cursor.transform.position, combinedRadius);
            }
        }
    }
    private void SepararEntreBolitas()
    {
        for (int i = 0;i < bolitas.Count; i++)
        {
            for (int j = i + 1; j < bolitas.Count; j++)
            {
                Vector3 desplazamiento = bolitas[i].position - bolitas[j].position;
                float minDistance = bolitaRadius * 2f;
                if (desplazamiento.magnitude < minDistance)
                {
                    Vector3 temporal = ConstraintDistance(bolitas[i].position, bolitas[j].position, bolitaRadius);
                    bolitas[j].position = ConstraintDistance(bolitas[j].position, bolitas[i].position, bolitaRadius);
                    bolitas[i].position = temporal;
                }
            }
        }
    }

    private void GenerarBolitas()
    {
        for (int i = 0; i < maxBolitas; i++)
        {
            Vector2 randomPos = Random.insideUnitCircle * radius;
            GameObject bolita = Instantiate(bolitaPrefab, transform.position + (Vector3)randomPos, Quaternion.identity);
            bolitas.Add(bolita.transform);
        }
    }
}
