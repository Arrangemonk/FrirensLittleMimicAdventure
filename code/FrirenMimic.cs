using Godot;
using System;

public partial class FrirenMimic : TextureRect
{
    private CustomSignals signals;
    public override void _Ready()
    {
        signals = Global.Signals(this);
        signals.PlayerChanged += PlayerChanged;
        PlayerChanged();
    }

    private void PlayerChanged()
    {
        Visible = Global.Player == Player.Friren && Global.State == Gamestate.End;
    }
}
