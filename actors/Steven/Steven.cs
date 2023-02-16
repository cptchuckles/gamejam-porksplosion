using Godot;
using GodotOnReady.Attributes;

public partial class Steven : KinematicBody
{
    [Export] private bool _walking = true;
    [Export] private readonly float _topSpeed = 3f;
    [Export(PropertyHint.Range, "0.0,1.0")] private readonly float _newGoalOdds = .5f;

    [OnReadyGet("%RandomGoalTimer")] private Timer _pickGoalTimer;

    private readonly RandomNumberGenerator rng = new();

    private float _speed;
    private Vector3 _velocity;
    private readonly Vector3 _gravity = (Vector3)ProjectSettings.GetSetting("physics/3d/default_gravity_vector")
                                        * (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

    [OnReady]
    private void Ready()
    {
        _speed = _walking ? _topSpeed : 0f;
        rng.Randomize();
        _ = _pickGoalTimer.Connect("timeout", this, nameof(AutonomousInput));
    }

    public override void _PhysicsProcess(float delta)
    {
        _speed = Mathf.MoveToward(_speed, _walking ? _topSpeed : 0f, 5f * delta);

        // TODO: make walking work

        LookAt(ToGlobal(_velocity with { y = 0f }), Vector3.Up);
        _velocity = MoveAndSlide(_velocity, Vector3.Up);
    }

    private void AutonomousInput()
    {
        if (rng.Randf() > _newGoalOdds)
        {
            return;
        }

        if (rng.Randf() < _newGoalOdds)
        {
            _walking = !_walking;
        }

        if (_walking && rng.Randf() < _newGoalOdds)
        {
            // TODO: move the WalkGoal around in local space
        }
    }
}
