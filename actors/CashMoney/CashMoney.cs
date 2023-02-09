using Godot;

public partial class CashMoney : Area, IInteractable
{
    [Export] private readonly float _spinRate = Mathf.Pi;
    [Export] private readonly int _value = 1;

    public override void _Process(float delta)
    {
        RotateY(_spinRate * delta);
    }

    public void Interact(Node byWhom)
    {
        if (byWhom is FPSPlayer player)
        {
            player.Score(_value);
            QueueFree();
        }
    }
}
