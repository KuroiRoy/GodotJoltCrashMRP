using Godot;

namespace GodotJoltCrashMRP.Scripts.Players; 

public partial class FirstPersonCamera : Camera3D {

    [Export] private float mouseSensitivity = 0.1f;
    [Export] private Vector2 pitchRange = new(-80, 85);

    private float yaw;
    private float pitch;

    public override void _Input (InputEvent @event) {
        if (@event is InputEventKey { Keycode: Key.Escape } && Input.MouseMode is not Input.MouseModeEnum.Visible) {
            Input.MouseMode = Input.MouseModeEnum.Visible;
            return;
        }

        if (Input.MouseMode == Input.MouseModeEnum.Visible && @event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true }) Input.MouseMode = Input.MouseModeEnum.Captured;
        if (@event is not InputEventMouseMotion mouseMotionEvent || Input.MouseMode is not Input.MouseModeEnum.Captured) return;
        if (InputHelper.IsClaimed(InputHelper.ClaimableInput.MouseMovement)) return;

        var mouseDelta = mouseMotionEvent.Relative * mouseSensitivity;
        yaw -= mouseDelta.X;
        pitch = Mathf.Clamp(pitch - mouseDelta.Y, pitchRange.X, pitchRange.Y);

        RotationDegrees = new Vector3(pitch, yaw, 0);
    }

}