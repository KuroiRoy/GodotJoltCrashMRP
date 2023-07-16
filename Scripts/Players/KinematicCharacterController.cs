using System.Diagnostics;
using Godot;
using Godot.Collections;

namespace GodotJoltCrashMRP.Scripts.Players;

public partial class KinematicCharacterController : CharacterBody3D {

    [Export] private Camera3D camera;
    [Export] private ShapeCast3D groundShapeCast;
    [Export] private float stepHeight = 0.25f;
    [Export] private float flyDrag = 0.6f;

    private Vector3 velocity = Vector3.Zero;
    private float speed = 4.0f;
    private float gravity = -9.8f;
    private float jumpHeight = 1.0f;
    private float groundHeight;
    private Stopwatch jumpStartTimer;
    private Mode mode;
    
    public override void _PhysicsProcess (double doubleDelta) {
        var delta = (float) doubleDelta;
        var inputEnabled = Input.MouseMode == Input.MouseModeEnum.Captured;
        var sprintMultiplier = Input.IsActionPressed("sprint") ? 2 : 1;

        var direction = new Vector3();
        var isJumpQueued = false;
        if (inputEnabled) {
            if (Input.IsActionPressed("move_forward")) direction -= camera.Basis.Z;
            if (Input.IsActionPressed("move_backward")) direction += camera.Basis.Z;
            if (Input.IsActionPressed("move_left")) direction -= camera.Basis.X;
            if (Input.IsActionPressed("move_right")) direction += camera.Basis.X;
            if (Input.IsActionPressed("jump")) direction += camera.Basis.Y;
            if (Input.IsActionPressed("crouch")) direction -= camera.Basis.Y;
            if (Input.IsActionJustPressed("jump")) {
                if (jumpStartTimer is { ElapsedMilliseconds: < 500 }) {
                    mode = mode == Mode.Walking ? Mode.Flying : Mode.Walking;
                    jumpStartTimer = null;
                }
                else {
                    jumpStartTimer = Stopwatch.StartNew();
                    isJumpQueued = true;
                }
            }
        }

        if (mode == Mode.Flying) {
            velocity -= velocity * flyDrag;
        }

        direction = direction.Normalized();
        velocity.X = direction.X * speed * sprintMultiplier;
        velocity.Z = direction.Z * speed * sprintMultiplier;
        if (mode == Mode.Flying) velocity.Y = direction.Y * speed * sprintMultiplier;

        if (groundShapeCast.GetCollisionCount() > 0)
            foreach (Dictionary groundCollision in groundShapeCast.CollisionResult) {
                groundHeight = Mathf.Max(groundHeight, groundCollision["point"].AsVector3().Y);
            }
        else groundHeight = GlobalPosition.Y + groundShapeCast.TargetPosition.Y;

        if (GlobalPosition.Y < groundHeight) groundHeight = GlobalPosition.Y;

        // Gravity and jumping
        if (mode == Mode.Walking) {
            if (isJumpQueued && IsOnFloor()) velocity.Y = Mathf.Sqrt(-2 * gravity * jumpHeight);
            else if (IsOnFloor()) velocity.Y = gravity * delta;
            else if (Input.IsActionPressed("jump") && jumpStartTimer is { ElapsedMilliseconds: < 500 }) { }
            else velocity.Y += gravity * delta;
        }

        // Move the character
        Velocity = velocity;
        if (mode == Mode.Flying || !StepUp(delta)) MoveAndSlide();
        // if (Input.IsActionJustPressed("unstuck")) {
        //     for (var i = 0; i < GetSlideCollisionCount(); i++) {
        //         var collision = GetSlideCollision(i);
        //         GlobalPosition += collision.GetDepth() * collision.GetNormal(); 
        //     }
        // }
        // PushRigidBodies();
    }

    private void PushRigidBodies () {
        var pushForceMultiplier = 0.1f; // Adjust the pushing force multiplier as needed

        // Calculate the horizontal velocity of the player
        var horizontalVelocity = new Vector3(velocity.X, 0, velocity.Z);
        var pushForce = horizontalVelocity.Length() * pushForceMultiplier;
        var pushDirection = horizontalVelocity.Normalized();

        var collisions = GetSlideCollisionCount();
        for (var i = 0; i < collisions; i++) {
            var collision = GetSlideCollision(i);
            if (collision.GetCollider() is RigidBody3D rigidBody) {
                // Apply an impulse at the point of contact
                var impulse = pushDirection * pushForce;
                var contactPoint = collision.GetPosition();
                rigidBody.ApplyImpulse(impulse, contactPoint - rigidBody.GlobalTransform.Origin);
            }
        }
    }

    private bool StepUp (float delta) {
        if (!IsOnFloor()) return false;

        // Cast body upward by step_height
        var upTestParams = new PhysicsTestMotionParameters3D();
        upTestParams.From = GlobalTransform;
        upTestParams.Motion = stepHeight * Vector3.Up;
        upTestParams.Margin = SafeMargin;
        
        var result = new PhysicsTestMotionResult3D();
        if (PhysicsServer3D.BodyTestMotion(GetRid(), upTestParams, result)) return false;

        var upTransform = GlobalTransform.Translated(stepHeight * Vector3.Up);
        var slideMotion = Slide(GetRid(), upTransform, Velocity * delta, SafeMargin).Slide(Vector3.Up);

        // Cast body by slide motion
        var forwardTestParams = new PhysicsTestMotionParameters3D {
            From = upTransform,
            Motion = slideMotion,
            Margin = SafeMargin
        };

        if (PhysicsServer3D.BodyTestMotion(GetRid(), forwardTestParams)) return false;

        // Cast body downward by step_height
        var downTestFrom = upTransform.Translated(slideMotion);
        var downTestParams = new PhysicsTestMotionParameters3D();
        downTestParams.From = downTestFrom;
        downTestParams.Motion = -stepHeight * Vector3.Up;
        downTestParams.Margin = SafeMargin;
        var downTestResult = new PhysicsTestMotionResult3D();

        if (PhysicsServer3D.BodyTestMotion(GetRid(), downTestParams, downTestResult)) {
            GlobalTransform = downTestFrom.Translated(downTestResult.GetTravel());
            return true;
        }

        return false;
    }

    private static Vector3 Slide (Rid body, Transform3D from, Vector3 motion, float margin = 0.001f, int maxSlides = 6, int maxCollisions = 16) {
        for (var i = 0; i < maxSlides; i++) {
            var params3D = new PhysicsTestMotionParameters3D();
            params3D.From = from;
            params3D.Motion = motion;
            params3D.Margin = margin;
            params3D.MaxCollisions = maxCollisions;

            var result = new PhysicsTestMotionResult3D();
            if (!PhysicsServer3D.BodyTestMotion(body, params3D, result)) break;

            var normalSum = Vector3.Zero;
            for (var j = 0; j < result.GetCollisionCount(); j++) normalSum += result.GetCollisionNormal(j);

            var normal = normalSum.Normalized();
            motion = result.GetRemainder().Slide(normal);
            from = from.Translated(result.GetTravel());
        }

        return motion;
    }

    private enum Mode {

        Walking,
        Flying,

    }

}