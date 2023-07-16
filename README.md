# GodotJoltCrashMRP
Minimum reproducible project of a crash I am experiencing with Godot-Jolt.

The issue occurs immediately when starting the Game.tscn scene. Godot gives no debug information whatsoever about the crash. Fortunately Rider does. 
On [line 127 of KinematicCharacterController.cs](https://github.com/KuroiRoy/GodotJoltCrashMRP/blob/ef8a18b8551f2797fa16c5349049dd91e3d3a265/Scripts/Players/KinematicCharacterController.cs#L127) the following error occurs:
```
Fatal error. System.AccessViolationException: Attempted to read or write protected memory. This is often an indication that other memory is corrupt.
   at Godot.NativeInterop.NativeFuncs.godotsharp_method_bind_ptrcall(IntPtr, IntPtr, Void**, Void*)
   at Godot.NativeCalls.godot_icall_3_749(IntPtr, IntPtr, Godot.Rid, IntPtr, IntPtr)
   at Godot.PhysicsServer3D.BodyTestMotion(Godot.Rid, Godot.PhysicsTestMotionParameters3D, Godot.PhysicsTestMotionResult3D)
   at GodotJoltCrashMRP.Scripts.Players.KinematicCharacterController.StepUp(Single)
   at GodotJoltCrashMRP.Scripts.Players.KinematicCharacterController._PhysicsProcess(Double)
   at Godot.Node.InvokeGodotClassMethod(Godot.NativeInterop.godot_string_name ByRef, Godot.NativeInterop.NativeVariantPtrArgs, Godot.NativeInterop.godot_variant ByRef)
   at Godot.Node3D.InvokeGodotClassMethod(Godot.NativeInterop.godot_string_name ByRef, Godot.NativeInterop.NativeVariantPtrArgs, Godot.NativeInterop.godot_variant ByRef)
   at Godot.CollisionObject3D.InvokeGodotClassMethod(Godot.NativeInterop.godot_string_name ByRef, Godot.NativeInterop.NativeVariantPtrArgs, Godot.NativeInterop.godot_variant ByRef)
   at Godot.PhysicsBody3D.InvokeGodotClassMethod(Godot.NativeInterop.godot_string_name ByRef, Godot.NativeInterop.NativeVariantPtrArgs, Godot.NativeInterop.godot_variant ByRef)
   at Godot.CharacterBody3D.InvokeGodotClassMethod(Godot.NativeInterop.godot_string_name ByRef, Godot.NativeInterop.NativeVariantPtrArgs, Godot.NativeInterop.godot_variant ByRef)
   at GodotJoltCrashMRP.Scripts.Players.KinematicCharacterController.InvokeGodotClassMethod(Godot.NativeInterop.godot_string_name ByRef, Godot.NativeInterop.NativeVariantPtrArgs, Godot.NativeInterop.godot_variant ByRef)
   at Godot.Bridge.CSharpInstanceBridge.Call(IntPtr, Godot.NativeInterop.godot_string_name*, Godot.NativeInterop.godot_variant**, Int32, Godot.NativeInterop.godot_variant_call_error*, Godot.NativeInterop.godot_variant*)
```
