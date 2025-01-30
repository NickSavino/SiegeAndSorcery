using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float CAM_MOVE_SPEED;

    [SerializeField]
    private float CAM_ZOOM_SPEED;

    [SerializeField]
    private float CAM_MAX_FOV;

    [SerializeField]
    private float CAM_MIN_FOV;

    [SerializeField]
    private float ORTHO_MAX_SIZE;

    [SerializeField]
    private float ORTHO_MIN_SIZE;

    [SerializeField]
    private float ROTATION_SENSITIVITY;

    [SerializeField]
    private float TILT_ROTATION;

    [SerializeField]
    private GameObject camObject;

    [SerializeField]
    private new Camera camera;

    private float DEFAULT_BOX_SCALE = 0f;

    private bool _shiftAcceleration;

    private float _baseMoveSpeed;

    private GameObject _dragBox;
    private Vector2 _dragBoxAnchor;

    private List<UnitController> _units;

    private float DRAGBOX_DEPTH = 1000f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _baseMoveSpeed = CAM_MOVE_SPEED;
        _dragBox = transform.Find("Canvas").Find("Image").gameObject;       // very bad!
        _units = new List<UnitController>();

    }

    // Update is called once per frame
    void Update() {
        ClearUnitsOnClick();
        SelectUnitClick();
        SetUnitsDestination();
        TranslateCamera();
        ZoomCameraProjection();
        ZoomCameraOrtho();
        RotateCamera();
        ShiftAccelerateCamera();
        DrawDragBox();
        UpdateDragBoxSize();

    }


    private void OnGUI() {
    //    GUI.Label(new Rect(10, 10, 200, 200),"HELOOOOOOOOOOOOOOOOO");
    }


    void TranslateCamera()
    {
        float degrees;
        if (camObject.transform.rotation.eulerAngles.y < 0)
        {
            degrees = 360 + camObject.transform.rotation.eulerAngles.y;
        }
        else
        {
            degrees = camObject.transform.rotation.eulerAngles.y;
        }

        float radians = degrees * (Mathf.PI / 180);
        float piOverTwo = Mathf.PI / 2;
        //radians += (Mathf.PI / 2);
        
        Vector3 forward = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians));
        Vector3 right = new Vector3(Mathf.Sin(radians + piOverTwo), 0, Mathf.Cos(radians + piOverTwo));
     
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 temp = camObject.transform.position;
            temp += (forward * CAM_MOVE_SPEED);
            camObject.transform.position = temp;
        }

        if (Input.GetKey(KeyCode.S))
        {
            Vector3 temp = camObject.transform.position;
            temp += (forward * CAM_MOVE_SPEED * -1);
            camObject.transform.position = temp;
        }

        if (Input.GetKey(KeyCode.A))
        {
            Vector3 temp = camObject.transform.position;
            temp += (right * CAM_MOVE_SPEED * -1);
            camObject.transform.position = temp;
        }

        if (Input.GetKey(KeyCode.D))
        {
            Vector3 temp = camObject.transform.position;
            temp += (right * CAM_MOVE_SPEED);
            camObject.transform.position = temp;
        }
    }



    void ZoomCameraProjection()
    {

        Vector2 scrollDelta = Input.mouseScrollDelta;
        if (scrollDelta.y > 0 && camera.fieldOfView > CAM_MIN_FOV)
        {
            if (camera.fieldOfView - CAM_ZOOM_SPEED < CAM_MIN_FOV)
            {
                camera.fieldOfView = CAM_MIN_FOV;
            }
            else
            {
                camera.fieldOfView -= CAM_ZOOM_SPEED;
            }
        }
        else if (scrollDelta.y < 0 && camera.fieldOfView < CAM_MAX_FOV)
        {
            if (camera.fieldOfView - CAM_ZOOM_SPEED > CAM_MIN_FOV)
            {
                camera.fieldOfView = CAM_MAX_FOV;
            }
            else
            {
                camera.fieldOfView += CAM_ZOOM_SPEED;
            }
        }
    }

    void ZoomCameraOrtho()
    {

        Vector2 scrollDelta = Input.mouseScrollDelta;
        if (scrollDelta.y > 0 && camera.orthographicSize > ORTHO_MIN_SIZE)
        {
            if (camera.orthographicSize - CAM_ZOOM_SPEED < ORTHO_MIN_SIZE)
            {
                camera.orthographicSize = ORTHO_MIN_SIZE;
            }
            else
            {
                camera.orthographicSize -= CAM_ZOOM_SPEED;
            }
        }
        else if (scrollDelta.y < 0 && camera.orthographicSize < ORTHO_MAX_SIZE)
        {
            if (camera.orthographicSize - CAM_ZOOM_SPEED > ORTHO_MIN_SIZE)
            {
                camera.orthographicSize = ORTHO_MAX_SIZE;
            }
            else
            {
                camera.orthographicSize += CAM_ZOOM_SPEED;
            }
        }
    }


    void RotateCamera()
    {
        if (Input.GetMouseButton(2))    // middle click
        {
            float delta = Input.GetAxis("Mouse X") * ROTATION_SENSITIVITY;
            camObject.transform.Rotate(new Vector3(-TILT_ROTATION, 0, 0));
            camObject.transform.Rotate(new Vector3(0, delta, 0));
            camObject.transform.Rotate(new Vector3(TILT_ROTATION, 0, 0));
        }
    }

    void ShiftAccelerateCamera()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            CAM_MOVE_SPEED = _baseMoveSpeed * 5;
        }
        else
        {
            CAM_MOVE_SPEED = _baseMoveSpeed;
        }
    }

    void DrawDragBox() {
        if (Input.GetMouseButton(0) && !_dragBox.activeSelf) {
            _dragBoxAnchor = Input.mousePosition;
            _dragBox.SetActive(true);
            _dragBox.transform.position = new Vector3(_dragBoxAnchor.x - (DEFAULT_BOX_SCALE / 2), _dragBoxAnchor.y + (DEFAULT_BOX_SCALE / 2), -10); // bottom right corner


        }
        else if (!Input.GetMouseButton(0) && _dragBox.activeSelf) {
            _dragBox.SetActive(false);
        }
    }
    

    void UpdateDragBoxSize() {
        if (_dragBox.activeSelf) {
            if (Input.GetMouseButton(0)) {
                Vector2 mousePos = Input.mousePosition;
                float xScale = mousePos.x - _dragBoxAnchor.x;
                float yScale =  mousePos.y - _dragBoxAnchor.y;

                _dragBox.transform.localScale = new Vector3(xScale, yScale, _dragBox.transform.localScale.z);

            }
        }
    }


    void SelectUnitClick() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {   // _currentPosts[0] is initial post
                hit.collider.gameObject.TryGetComponent<UnitController>(out UnitController unitHit);
                if (unitHit != null) {
                    unitHit._isSelected = true;
                    _units.Add(unitHit);
                }
            }
        }
    }

    void SelectUnitDrag() {
        if (Input.GetMouseButtonDown(1) && _dragBox.activeSelf) {
            _dragBox.TryGetComponent<RectTransform>(out RectTransform boxTransform);
            Vector3[] corners = new Vector3[4];
            boxTransform.GetWorldCorners(corners);

        }
    }

    void ClearUnitsOnClick() {
        if (Input.GetMouseButtonDown(0) && _units.Count > 0) {
            foreach (UnitController cont in _units) {
                cont._isSelected = false;
            }
        }    
    }

    void SetUnitsDestination() {
        if (Input.GetMouseButtonDown(1) && _units.Count > 0) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {   // _currentPosts[0] is initial post
                Vector3 destination = hit.point;
                destination.y = _units[0].gameObject.transform.position.y;  // keep y axis unaffected
                foreach (UnitController cont in _units) {
                    cont.SetNavDestination(destination);
                }
            }
        }
    }
}

