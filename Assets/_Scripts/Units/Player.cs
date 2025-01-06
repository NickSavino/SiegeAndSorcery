using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    Rigidbody rigidbody;
    public const float THRUST = 10f;

    public bool healthBarActive;
    
    [SerializeField]
    public GameObject healthBar;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) {
            rigidbody.AddForce(Vector3.forward * THRUST, ForceMode.Force);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rigidbody.AddForce(Vector3.back * THRUST, ForceMode.Force);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rigidbody.AddForce(Vector3.left * THRUST, ForceMode.Force);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rigidbody.AddForce(Vector3.right * THRUST, ForceMode.Force);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            rigidbody.AddForce(Vector3.up * THRUST, ForceMode.Force);
        }

        healthBarActive = healthBar.activeSelf;

        var healthBarPosition = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        healthBar.transform.position = healthBarPosition;
        
    }
}