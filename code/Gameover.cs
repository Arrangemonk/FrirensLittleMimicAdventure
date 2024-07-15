using Godot;
using System;

public partial class Gameover : CenterContainer
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
        Visible = Global.State == Gamestate.End || Global.State == Gamestate.EndFern;
    }

    private void OnViewportResize()
    {
        var screensize = GetViewport().GetVisibleRect().Size;
        var factor = 0.75f * screensize.X / 1060f;
        Scale = new Vector2(factor, factor);
    }
}
