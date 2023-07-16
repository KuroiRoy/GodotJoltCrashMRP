using System.Collections.Generic;
using Godot;

namespace GodotJoltCrashMRP.Scripts.Players; 

public class InputHelper {

    private static Dictionary<string, GodotObject> claimingObjectsByInputAction = new();

    public static bool IsClaimed (ClaimableInput input) => IsClaimed($"{nameof(ClaimableInput)}.{input}");
    public static bool IsClaimed (string action) => claimingObjectsByInputAction.ContainsKey(action);
    
    public static bool HasExclusiveInput (ClaimableInput input, GodotObject claimer) => HasExclusiveInput($"{nameof(ClaimableInput)}.{input}", claimer);
    public static bool HasExclusiveInput (string action, GodotObject claimer) => claimingObjectsByInputAction.TryGetValue(action, out var claimingObject) && claimingObject == claimer;

    public static void ReleaseExclusiveInput (ClaimableInput input, GodotObject claimer) => ReleaseExclusiveInput($"{nameof(ClaimableInput)}.{input}", claimer);
    public static void ReleaseExclusiveInput (string action, GodotObject claimer) {
        if (claimingObjectsByInputAction.TryGetValue(action, out var claimingObject) && claimer != claimingObject) GD.PushWarning($"Input {action} was not claimed by {claimer}! It was claimed by {claimingObject}");
        claimingObjectsByInputAction.Remove(action);
    }

    public static void TakeExclusiveInput (ClaimableInput input, GodotObject claimer) => TakeExclusiveInput($"{nameof(ClaimableInput)}.{input}", claimer);
    public static void TakeExclusiveInput (string action, GodotObject claimer) => claimingObjectsByInputAction.TryAdd(action, claimer);

    public enum ClaimableInput {

        MouseMovement

    }

}