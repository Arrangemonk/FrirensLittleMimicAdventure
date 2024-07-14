using Godot;
using System;

public partial class FullscreenCheckbox : CheckBox
{
    private CustomSignals signals;

    public override void _Ready()
    {
        signals = Global.Signals(this);
        signals.StateChanged += GrabFocus;
        GrabFocus();
    }

}
