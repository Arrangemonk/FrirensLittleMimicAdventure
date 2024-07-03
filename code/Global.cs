using Godot;
using System;
using System.Security.AccessControl;

public enum Gamestate{
    Title,
    Setting,
    Credits,
    Shuffle,
    Revolver,
    Result,
    Slotmachine,
    End,
    Quit
}

public enum Direction{
    Left,
    Right,
}

public enum Fadestate{
    None,
    FadeOut,
    FadeIn,
}

public enum Player{
    Friren = 0,
    Fern = 1,
    Stark = 2
}

public partial class Global : Node
{
    public static Gamestate State {get;set;} = Gamestate.Title;
    public static Gamestate TargetState {get;set;} = Gamestate.Title;
    public static Fadestate Fadestate {get;set;} = Fadestate.None;

    public static Player TargetPlayer {get;set;} = Player.Friren;
    public static Player Player {get;set;} = Player.Friren;
    public static int AmountChests {get;set;} = 3;
    public static int MaxChests  {get;set;} = 9; 
    public static int Speed  {get;set;} = 1; 
    public static int MaxSpeed  {get;set;} = 10; 
    public static int Shuffles  {get;set;} = 3; 
    public static int MaxShuffles {get;set;} = 100; 
    public static Vector3 CameraPosition {get;set;}
    public static Vector3 CameraTarget {get;set;}

    public static float SmoothStep(float from, float to, float weight)
    {
        weight = weight * weight * (3f - 2f * weight);
		return from + (to - from) * weight;
    }

    public static Vector3 SmoothStep(Vector3 from, Vector3 to, float weight)
    {
        weight = weight * weight * (3f - 2f * weight);
		return from + (to - from) * weight;
    }

    public static Vector2 SmoothStep(Vector2 from, Vector2 to, float weight)
    {
        weight = weight * weight * (3f - 2f * weight);
		return from + (to - from) * weight;
    }

    public static float OvershootSmoothStep(float from,float to, float weight)
    {
        //weight = weight * weight * (4f - 3f * weight);
        //weight = ((-Mathf.Cos(weight* MathF.PI) * 0.5f + 0.5f * (Mathf.Cos(weight * 3f * Mathf.Pi)*0.2f + 0.2f))+0.3f)*1.25f;
        weight = 1.25f*(0.1f * Mathf.Cos(3 * MathF.PI *weight)-0.5f*Mathf.Cos(MathF.PI*weight) +0.4f);
		return from + (to - from) * weight;
    }

    public static Vector2 OvershootSmoothStep(Vector2 from,Vector2 to, float weight)
    {
        //weight = weight * weight * (4f - 3f * weight);
        //weight = ((-Mathf.Cos(weight* MathF.PI) * 0.5f + 0.5f * (Mathf.Cos(weight * 3f * Mathf.Pi)*0.2f + 0.2f))+0.3f)*1.25f;
        weight = 1.25f*(0.1f * Mathf.Cos(3 * MathF.PI *weight)-0.5f*Mathf.Cos(MathF.PI*weight) +0.4f);
		return from + (to - from) * weight;
    }

    public static Vector3 OvershootSmoothStep(Vector3 from,Vector3 to, float weight)
    {
        //weight = weight * weight * (4f - 3f * weight);
        //weight = ((-Mathf.Cos(weight* MathF.PI) * 0.5f + 0.5f * (Mathf.Cos(weight * 3f * Mathf.Pi)*0.2f + 0.2f))+0.3f)*1.25f;
        weight = 1.25f*(0.1f * Mathf.Cos(3 * MathF.PI *weight)-0.5f*Mathf.Cos(MathF.PI*weight) +0.4f);
		return from + (to - from) * weight;
    }

    public static int Mod(int input,int Modulator)
    {
        return (input % Modulator + Modulator) % Modulator;
    }

    public static float Mod(float input,float Modulator)
    {
        return (input % Modulator + Modulator) % Modulator;
    }
} 
