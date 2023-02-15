using Godot;
using GodotOnReady.Attributes;

public partial class Hud : Control
{
    [OnReadyGet("%GrabbyHand")] private TextureRect _hand;

    public void SetCashMoney(int amount)
    {
        GetNode<Label>("%Score").Text = amount.ToString();
    }

    public void ShowHand()
    {
        _hand.Visible = true;
    }

    public void HideHand()
    {
        _hand.Visible = false;
    }
}
