using Godot;
using System;

public partial class StateFader : ColorRect
{
    private const float Speed = 3f;
    private const float Duration = Mathf.Pi / Speed;
    private float timer = 0f;
    private CustomSignals signals;
    private Fadestate oldFadestate = Fadestate.None;
    public override void _Ready()
    {
        signals = Global.Signals(this);
        GetViewport().SizeChanged += OnViewportResize;
        OnViewportResize();
        Color = new Color(0, 0, 0, 0);
    }

    public override void _Process(double delta)
    {
        if (Global.Fadestate != oldFadestate)
        {
            oldFadestate = Global.Fadestate;
            timer = 0f;
        }
        if (Global.Fadestate == Fadestate.FadeOut)
        {
            if (timer > Duration)
            {
                Modulate = Modulate with { A = 0f };
                Global.Fadestate = Fadestate.None;
                GD.Print((int)Global.Player);
            }
            else if (timer > Duration * 0.5)
            {
                if (Global.State != Global.TargetState)
                {
                    Global.State = Global.TargetState;
                    signals.EmitSignal(nameof(CustomSignals.StateChanged));
                    GD.Print(Global.State);
                }
                if (Global.Player != Global.TargetPlayer)
                {
                    Global.Player = Global.TargetPlayer;
                    signals.EmitSignal(nameof(CustomSignals.PlayerChanged));
                    GD.Print(Global.Player);
                }
            }
            Color = new Color(0, 0, 0);
            Modulate = Modulate with { A = Mathf.Sin(Speed * Mathf.Min(timer, Duration)) };
        }
        timer += (float)delta;
    }

    public override void _Input(InputEvent @event)
    {
        if (Global.State != Gamestate.Title || Global.Fadestate != Fadestate.None)
            return;
        if (@event.IsAction("Left"))
        {
            Global.Fadestate = Fadestate.FadeOut;
            Global.TargetPlayer = (Player)Global.Mod((int)Global.Player - 1, 3);
            Global.Playsound(this, Audio.Woosh);
        }
        if (@event.IsAction("Right"))
        {
            Global.Fadestate = Fadestate.FadeOut;
            Global.TargetPlayer = (Player)Global.Mod((int)Global.Player + 1, 3);
            Global.Playsound(this, Audio.Woosh);
        }
    }

    private void OnViewportResize()
    {
        Size = GetViewport().GetVisibleRect().Size;
        Position = new Vector2(0, 0);
    }
}
