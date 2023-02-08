using Godot;

public partial class FPSPlayer : KinematicBody
{
    [Export] private readonly float _speed = 10f;
    [Export] private readonly float _stick_deadzone = 0.1f;

    private Vector3 _velocity = new();
    private Vector3 _gravity;

    public FPSPlayer()
    {
        _gravity = (Vector3)ProjectSettings.GetSetting("physics/3d/default_gravity_vector")
                   * (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
    }

    public override void _PhysicsProcess(float delta)
    {
        ComputeVelocity(delta);

        _velocity = MoveAndSlideWithSnap(
                _velocity,
                snap: Vector3.Down,
                upDirection: Vector3.Up,
                stopOnSlope: true,
                infiniteInertia: false
                );
    }

    private void ComputeVelocity(float delta)
    {
        Vector2 input = Input.GetVector("left", "right", "forward", "back", _stick_deadzone);
        GD.Print(input.Length());
        input *= _speed;

        _velocity += _gravity * delta;
        _velocity = _velocity with
        {
            x = input.x,
            z = input.y,
        };
        _velocity = GlobalTransform.basis.Xform(_velocity);
    }
}
