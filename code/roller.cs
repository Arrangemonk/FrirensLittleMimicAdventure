using Godot;
using System;

public partial class roller : Node3D
{
	// Called when the node enters the scene tree for the first time.

	MenuRevolver revolver;
	public override void _Ready()
	{
		revolver = GetNode<MenuRevolver>(nameof(MenuRevolver));
	}

	public override void _Input(InputEvent @event)
	{
		revolver._Input(@event);
	}
}
