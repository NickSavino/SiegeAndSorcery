using UnityEngine;

public class DefenderUIController : MonoBehaviour
{

    [SerializeField]
    public WallBuilding wallBuilder;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void OnWallButtonClick()
    {
        wallBuilder.toggleActive();
    }

    public void OnTowerButtonClick()
    {

    }
}
