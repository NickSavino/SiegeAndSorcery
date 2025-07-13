using UnityEngine;

public class FPSController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private Camera _fpsCamera;

    [SerializeField]
    private Rigidbody _body;

    private float PLAYER_MOVE_SPEED = 1f;
    private Vector3 JUMP_DIST = new Vector3(0f, 10f, 0f);
    private float CAMERA_SENS = 5f;
    private const float MIN_ANGLE = -90f;
    private const float MAX_ANGLE = 90f;
    private float _vRotation = 0f;
    private float _hRotation = 0f;

    void Start()
    {
       // _body.v
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.y, 0));
        ToggleCamera();
        if (_fpsCamera.enabled) {
            MoveCamera();
            JumpPlayer();
        }
    }

    void FixedUpdate() {
        if (_fpsCamera.enabled) {
            MovePlayer();
            JumpPlayer();
          // MoveCamera();
        }
    }


    public void ToggleCamera() {
        if (Input.GetKeyDown(KeyCode.F5)) {
            if (_fpsCamera.enabled) {
                _fpsCamera.enabled = false;
                Cursor.lockState = CursorLockMode.None;
            }
            else {
                _fpsCamera.enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }


    private void MoveCamera() {
        float horizontal = Input.GetAxis("Mouse X");
        float vertical = Input.GetAxis("Mouse Y");
        _hRotation += horizontal * CAMERA_SENS;
        _vRotation -= vertical * CAMERA_SENS;
        _vRotation = Mathf.Clamp(_vRotation, MIN_ANGLE, MAX_ANGLE);
        _fpsCamera.transform.localEulerAngles = new Vector3(_vRotation, _hRotation, 0f);
    }


    private void JumpPlayer() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            _body.AddForce(JUMP_DIST, ForceMode.Impulse);
        }
    }



    private void MovePlayer() {
        float degrees;
        if (_fpsCamera.transform.rotation.eulerAngles.y < 0) {
            degrees = 360 + _fpsCamera.transform.rotation.eulerAngles.y;
        }
        else {
            degrees = _fpsCamera.transform.rotation.eulerAngles.y;
        }

        float radians = degrees * (Mathf.PI / 180);
        float piOverTwo = Mathf.PI / 2;
        //radians += (Mathf.PI / 2);

        Vector3 forward = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians));
        Vector3 right = new Vector3(Mathf.Sin(radians + piOverTwo), 0, Mathf.Cos(radians + piOverTwo));
        Vector3 temp = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) {
            temp += forward;
        }

        if (Input.GetKey(KeyCode.S)) {
            temp -= forward;
        }

        if (Input.GetKey(KeyCode.A)) {
            temp -= right;
        }

        if (Input.GetKey(KeyCode.D)) {
            temp += right;
        }

        // I think unnecessary
        temp = temp.normalized;

        _body.AddForce(temp * PLAYER_MOVE_SPEED, ForceMode.Impulse);
    }
}
