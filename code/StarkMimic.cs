using Godot;
using System;

public partial class StarkMimic : TextureRect
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
        Visible = Global.Player == Player.Stark && Global.State == Gamestate.End;
    }
}
