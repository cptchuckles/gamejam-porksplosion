using System;
using Godot;
using GodotOnReady.Attributes;

public partial class FPSPlayer : KinematicBody
{
    [Export] private readonly float _speed = 10f;
    [Export] private readonly float _turnSpeed = 3f * Mathf.Pi;
    [Export] private readonly float _stickDeadzone = 0.2f;

    private Vector3 _velocity = new();
    private Vector3 _gravity;

    private Vector2? _mouseDelta = null!;
    [Export] private readonly float _mouseSensitivity = 0.02f;

    [OnReadyGet("Head")] private Position3D _head;
    [OnReadyGet("Head/InteractRay")] private RayCast _interactRay;
    private int _score = 0;

    public FPSPlayer()
    {
        _gravity = (Vector3)ProjectSettings.GetSetting("physics/3d/default_gravity_vector")
                   * (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
    }

    [OnReady]
    private void Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _PhysicsProcess(float delta)
    {
        Orientate(delta);
        ComputeVelocity(delta);

        _velocity = MoveAndSlideWithSnap(
                _velocity,
                snap: Vector3.Down,
                upDirection: Vector3.Up,
                stopOnSlope: true,
                infiniteInertia: false
                );
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion) {
            _mouseDelta = motion.Relative;
        }
        else if (@event.IsActionPressed("ui_cancel"))
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
        else if (@event.IsActionPressed("use"))
        {
            if (_interactRay.IsColliding())
            {
                if (_interactRay.GetCollider() is IInteractable interactable)
                {
                    interactable.Interact(this);
                }
            }
        }
    }

    private void Orientate(float delta)
    {
        Vector2 input = Input.GetVector("yaw_right", "yaw_left", "pitch_down", "pitch_up", _stickDeadzone);
        input *= input.Length();

        if (_mouseDelta.HasValue)
        {
            input = - _mouseDelta.Value * _mouseSensitivity;
            _mouseDelta = null;
        }

        RotateY(input.x * _turnSpeed * delta);
        _head.RotateX(input.y * _turnSpeed * delta);

        _head.Rotation = _head.Rotation with {
            x = Mathf.Clamp(_head.Rotation.x, -Mathf.Pi / 2, Mathf.Pi / 2),
        };
    }

    private void ComputeVelocity(float delta)
    {
        Vector2 input = Input.GetVector("left", "right", "forward", "back", _stickDeadzone);
        input *= input.Length() * _speed;

        _velocity += _gravity * delta;
        _velocity = _velocity with
        {
            x = input.x,
            z = input.y,
        };
        _velocity = GlobalTransform.basis.Xform(_velocity);
    }

    public void Score(int value)
    {
        _score += value;
        GetNode("Head/Camera/HUD").GetNode<Label>("%Score").Text = $"{_score}";
    }
}
