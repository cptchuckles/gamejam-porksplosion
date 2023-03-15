using Godot;
using GodotOnReady.Attributes;

public partial class Steven : KinematicBody
{
    [Export] private bool _walking = true;
    [Export] private readonly float _topSpeed = 3f;
    [Export] private readonly float _acceleration = 5f;

    [Export(PropertyHint.Range, "0.0,1.0")]
    private readonly float _newGoalOdds = .5f;

    [OnReadyGet("%RandomGoalTimer")]
    private Timer _pickGoalTimer;

    [OnReadyGet("%Hitbox")] private Area _hitbox;

    private readonly RandomNumberGenerator _rng = new();

    private Vector2 _input = new();
    private float _speed;
    private Vector3 _velocity;
    private Vector3 _look;
    private Vector3 _gravityAccumulator = new();
    private readonly Vector3 _gravity =
        (Vector3)ProjectSettings.GetSetting("physics/3d/default_gravity_vector")
        * (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

    [OnReady]
    private void Ready()
    {
        _look = -GlobalTransform.basis.z;
        _speed = _walking ? _topSpeed : 0f;
        _rng.Randomize();
        _ = _pickGoalTimer.Connect("timeout", this, nameof(AutonomousInput));
        _ = _hitbox.Connect("area_entered", this, nameof(OnHitReceived));
    }

    private void OnHitReceived(Area other)
    {
        if (other.Owner is IFruit fruit)
        {
            GD.Print("Steven down");
            fruit.GetEaten();
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        _gravityAccumulator += _gravity * delta;
        _speed = Mathf.MoveToward(_speed,
                                  _walking ? _topSpeed : 0f,
                                  _acceleration * delta);

        _velocity = new Vector3(_input.x, 0f, _input.y) * _speed;

        if (_velocity.Length() > 0f)
        {
            _look = _look.LinearInterpolate(
                GlobalTranslation.DirectionTo(GlobalTranslation + _velocity),
                0.2f);

            LookAt(GlobalTranslation + _look, Vector3.Up);
        }
        _ = MoveAndSlide(_velocity + _gravityAccumulator, Vector3.Up);

        if (IsOnFloor())
        {
            _gravityAccumulator = new();
        }
    }

    private void AutonomousInput()
    {
        if (_rng.Randf() > _newGoalOdds)
        {
            return;
        }

        if (_rng.Randf() < _newGoalOdds)
        {
            _walking = !_walking;
        }

        if (_walking && _rng.Randf() < _newGoalOdds)
        {
            _input = Vector2.Right.Rotated(_rng.Randf() * Mathf.Pi * 2f);
        }
    }
}
