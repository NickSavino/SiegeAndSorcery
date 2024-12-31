using UnityEngine;

public class CameraControl : MonoBehaviour
{

    public float CAM_MOVE_SPEED;
    public float CAM_ZOOM_SPEED;

    public float CAM_MAX_FOV;
    public float CAM_MIN_FOV;

    public GameObject camObject;
    public Camera camera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        translateCamera();
        zoomCamera();


    }


    void translateCamera()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Vector3 temp = camObject.transform.position;
            temp += (Vector3.forward * CAM_MOVE_SPEED);
            camObject.transform.position = temp;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            Vector3 temp = camObject.transform.position;
            temp += (Vector3.back * CAM_MOVE_SPEED);
            camObject.transform.position = temp;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Vector3 temp = camObject.transform.position;
            temp += (Vector3.left * CAM_MOVE_SPEED);
            camObject.transform.position = temp;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            Vector3 temp = camObject.transform.position;
            temp += (Vector3.right * CAM_MOVE_SPEED);
            camObject.transform.position = temp;
        }
    }



    void zoomCamera()
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
}