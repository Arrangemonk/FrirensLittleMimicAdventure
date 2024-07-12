using Godot;
using System;

public partial class Credits : RichTextLabel
{
	private CustomSignals signals;
	public override void _Ready()
	{
		signals = Global.Signals(this);
		signals.StateChanged += OnStateChanged;
		GetViewport().SizeChanged += OnViewportResize;
		OnViewportResize();
		Visible = false;
	}

    private void OnStateChanged()
	{
		Visible = false;
		if(Global.State != Gamestate.Credits)
			return;
		Global.CameraPosition = new Vector3(0.0f,5.0f,6.0f);
		Global.CameraTarget = new Vector3(0.0f, 6.0f, 10f);
		OnViewportResize();
    }

	private void OnViewportResize()
	{
		var screensize = GetViewport().GetVisibleRect().Size;
		var factor = screensize.X /  1280f;
		Size = screensize;
		AddThemeFontSizeOverride("normal_font_size",(int)(factor * 40f));
		AddThemeFontSizeOverride("bold_font_size",(int)(factor * 50f));
		Position = new Vector2(0,0);
	}
}
