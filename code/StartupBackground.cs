using Godot;
using System;

public partial class StartupBackground : ColorRect
{
    private float timer = MathF.PI * 0.25f;
	private CustomSignals signals;
    public override void _Ready()
	{
		signals = Global.Signals(this);
        Color = new Color(0, 0, 0);
        Global.Playsound(this,Audio.Flma);
        GetViewport().SizeChanged += OnViewportResize;
        OnViewportResize();
}
    public override void _Process(double delta)
	{
		if(Global.State != Gamestate.Title)
		    return;
		timer += (float)delta;
        Modulate = Modulate with { A = Mathf.Sin(Mathf.Min(Math.Max(MathF.PI / 2f, timer), MathF.PI)) };
        if (timer < MathF.PI * 1.05 ) 
            return;
        signals.EmitSignal(nameof(CustomSignals.ControlsUnlocked));
		QueueFree();
    }

 	private void OnViewportResize()
	{
		Size = GetViewport().GetVisibleRect().Size;
		Position = new Vector2(0,0);
	} 
}
