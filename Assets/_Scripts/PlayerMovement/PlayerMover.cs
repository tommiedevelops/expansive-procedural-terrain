using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMover : MonoBehaviour
{
    private float speed = 10f;
    private Transform cameraTransform;

    void Start()
    {
        if (Camera.main != null) cameraTransform = Camera.main.transform;
    }
    void Update()
    {
        var moveDir = (transform.position - cameraTransform.position).normalized;
        moveDir.y = 0f;
        moveDir = moveDir.normalized;
        
        if (Input.GetKey(KeyCode.W))
            transform.position += moveDir * (speed * Time.deltaTime);
    }

}
