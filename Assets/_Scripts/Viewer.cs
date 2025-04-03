using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Viewer : MonoBehaviour {
    CharacterController _cc;
    [SerializeField] Camera _cam;
    [SerializeField] float speed;
    [SerializeField] Quaternion rotation;
    private void Start() {
        _cc = GetComponent<CharacterController>();
    }

    private void Update() {
        Vector3 diff = transform.position - _cam.transform.position;
        Vector3 forward = new Vector3(diff.x, 0f, diff.z);
        transform.rotation = rotation;
        if (Input.GetKey(KeyCode.W)) _cc.Move(speed * Time.deltaTime * forward);
    }
}
