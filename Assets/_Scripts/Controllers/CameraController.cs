using UnityEngine;

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
    private Camera camera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        translateCamera();
        zoomCameraProjection();
        zoomCameraOrtho();
        rotateCamera();

    }


    void translateCamera()
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
     
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Vector3 temp = camObject.transform.position;
            temp += (forward * CAM_MOVE_SPEED);
            camObject.transform.position = temp;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            Vector3 temp = camObject.transform.position;
            temp += (forward * CAM_MOVE_SPEED * -1);
            camObject.transform.position = temp;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Vector3 temp = camObject.transform.position;
            temp += (right * CAM_MOVE_SPEED * -1);
            camObject.transform.position = temp;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            Vector3 temp = camObject.transform.position;
            temp += (right * CAM_MOVE_SPEED);
            camObject.transform.position = temp;
        }
    }



    void zoomCameraProjection()
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

    void zoomCameraOrtho()
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


    void rotateCamera()
    {
        if (Input.GetMouseButton(2))    // middle click
        {
            float delta = Input.GetAxis("Mouse X") * ROTATION_SENSITIVITY;
            camObject.transform.Rotate(new Vector3(-TILT_ROTATION, 0, 0));
            camObject.transform.Rotate(new Vector3(0, delta, 0));
            camObject.transform.Rotate(new Vector3(TILT_ROTATION, 0, 0));
        }
    }



}

