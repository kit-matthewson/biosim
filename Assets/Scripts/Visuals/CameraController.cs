using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles camera movement from keyboard inputs.
/// </summary>
public class CameraController : MonoBehaviour {
    public float MoveSpeed;
    public float LookSpeed;

    private Vector2 _horizontalMovement;
    private float _verticalMovement;
    private Vector2 _mouseDelta;

    [PublicAPI]
    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    [PublicAPI]
    private void Update() {
        Vector3 movement = new(_horizontalMovement.x, 0, _horizontalMovement.y);
        movement += transform.InverseTransformDirection(Vector3.up * _verticalMovement);

        transform.Translate(MoveSpeed * Time.deltaTime * movement);
        transform.Rotate(LookSpeed * Time.deltaTime * new Vector3(_mouseDelta.y, _mouseDelta.x, 0));
        transform.rotation =
            Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0); // This shouldn't be needed, but is ??
    }

    public void OnFocusToggle() {
        Cursor.visible = !Cursor.visible;

        Cursor.lockState = Cursor.visible ? CursorLockMode.Confined : CursorLockMode.Locked;
    }

    public void OnHorizontalMovement(InputValue input) {
        _horizontalMovement = input.Get<Vector2>();
    }

    public void OnVerticalMovement(InputValue input) {
        _verticalMovement = input.Get<float>();
    }

    public void OnLook(InputValue input) {
        if (Cursor.lockState == CursorLockMode.Locked) {
            _mouseDelta = input.Get<Vector2>();
        }
    }
}