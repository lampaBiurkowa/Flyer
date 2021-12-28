using Godot;
using System;

public class Cockpit : Control
{
	Label altitudeLabel;
	Label speedLabel;
	Label liftLabel;
	Label weightLabel;
	Sprite artificialHorizon;
	
	public override void _Ready()
	{
		loadComponents();
	}

	public void Initialize()
	{
		loadComponents();
	}

	void loadComponents()
	{
		liftLabel = (Label)GetNode("Lift");
		speedLabel = (Label)GetNode("Speed");
		altitudeLabel = (Label)GetNode("Altitude");
		weightLabel = (Label)GetNode("Weight");
		artificialHorizon = (Sprite)GetNode("AH/AHControl");
	}

	public void SetAltitude(int altitude)
	{
		if (altitudeLabel == null)
			return;

		altitudeLabel.Text = $"{altitude}";
	}

	public void SetLift(float lift)
	{
		if (liftLabel == null)
			return;

		liftLabel.Text = $"{lift}";
	}

	public void SetSpeed(float speed)
	{
		if (speedLabel == null)
			return;

		speedLabel.Text = $"{speed}";
	}

	public void SetWeight(float weight)
	{
		if (weightLabel == null)
			return;

		weightLabel.Text = $"{weight}";
	}
}
