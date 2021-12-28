using Godot;
using System;
using System.Collections.Generic;

public class Cockpit : Control
{
	Label altitudeLabel;
	Label speedLabel;
	Label liftLabel;
	Label weightLabel;
	Label aileronsLabel;
	Label elevatorLabel;
	Label flapsLabel;
	Label rudderLabel;
	Label slatsLabel;
	Label wingsLabel;
	Label enginesLabel;
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
		aileronsLabel = (Label)GetNode("Ailerons");
		elevatorLabel = (Label)GetNode("Elevator");
		flapsLabel = (Label)GetNode("Flaps");
		rudderLabel = (Label)GetNode("Rudder");
		slatsLabel = (Label)GetNode("Slats");
		wingsLabel = (Label)GetNode("Wings");
		enginesLabel = (Label)GetNode("Engines");
		artificialHorizon = (Sprite)GetNode("AH/AHControl");
	}

	public void SetEngines(List<float> thrust)
	{
		if (enginesLabel == null)
			return;
		string text = "";
		for (int i = 0; i < thrust.Count; i++)
		{
			text += $"{i+1}:{thrust[i]}";
		}
		enginesLabel.Text = text;
	}

	public void SetAilerons(Tuple<float, float> lift, Tuple<float, float> drag, Tuple<float, float> side)
	{
		if (aileronsLabel == null)
			return;

		aileronsLabel.Text = $"L {lift.Item1} {lift.Item2} | D {drag.Item1} {drag.Item2} | S {side.Item1} {side.Item2}";
	}

	public void SetFlaps(Tuple<float, float> lift, Tuple<float, float> drag, Tuple<float, float> side)
	{
		if (flapsLabel == null)
			return;

		flapsLabel.Text = $"L {lift.Item1} {lift.Item2} | D {drag.Item1} {drag.Item2} | S {side.Item1} {side.Item2}";
	}

	public void SetSlats(Tuple<float, float> lift, Tuple<float, float> drag, Tuple<float, float> side)
	{
		if (slatsLabel == null)
			return;

		slatsLabel.Text = $"L {lift.Item1} {lift.Item2} | D {drag.Item1} {drag.Item2} | S {side.Item1} {side.Item2}";
	}

	public void SetWings(Tuple<float, float> lift, Tuple<float, float> drag, Tuple<float, float> side)
	{
		if (wingsLabel == null)
			return;

		wingsLabel.Text = $"L {lift.Item1} {lift.Item2} | D {drag.Item1} {drag.Item2} | S {side.Item1} {side.Item2}";
	}

	public void SetElevator(float lift, float drag, float side)
	{
		if (elevatorLabel == null)
			return;

		elevatorLabel.Text = $"L {lift} | D {drag} | S {side}";
	}

	public void SetRudder(float lift, float drag, float side)
	{
		if (rudderLabel == null)
			return;

		rudderLabel.Text = $"L {lift} | D {drag} | S {side}";
	}

	public void SetAltitude(int altitude)
	{
		if (altitudeLabel == null)
			return;

		altitudeLabel.Text = $"{altitude}";
	}

	public void SetLift(float lift, float left, float right)
	{
		if (liftLabel == null)
			return;

		liftLabel.Text = $"{lift} ({left} + {right})";
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
