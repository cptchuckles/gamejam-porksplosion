using Godot;

public partial class CashMoney : Area, IInteractable
{
    [Export] private readonly int _value = 1;

    public void Interact(Node byWho)
    {
        if (byWho is FPSPlayer player)
        {
            player.Score(_value);
            QueueFree();
        }
    }
}
