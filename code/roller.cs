using Godot;
using System;

public partial class Roller : Node3D
{

    private MenuRevolver revolver;
	public override void _Ready()
	{
		revolver = GetNode<MenuRevolver>(nameof(MenuRevolver));
	}

	public override void _Input(InputEvent @event)
	{
		revolver._Input(@event);
	}
}
