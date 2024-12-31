using UnityEngine;
using System.Collections;





public class WallObject : MonoBehaviour
{

    private GameObject[] _posts;
    private GameObject[] _panels;

    [SerializeField]
    private int MAX_POSTS;

    [SerializeField]
    private int MAX_PANELS;

    [SerializeField]
    private float MAX_HEALTH;


    private float _currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentHealth = MAX_HEALTH;
        _posts = new GameObject[MAX_POSTS];
        _panels = new GameObject[MAX_PANELS];
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    }


public class WallManager : MonoBehaviour
{

    private WallObject[] walls;

    [SerializeField]
    private float MAX_HEALTH;
    private float currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = MAX_HEALTH;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
