using Godot;
using System;

public class LeverPanel : Panel
{
	public Lever FlapsLever {get; private set;}
	public Lever GearLever {get; private set;}
	public Lever SlatsLever {get; private set;}

	public override void _Ready()
	{
		loadComponents();
		initializeLevers();
	}

	void loadComponents()
	{
		FlapsLever = (Lever)GetNode("FlapsLever");
		GearLever = (Lever)GetNode("GearLever");
		SlatsLever = (Lever)GetNode("SlatsLever");
	}

	void initializeLevers()
	{
		FlapsLever.Initialize("flaps", 3, 2);
		GearLever.Initialize("gear", 2, 1);
		SlatsLever.Initialize("slats", 2, 1);
	}
}
