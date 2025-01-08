using UnityEngine;

public class StartupController : MonoBehaviour
{
    public Texture2D texture;
    public CursorMode cursorMode;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector2 offset = new Vector2(texture.width/4, texture.height/2);
        Cursor.SetCursor(texture, offset, cursorMode);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
