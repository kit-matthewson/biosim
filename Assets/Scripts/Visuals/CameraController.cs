using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour {

    public float moveSpeed;
    public float lookSpeed;

    private Vector2 horizontalMovement = new();
    private float verticalMovement = 0;
    private Vector2 mouseDelta = new();

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnHorizontalMovement(InputValue input) {
        horizontalMovement = input.Get<Vector2>();
    }

    public void OnVerticalMovement(InputValue input) {
        verticalMovement = input.Get<float>();
    }

    public void OnLook(InputValue input) {
        mouseDelta = input.Get<Vector2>();
    }
    private void Update() {
        Vector3 movement = new(horizontalMovement.x, 0, horizontalMovement.y);
        movement += transform.InverseTransformDirection(Vector3.up * verticalMovement);

        transform.Translate(moveSpeed * Time.deltaTime * movement);
        // annoying but z should always be 0 (tried world space) confusing should be better way
        transform.Rotate(lookSpeed * Time.deltaTime * new Vector3(mouseDelta.y, mouseDelta.x, 0), Space.Self);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
    }
}
