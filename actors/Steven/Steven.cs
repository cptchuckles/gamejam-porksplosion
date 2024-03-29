using System;
using Godot;
using GodotOnReady.Attributes;

public partial class Steven : KinematicBody
{
    [Export] private bool _walking = true;
    [Export] private readonly float _topSpeed = 3f;
    [Export] private readonly float _acceleration = 5f;
    [Export]
    public int Hp
    {
        get => hp;
        private set
        {
            hp = value;
            if (hp <= 0) {
                Die();
            }
        }
    }

    public int Coins { get; private set; } = 10;


    [Export(PropertyHint.Range, "0.0,1.0")]
    private readonly float _newGoalOdds = .5f;

    [OnReadyGet("%RandomGoalTimer")]
    private Timer _pickGoalTimer;

    [OnReadyGet("%Hitbox")] private Area _hitbox;

    private readonly RandomNumberGenerator _rng = new();

    private readonly PackedScene _cashMoneyScene = GD.Load<PackedScene>("res://actors/CashMoney/CashMoney.tscn");
    private Vector2 _input = new();
    private float _speed;
    private Vector3 _velocity;
    private Vector3 _look;
    private Vector3 _gravityAccumulator = new();
    private int hp = 10;
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
            Hp--;
        }
    }

    public override void _PhysicsProcess(float delta)
    {
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

        _gravityAccumulator += _gravity * delta;

        _ = MoveAndSlide(_velocity + _gravityAccumulator, Vector3.Up);

        if (IsOnFloor())
        {
            _gravityAccumulator -= _gravityAccumulator;
        }
    }

    private void Die()
    {
        QueueFree();
        for (int i=0; i<(int)GD.RandRange(Coins/2, Coins); i++) {
            var cashMoney = _cashMoneyScene.Instance<RigidBody>();
            cashMoney.Translate(Transform.origin);
            GetParent().AddChild(cashMoney);
            cashMoney.ApplyCentralImpulse(Vector3.Up * 5f);
            cashMoney.ApplyCentralImpulse(Vector3.Right.Rotated(Vector3.Up, (float)GD.RandRange(0, 2 * Mathf.Pi)) * 5f);
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
