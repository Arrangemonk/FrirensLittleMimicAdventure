using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class RotatingChests : MultiMeshInstance3D
{
	int amount;
	const float distance = 3f;
	private float speed = 1.0f;
	private int shuffles;
	private int currentshuffle;
	float timer = 0f;

	private Random rnd;
	private Gamestate lastState {get;set;}
	private CustomSignals signals;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		signals = GetNode<CustomSignals>("/root/CustomSignals");
		signals.AmountChecksChanged += Setup;
		rnd = new Random();
	}

	private void Setup()
	{
		switch (Global.State)
			{
				case Gamestate.Revolver:
					SetupRevolver();
					break;
				case Gamestate.Shuffle:
					SetupShuffle();
					break;
				default:
					SetupHidden();
					break;
			}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Global.State != lastState)
		{
			lastState =Global.State;
			Setup();
		}
		switch (Global.State)
		{
			case Gamestate.Revolver:
				Revolver(delta);
				break;
			case Gamestate.Shuffle:
				Shuffle(delta);
				break;
			default:
				Hidden(delta);
				break;
		}
	}

	private void SetupHidden()
	{
		for (int i = 0; i < Global.MaxChests; i++)
		{
			Multimesh.SetInstanceTransform(i, new Transform3D(Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero));
		}
	}

	private void Hidden(double delta)
	{
	}


	private static int[] ValidPositions(int count)
	{
        return count switch
        {
            3 => new[] {          3, 4, 5          },
            4 => new[] { 0,    2,          6,    8 },
            5 => new[] { 0,    2, 	 4,    6,    8 },
            6 => new[] { 0, 1, 2, 		   6, 7, 8 },
            7 => new[] { 0, 1, 2, 	 4,    6, 7, 8 },
            8 => new[] { 0, 1, 2, 3, 	5, 6, 7, 8 },
            _ => new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
        };
    }

	private void SetupShuffle()
	{
		speed = Global.Speed;
		shuffles = Global.Shuffles;
		currentshuffle = 0;
		GD.Print("shuffle");
		Global.CameraPosition = new Vector3(0.0f,5.0f,6.0f);
		Global.CameraTarget = new Vector3(0.0f, 1.0f, 0.0f);
		amount = Global.AmountChests;
		valids = ValidPositions(amount);
		replacing = false;
	}


	private int replacer;
	private Vector2 replacerInit;
	private int replacee;
	private Vector2 replaceeInit;
	private bool replacing = false;

	private int[] valids;

	private float HeightA(float x) =>  MathF.Sin(x * x * MathF.PI);
	
	private void Shuffle(double delta)
	{
		timer += (float)delta * speed;
		//predefine all as invalid
		SetupHidden();
		//place valid ones
		foreach(int i in valids)
		{
			Multimesh.SetInstanceTransform(i, new Transform3D(Basis.Identity, new Vector3((i%3)*3f -3f,0f,(i-(i%3))-3f)));
		}
		//determine current pair
		if(!replacing)
		{
			if(currentshuffle > shuffles)
				Global.State = Gamestate.Revolver;
			currentshuffle++;
			replacing = true;
			timer = 0;
				replacer = valids[rnd.Next(0,valids.Length)];
			do{
				replacee = valids[rnd.Next(0,valids.Length)];
			}while(replacer == replacee);
			replacerInit = new Vector2((replacer%3)*3f -3f,(replacer-(replacer%3))-3f);
			replaceeInit = new Vector2((replacee%3)*3f -3f,(replacee-(replacee%3))-3f);
		}
		//animate replacement
		var currentreplacer = Global.SmoothStep(replacerInit,replaceeInit,timer);
		var currentreplacee = Global.SmoothStep(replaceeInit,replacerInit,timer);

		Multimesh.SetInstanceTransform(replacer, new Transform3D(Basis.Identity, new Vector3(currentreplacer.X,Mathf.Sin(Global.SmoothStep(0f,1f,timer)* MathF.PI)*4f,currentreplacer.Y)));
		Multimesh.SetInstanceTransform(replacee, new Transform3D(Basis.Identity, new Vector3(currentreplacee.X,Mathf.Sin(Global.SmoothStep(0f,1f,timer)* MathF.PI)*2f,currentreplacee.Y)));

		if(timer >= 1f)
		replacing = false;
	}


	private void SetupRevolver()
	{
		GD.Print("revolver");
		Global.CameraPosition = new Vector3(0.0f,1.5f,6.0f);
		Global.CameraTarget = new Vector3(0.0f, 1.0f, -0.0f);
		amount = Global.AmountChests;
		rotation = 2.0f * MathF.PI / amount;
		oldRotation = 0f;
		currentRotation = rotation;
		SetupHidden();
		for (int i = 0; i < amount; i++)
		{
			var angle = 2.0f * MathF.PI * i / amount;
			this.Multimesh.SetInstanceTransform(i, new Transform3D(Basis.Identity, new Vector3(MathF.Sin(angle) * distance, 0f, MathF.Cos(angle) * distance)));
		}
	}

	float rotation = 0f;
	float oldRotation;
	float currentRotation;

	private void Revolver(double delta)
	{
		timer += (float)delta * speed;
		if (timer > 1)
		{
			timer = 0;

		//	amount = (amount + 1) % (Global.MaxChests + 1);
		//	if (amount == 0)
		//		amount = 1;

			GD.Print(amount);
			rotation = 2.0f * MathF.PI / amount;
			//oldRotation = currentRotation;
			//currentRotation += rotation;
			oldRotation = 0f;
			currentRotation = rotation;
			for (int i = amount; i < Global.MaxChests; i++)
			{
				Multimesh.SetInstanceTransform(i, new Transform3D(Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero));
			}
		}

		var tmprotation = Global.OvershootSmoothStep(oldRotation, currentRotation, timer);
		for (int i = 0; i < amount; i++)
		{
			var angle = 2.0f * MathF.PI * i / amount + tmprotation;
			this.Multimesh.SetInstanceTransform(i, new Transform3D(Basis.Identity, new Vector3(MathF.Sin(angle) * distance, 0f, MathF.Cos(angle) * distance)));
		}
	}
}
