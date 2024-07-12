using Godot;
using System;

public partial class Slotmachine : Node3D
{
	// Called when the node enters the scene tree for the first time.

    private Slot slot;
	private CustomSignals signals;
	public override void _Ready()
	{
		signals = Global.Signals(this);
		slot = GetNode<Slot>("Slotmachine");
		signals.StateChanged += Statechanged;
		signals.SlotmachineActivated += Activate;
	}

	public override void _Input(InputEvent @event)
	{
		slot._Input(@event);
	}

	private void Statechanged()
	{
		Visible = (Global.State == Gamestate.Result);
		GD.Print(Global.State,"slotmachine visible:", Visible);
	}

		private void Activate()
	{
		Visible = true;
		GD.Print(Global.State,"slotmachine visible:", Visible);
	}

}
