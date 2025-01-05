using UnityEngine;

public class Billboard : MonoBehaviour
{

    [SerializeField]
    GameObject camObject;

    [SerializeField]
    Camera camera;
    void Update()
    {
        Vector3 direction = camObject.transform.position - transform.position;
        Vector3 lookRotation = Quaternion.LookRotation(direction).eulerAngles;
        transform.rotation = Quaternion.Euler(lookRotation);
        GetComponent<Rigidbody>().freezeRotation = true;



        Animator anim = GetComponent<Animator>();
       // anim.trig
    }
}