using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private string nextScene;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Método público para conectar al botón Play en el inspector
    public void PlayGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
    }

}
