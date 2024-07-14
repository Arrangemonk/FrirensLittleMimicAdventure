using Godot;
using System;

public partial class SettingsMenu : CenterContainer
{
    private CustomSignals signals;

    public override void _Ready()
    {
        signals = Global.Signals(this);
        signals.StateChanged += StateChanged;
        StateChanged();
    }

    private void StateChanged()
    {
        Visible = Global.State == Gamestate.Setting;
    }
}
