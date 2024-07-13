using Godot;
using System;

public partial class GameOverFern : Sprite2D
{
    private CustomSignals signals;
    public override void _Ready()
    {
        signals = Global.Signals(this);
		GetViewport().SizeChanged += OnViewportResize;
        signals.StateChanged += StateChanged;
        OnViewportResize();
        StateChanged();
	}

    private void OnViewportResize()
    {
        var screensize = GetViewport().GetVisibleRect().Size;
        var scale = screensize.X / Texture.GetSize().X;
        Position = new Vector2(screensize.X / 2f, screensize.Y / 2f);
        Scale = new Vector2(scale, scale);
    }

    private void StateChanged()
    {
        Visible = Global.State == Gamestate.End;
    }


}
