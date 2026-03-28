using LethalCompanyInputUtils.Api;
using LethalCompanyInputUtils.BindingPathEnums;
using UnityEngine.InputSystem;

public class GameBoyInput : LcInputActions
{
    [InputAction(KeyboardControl.E, Name = "Load Cartridge", GamepadPath = "")]
    public InputAction LoadCartridgeKey { get; set; }

    [InputAction(KeyboardControl.Period, Name = "Volume Up", GamepadPath = "<Gamepad>/dpad/up")]
    public InputAction VolumeUpKey { get; set; }

    [InputAction(KeyboardControl.Comma, Name = "Volume Down", GamepadPath = "<Gamepad>/dpad/down")]
    public InputAction VolumeDownKey { get; set; }
}
