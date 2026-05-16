using UnityEngine;
using UnityEngine.InputSystem;

public class FollowMouse : MonoBehaviour
{
    public float radius;
    void Start()
    {

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //Vector3 desired = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 screenPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        screenPos.z = 0;

        transform.position = screenPos;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
