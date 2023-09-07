using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PercentageToScreen
{
    //#####		RETURN THE SIZE AND POSITION for GUI images ##################
    public static float position_x(float var)
    {
        return Screen.width * var / 100;
    }

    public static float position_y(float var)
    {
        return Screen.height * var / 100;
    }

    public static float size_x(float var)
    {
        return Screen.width * var / 100;
    }

    public static float size_y(float var)
    {
        return Screen.height * var / 100;
    }

    public static Vector2 vec2(Vector2 _vec2)
    {
        return new Vector2(Screen.width * _vec2.x / 100, Screen.height * _vec2.y / 100);
    }
    //#
}
