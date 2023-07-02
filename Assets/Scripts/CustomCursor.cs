using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D crosshair;

    void Start()
    {
        Vector2 cursorOffset = new Vector2(crosshair.width / 3, crosshair.height / 4f);

        Cursor.SetCursor(crosshair, cursorOffset, CursorMode.Auto);
    }
}
