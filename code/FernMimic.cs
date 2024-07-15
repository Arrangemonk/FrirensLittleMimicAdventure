using Godot;
using System;

public partial class FernMimic : TextureRect
{
    private CustomSignals signals;
    public override void _Ready()
    {
        signals = Global.Signals(this);
        signals.PlayerChanged += PlayerChanged;
        signals.StateChanged += PlayerChanged;
        PlayerChanged();
    }

    private void PlayerChanged()
    {
        Visible = Global.Player == Player.Fern && Global.State == Gamestate.End;
    }
}
