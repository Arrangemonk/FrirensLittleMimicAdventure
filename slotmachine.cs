using Godot;
using System;

public partial class slotmachine : Node3D
{
	// Called when the node enters the scene tree for the first time.

	Slot slot;
	public override void _Ready()
	{
		slot = GetNode<Slot>(nameof(slotmachine));
	}

    public override void _Input(InputEvent @event)
    {
		slot._Input(@event);
    }
}
