using Godot;
using System;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

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

        Global.FernTimer = new Timer
        {
            WaitTime = 20,
            OneShot = true,
        };
        Global.FernTimer.Timeout += Timeout;
        AddChild(Global.FernTimer);
        Global.FernTimer.Start();
}

    private void Timeout()
    {
        if (Global.State != Gamestate.Revolver)
            return;
        Global.Fadestate = Fadestate.FadeOut;
        Global.TargetState = Gamestate.EndFern;
        signals.EmitSignal(nameof(CustomSignals.PlaySound), (int)Audio.Angry);
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
        Visible = Global.State == Gamestate.EndFern;
    }

    public override void _Input(InputEvent @event)
    {
        Global.FernTimer.Start();
    }


}
