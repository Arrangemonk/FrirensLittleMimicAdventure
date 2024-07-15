using Godot;
using System;
using System.Drawing;

public partial class SettingsMenu : CenterContainer
{
    private CustomSignals signals;
    public override void _Ready()
    {
        signals = Global.Signals(this);
        signals.StateChanged += StateChanged;
        StateChanged();
        GetViewport().SizeChanged += OnViewportResize;
        OnViewportResize();
    }

    private void StateChanged()
    {
        Visible = Global.State == Gamestate.Setting;
    }

    private void OnViewportResize()
    {
        var screensize = GetViewport().GetVisibleRect().Size;
        var factor = 0.5f * screensize.X / 698;
        Scale = new Vector2(factor, factor);
    }
}
