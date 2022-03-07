using System.Collections.Generic;
using UnityEngine;

public static class CustomInput
{
    public const float CONTROLLER_AXIS_THRESHOLD = 0.4f;

    // PS5 Controller
    // X - 1
    // O - 2
    // [] - 0
    // ^ - 3
    // Start - 9
    // Rb - 5
    // Lb - 4

    public const string Start = "start";
    public const string Use = "use";
    public const string Interact = "interact";
    public const string Toggle = "toggle";
    public const string Accept = "accept";
    public const string Cancel = "cancel";
    public const string Rb = "Rb";
    public const string Lb = "Lb";

    readonly static Dictionary<string, KeyCode> Keyboard = new Dictionary<string, KeyCode>()
    {
        { Start, KeyCode.Escape },
        { Use, KeyCode.Mouse0 },
        { Interact, KeyCode.Space},
        { Toggle, KeyCode.E },
        { Accept, KeyCode.Space },
        { Cancel, KeyCode.Backspace },
        { Rb, KeyCode.RightArrow },
        { Lb, KeyCode.LeftArrow }
    };

    readonly static Dictionary<string, string> PS5Controller = new Dictionary<string, string>()
    {
        { Start, "joystick 1 button 9" }, // menu / start button
        { Use, "joystick 1 button 5" }, // Rb
        { Interact, "joystick 1 button 0" }, // Square
        { Toggle, "joystick 1 button 3" }, // Triangle
        { Accept, "joystick 1 button 1" }, // X
        { Cancel, "joystick 1 button 2" }, // Circle
        { Rb, "joystick 1 button 5"},
        { Lb, "joystick 1 button 4" }
    };

    public static bool IsController() => Input.GetJoystickNames().Length > 0;

    public static bool GetKey(string key)
    {
        if (IsController())
            return Input.GetKey(PS5Controller[key]);

        return Input.GetKey(Keyboard[key]);
    }

    public static bool GetKeyDown(string key)
    {
        if (IsController())
            return Input.GetKeyDown(PS5Controller[key]);

        return Input.GetKeyDown(Keyboard[key]);
    }

    public static bool GetKeyUp(string key)
    {
        if (IsController())
            return Input.GetKeyUp(PS5Controller[key]);

        return Input.GetKeyUp(Keyboard[key]);
    }

    public static float GetAxis(string axis) => Input.GetAxis(axis);

    public static void PrintActiveButtons()
    {
        for (var i = 0; i < 20; ++i)
        {
            try
            {
                if (Input.GetKey($"joystick 1 button {i}"))
                {
                    Debug.Log($"joystick 1 button {i}");
                }
            }
            catch { }
        }
    }
}
