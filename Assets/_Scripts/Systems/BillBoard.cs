using UnityEngine;

public class Billboard : MonoBehaviour
{


    void Start()
    {
        GetComponent<Rigidbody>().freezeRotation = true;
    }

    void Update()
    {
        Vector3 direction = Camera.main.transform.position - transform.position;
        Vector3 lookRotation = Quaternion.LookRotation(direction).eulerAngles;
        transform.rotation = Quaternion.Euler(lookRotation);



        Animator anim = GetComponent<Animator>();
       // anim.trig
    }
}