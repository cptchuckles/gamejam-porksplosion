using Godot;
using GodotOnReady.Attributes;

public partial class Hud : Control
{
    [OnReadyGet("%Score")] private Label _score;
    [OnReadyGet("%GrabbyHand")] private TextureRect _hand;
    [OnReadyGet("%Crosshair")] private TextureRect _crosshair;

    public void SetCashMoney(int amount)
    {
        _score.Text = amount.ToString();
    }

    public void ShowHand()
    {
        _hand.Visible = true;
        _crosshair.Visible = false;
    }

    public void HideHand()
    {
        _hand.Visible = false;
        _crosshair.Visible = true;
    }
}
