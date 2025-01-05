using System;
using UnityEngine;

public class DevCam : MonoBehaviour
{

    [SerializeField]
    private GameObject TERRAIN;

    [SerializeField]
    private float FOCUS_OFFSET;         //20 seems to be good with TILT_ROTATION = 40

    [SerializeField]
    private float CAM_MOVE_SPEED;

    [SerializeField]
    private float CAM_ZOOM_SPEED;

    [SerializeField]
    private float CAM_MAX_FOV;

    [SerializeField]
    private float CAM_MIN_FOV;

    [SerializeField]
    private float ROTATION_SENSITIVITY;

    [SerializeField]
    private float TILT_ROTATION;            // I FOUND 40 is good

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
        zoomCamera();
        rotateCamera();
        panToGameObject();

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


    void rotateCamera()
    {
        if (Input.GetMouseButton(2))    // middle click
        {
            float delta = Input.GetAxis("Mouse X") * ROTATION_SENSITIVITY;
            rotateAlongY(camObject, delta);
        }
    }


    void panToGameObject()
    {

        if (Input.GetMouseButton(1))
        {


            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 objPos = new Vector3();
            if (Physics.Raycast(ray, out hit))
            {
                GameObject obj = hit.collider.gameObject;
                if (obj.name != TERRAIN.name)
                {

                    objPos = obj.transform.position;
                    // vector from camera to target
                    Vector3 distanceVector = objPos - camObject.transform.position;

                    // rotate toward object
                    Vector3 lookRotation = Quaternion.LookRotation(distanceVector).eulerAngles;
                    lookRotation.x = TILT_ROTATION;
                    lookRotation.z = 0f;
                    camObject.transform.rotation = Quaternion.Euler(lookRotation);


                    // move toward object
                    camObject.transform.position = new Vector3(objPos.x, camObject.transform.position.y, objPos.z);  // don't translate along Y axis
                    distanceVector.y = 0f;  // ignore y distance 
                    camObject.transform.position -= distanceVector.normalized * FOCUS_OFFSET;

                    camera.fieldOfView = CAM_MIN_FOV;       // zoom in as much as possible

                }
            }
        }
    }


    void rotateAlongY(GameObject gameObj, float rotation)
    {

        gameObj.transform.Rotate(new Vector3(-TILT_ROTATION, 0, 0));
        gameObj.transform.Rotate(new Vector3(0, rotation, 0));
        gameObj.transform.Rotate(new Vector3(TILT_ROTATION, 0, 0));
    }



}

