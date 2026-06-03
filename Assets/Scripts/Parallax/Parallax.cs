using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField]
    private Transform targetCamera;
    [SerializeField, Range(0, 1)]
    private float efectoParallax;
    [SerializeField]
    private float longitudSprite;
    private float posicionInicial;
    void Start()
    {
        posicionInicial = transform.position.x;
    }
    private void LateUpdate()
    {
        float distanciaParallax = targetCamera.position.x * efectoParallax;
        transform.position = new Vector3(posicionInicial + distanciaParallax, transform.position.y, transform.position.z);
        float distanciaTemporal = targetCamera.position.x * (1 - efectoParallax);

        if (distanciaTemporal > posicionInicial + longitudSprite)
        {
            posicionInicial += longitudSprite;
        }
        else if (distanciaTemporal < posicionInicial - longitudSprite)
        {
            posicionInicial -= longitudSprite;
        }
    }
}