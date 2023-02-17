using System;
using Godot;
using GodotOnReady.Attributes;

public partial class Bazooka : Spatial, IFirearm
{
    [Export] private readonly PackedScene _projectile;
    [Export] public float LaunchForce { get; set; } = 30f;

    [OnReadyGet("%Muzzle")]
    private Position3D _muzzle;

    [OnReady]
    private void Ready()
    {
        if (_projectile is null)
        {
            throw new ApplicationException("No projectile scene resource specified");
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("fire"))
        {
            Fire();
        }
    }

    public void Fire()
    {
        _muzzle.AddChild(_projectile.Instance());
    }
}
