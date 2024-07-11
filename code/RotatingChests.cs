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

	bool Success = true;

	private Random rnd;
	private Gamestate lastState {get;set;}
	private CustomSignals signals;

	private chest_grimoir grimoir;
	private chest_mimic mimic;

	private GpuParticles3D SmokepuffReplacer;
	private GpuParticles3D SmokepuffReplacee;

	List<Label3D> labels = new List<Label3D>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		signals = GetNode<CustomSignals>("/root/CustomSignals");
		grimoir = GetNode<chest_grimoir>("../chest_grimoir");
		mimic = GetNode<chest_mimic>("../chest_mimic");
		SmokepuffReplacer = GetNode<GpuParticles3D>("SmokepuffReplacer");
		SmokepuffReplacee = GetNode<GpuParticles3D>("SmokepuffReplacee");
		for(int i = 0; i <Global.MaxChests; i++)
		{
            var label = new Label3D
            {
                Billboard = BaseMaterial3D.BillboardModeEnum.Enabled,
				FontSize = 52,
            };
            labels.Add(label);
			AddChild(label);
		}
	
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
				case Gamestate.Result:
					SetupResult();
					break;
				default:
					SetupHidden(true);
					break;
			}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Global.State != lastState)
		{
			lastState = Global.State;
			SmokepuffReplacer.Emitting = false;
			SmokepuffReplacee.Emitting = false;
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
			case Gamestate.Result:
				Result(delta);
				break;
			default:
				Hidden(delta);
				break;
		}
		timer += (float)delta * speed;
	}

	private void SetupHidden(bool all = false)
	{
		mimic.Visible = false;
		grimoir.Visible = false;
		var start = all ? 0: amount;
		for (int i = start; i < Global.MaxChests; i++)
		{
			Multimesh.SetInstanceTransform(i, new Transform3D(Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero));
			labels[i].Scale = Vector3.Zero;
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

	private static int starter (int count)
	{
		return count switch
        {
            3 => 4,
            4 => 0,
            5 => 4,
            6 => 1,
            7 => 4,
            8 => 1,
            _ => 4,
        };
	}

	private void Replace(int replacer, int replacee)
	{
		if(current == replacer)
			current = replacee;
		else if(current == replacee)
		current = replacer;

		//valids?
		var  v1 = Array.IndexOf(valids,replacer);
		var  v2 = Array.IndexOf(valids,replacee);
		var tmp = lablenames[v1];
		lablenames[v1] = lablenames[v2];
		lablenames[v2] = tmp;

	}

	private int current = 0;
	private int selected = 0;
	private int oldselected = 0;

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
		lablenames = new int[amount];
		oldlablenames = new int[amount];
		for(int i = 0;i<amount;i++)
		{
			lablenames[i] = i+1;
			oldlablenames[i] = i+1;
		}
		replacing = false;
		current = starter(amount);
		showlabels = true;
		timer = 0;
		grimoir.Visible = true;
		grimoir.Position = new Vector3((current%3)*3f -3f,0f,-1*((current-(current%3))-3f));
		var player = grimoir.GetNode<AnimationPlayer>("GrimoirPlayer");
		player.Seek(0.5);
		signals.EmitSignal(nameof(CustomSignals.PlaySound),"chestopen");
	}


	private int replacer;
	private Vector2 replacerInit;
	private int replacee;
	private Vector2 replaceeInit;
	private bool replacing = false;
	private bool showlabels = false;

	private int[] valids;
	private int[] lablenames;
	private int[] oldlablenames;

	private float HeightA(float x) =>  MathF.Sin(x * x * MathF.PI);
	private static Vector3 hover = new Vector3(0,2,0);

	private void Shuffle(double delta)
	{
		//predefine all as invalid
		SetupHidden(true);
		//place valid ones
		foreach(int i in valids)
		{
			var position = new Vector3((i%3)*3f -3f,0f,-1*((i-(i%3))-3f));
			Multimesh.SetInstanceTransform(i, new Transform3D(Basis.Identity, position));
			if(!showlabels)
				continue;
			labels[i].Position = position + hover;
			labels[i].Scale = Vector3.One;
			labels[i].Text = (oldlablenames[Array.IndexOf(valids,i)]).ToString();
		}
		//we dont want to confuse people with animation during fadestate
		if(Global.Fadestate == Fadestate.FadeOut)
			return;
		//determine current pair
		if(!replacing && !showlabels)
		{
			if(currentshuffle == shuffles){
				Global.Fadestate = Fadestate.FadeOut;
				Global.TargetState =  Gamestate.Revolver;
			}
			else{
				signals.EmitSignal(nameof(CustomSignals.PlaySound),"shuffle");
			}
			if(currentshuffle > 0){
				GD.Print("shake");
				signals.EmitSignal(nameof(CustomSignals.ViolentShakeStart));
				signals.EmitSignal(nameof(CustomSignals.PlaySound),"impact");

				SmokepuffReplacer.Position = new Vector3(replacerInit.X,0,-replacerInit.Y);
				SmokepuffReplacer.Emitting = true;
				SmokepuffReplacee.Position = new Vector3(replaceeInit.X,0,-replaceeInit.Y);
				SmokepuffReplacee.Emitting = true;
			}
			replacing = true;
			currentshuffle++;
			timer = 0;
				replacer = valids[rnd.Next(0,valids.Length)];
			do{
				replacee = valids[rnd.Next(0,valids.Length)];
			}while(replacer == replacee);
			for(int i = 0; i< lablenames.Length;i++)
			{
				oldlablenames[i] = lablenames[i];
			}
			Replace(replacer,replacee);
			GD.Print(current);

			replacerInit = new Vector2((replacer%3)*3f -3f,(replacer-(replacer%3))-3f);
			replaceeInit = new Vector2((replacee%3)*3f -3f,(replacee-(replacee%3))-3f);
		}
		if(showlabels)
		{
			if(timer >= 2f){
				showlabels = false;
				foreach(int i in valids)
				{
					labels[i].Scale = Vector3.Zero;
				}
			}
			grimoir.Visible = true;
			Multimesh.SetInstanceTransform(current, new Transform3D(Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero));

			return;
		}

		var progress = Global.Lerp(0f,1f,timer);
		//animate replacement
		var currentreplacer = Global.SmoothStep(replacerInit,replaceeInit,progress);
		var currentreplacee = Global.SmoothStep(replaceeInit,replacerInit,progress);

		var scaley = 1f;//1f + Mathf.Sin((progress + 0.25f) * 2 * Mathf.Pi) * 0.2f;
		var scalex = 1f;//1.05f + Mathf.Sin((progress - 0.25f) * 2 * Mathf.Pi) * 0.025f;

		var replacerpos = new Vector3(currentreplacer.X,Mathf.Sin(progress*  MathF.PI)*4f,-currentreplacer.Y);
		Multimesh.SetInstanceTransform(replacer, 
		new Transform3D(new Vector3(scalex,0,0), new Vector3(0,scaley,0), new Vector3(0,0,1),replacerpos));
		labels[replacer].Position = replacerpos + hover;

		var replaceepos = new Vector3(currentreplacee.X,Mathf.Sin(progress* MathF.PI)*2f,-currentreplacee.Y);
		Multimesh.SetInstanceTransform(replacee,
		 new Transform3D(new Vector3(scalex,0,0), new Vector3(0,scaley,0), new Vector3(0,0,1),replaceepos));
		labels[replacee].Position = replaceepos + hover;

		 if(timer >= 0.15f)
		 {
			SmokepuffReplacer.Emitting = false;
			SmokepuffReplacee.Emitting = false;
		 }

		if(timer >= 1f)
		replacing = false;
	}


	private void SetupRevolver()
	{
		GD.Print("revolver");
		Global.CameraPosition = new Vector3(0.0f,1.5f,6.0f);
		Global.CameraTarget = new Vector3(0.0f, 1.0f, -0.0f);
		amount = Global.AmountChests;
		//rotation = 2.0f * MathF.PI / amount;
		oldRotation = 0;
		currentRotation = 0;
		SetupHidden(true);
		timer = 1;
		oldselected = 0;
		selected = 0;
	}

	float rotation = 0f;
	float oldRotation;
	float currentRotation;

	private void Revolver(double delta)
	{
		if(timer > 1){
			ProcessActions();
		return;
		}
		var tmprotation = Global.OvershootSmoothStep(oldRotation, currentRotation, timer);
		for (int i = 0; i < amount; i++)
		//foreach(int v in valids)
		{
			var selectedindex = Array.IndexOf(valids,oldselected);
			//var i = Array.IndexOf(valids,v);
			var angle = 2.0f * MathF.PI * (i-selectedindex) / amount + tmprotation;
			var position = new Vector3(MathF.Sin(angle) * distance, 0f, MathF.Cos(angle) * distance);
			Multimesh.SetInstanceTransform(i, new Transform3D(Basis.Identity,position ));
			labels[i].Position = position + hover;
			labels[i].Scale = Vector3.One;
			labels[i].Text = (i+1).ToString();
		}
	}

	private void resetRevolver(Direction direction)
	{
			timer = 0;
			rotation = (direction == Direction.Left ? 1 : -1) * 2.0f * MathF.PI / amount;
			oldRotation = 0f;
			currentRotation = rotation;
			SetupHidden();
	}

	private void SetupResult()
	{
		timer = 0;
		SetupHidden(true);
		//we dont use the 0th, it is replaced by mimic or grimoir
		for (int i = 1; i < amount; i++)
		{
			var angle = 2.0f * MathF.PI * i / amount;
			Multimesh.SetInstanceTransform(i, new Transform3D(Basis.Identity, new Vector3(MathF.Sin(angle) * distance, 0f, MathF.Cos(angle) * distance)));
		}
	
			if(selected == current)
			{
				//signals.EmitSignal(nameof(CustomSignals.PlaySound),"plop");
				grimoir.Visible = true;
				grimoir.Position = new Vector3(0f, 0f,distance);
				var player = grimoir.GetNode<AnimationPlayer>("GrimoirPlayer");
				player.Seek(0.5);
				slotmachinestaring = true;
			}
			else{
				mimistarting = true;
				//signals.EmitSignal(nameof(CustomSignals.PlaySound),"plop");
				mimic.Visible = true;
				mimic.Position = new Vector3(0f, 0f,distance);
				var player = mimic.GetNode<AnimationPlayer>("MimicPlayer");
				player.Seek(0.5);
			}
	}

	private bool slotmachinestaring;
	private bool mimistarting;
	private void Result(double Delta)
	{
		if(timer < 0.7f)
			return;
		if(slotmachinestaring)
		{
			slotmachinestaring = false;
			GD.Print("slotmachine!!");
			signals.EmitSignal(nameof(CustomSignals.SlotmachineActivated));
			signals.EmitSignal(nameof(CustomSignals.PlaySound),"slotmachine");
		}

		if(mimistarting)
		{
			mimistarting = false;
				Global.Fadestate = Fadestate.FadeOut;
				Global.TargetState = Gamestate.End;
		}
		
	}

	private bool _left;
	private bool _right;
	private bool _return;



		byte? action = null;

    public override void _Input(InputEvent @event)
    {
		if(Global.State != Gamestate.Revolver)
			return;
		if(timer < 0.25) 
			return;
		if (@event.IsAction("Left"))
		{
			action = 1;
		}
		if (@event.IsAction("Right"))
		{
			action = 2;
		}
		if (@event.IsAction("Return"))
		{
			action = 3;
		}
    }

	private void ProcessActions()
	{
        switch (action)
        {
            case 3:
				//Global.Fadestate = Fadestate.FadeOut;
				Global.State =  Gamestate.Result;
				signals.EmitSignal(nameof(CustomSignals.PlaySound),"chestopen");
                break;
            case 1:
                resetRevolver(Direction.Left);
				oldselected = selected;
				selected = valids[(valids.ToList().IndexOf(selected) -1 + amount) % amount];
                signals.EmitSignal(nameof(CustomSignals.PlaySound), "plop");
                break;
            case 2:
                resetRevolver(Direction.Right);
				oldselected = selected;
				selected = valids[(valids.ToList().IndexOf(selected) +1 + amount) % amount];
                signals.EmitSignal(nameof(CustomSignals.PlaySound), "plop");
                break;
        }
        action = null;
	}
	
}
