using UnityEngine;

public class Billboard : MonoBehaviour
{


    void Start()
    {

    }

    void Update()
    {
        // Vector3 direction = Camera.main.transform.position - transform.position;
        // Vector3 lookRotation = Quaternion.LookRotation(direction).eulerAngles;
        // transform.rotation = Quaternion.Euler(lookRotation);

        transform.forward = -Camera.main.transform.forward;

        Animator anim = GetComponent<Animator>();
       // anim.trig
    }
}