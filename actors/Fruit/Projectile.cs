using Godot;

public class Projectile : RigidBody
{
    [Export] private float _launchForce = 10f;
    [Export] private readonly float _launchTorque = 3f;
    [Export] private readonly float _lifeSpan = 5f;

    public override void _Ready()
    {
        if (GetParent().Owner is IFirearm firearm)
        {
            _launchForce = Mathf.Max(_launchForce, firearm.LaunchForce);
        }
        SetAsToplevel(true);

        ApplyCentralImpulse(-GlobalTransform.basis.z * _launchForce);

        Vector3 vector3Random = new Vector3(GD.Randf(), GD.Randf(), GD.Randf()).Normalized();
        ApplyTorqueImpulse(vector3Random * GD.Randf() * _launchTorque);

        GetTree().CreateTimer(_lifeSpan).Connect("timeout", this, nameof(LifetimeOver));
    }

    private void LifetimeOver()
    {
        CreateTween()
            .SetProcessMode(Tween.TweenProcessMode.Physics)
            .TweenProperty(this, "scale", Vector3.Zero, .5f)
            .Connect("finished", this, "queue_free");
    }
    }
}
