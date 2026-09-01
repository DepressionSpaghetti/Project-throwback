using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

public static class NativeControllerBridge
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RegisterNativeBridges()
    {
        InputSystem.RegisterLayoutMatcher("DualShock4GamepadHID",
            new InputDeviceMatcher()
                .WithInterface("HID")
                .WithProduct(".*DUALSHOCK.*")
        );

    }
}
