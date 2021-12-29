using Godot;
using ClientCore.Physics;
using ClientCore.Cockpit;
using Shared;
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
	Label pitchLabel;
	Label rollLabel;
	Label yawLabel;
	Sprite ahControl;
	ArtificialHorizonData ahData;
	public override void _Ready()
	{
		Initialize();
	}

	public void Initialize()
	{
		loadComponents();
		ahData = new ArtificialHorizonData("Resources/Planes/Cockpit/ahContent.png");
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
		pitchLabel = (Label)GetNode("Pitch");
		rollLabel = (Label)GetNode("Roll");
		yawLabel = (Label)GetNode("Yaw");
		ahControl = (Sprite)GetNode("AH/AHControl");
	}

	public void SetEngines(List<Tuple<float, float>> thrust)
	{
		if (enginesLabel == null)
			return;
		string text = "";
		for (int i = 0; i < thrust.Count; i++)
			text += $"{i+1}:{thrust[i].Item1} ({thrust[i].Item2})";
		
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

	public void SetSpeed(PlanePhysics p)//float speed)
	{
		if (speedLabel == null)
			return;

		float x = (float)(p.FlightData.Speed.X * Math.Cos(p.LocalRotation.Z)); //yaw
		float y = (float)(p.FlightData.Speed.Y * Math.Sin(p.LocalRotation.Y)); //pitch
		float z = (float)(p.FlightData.Speed.Z * Math.Cos(p.LocalRotation.X)); //roll
		float tot = (float)(Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2)));
		speedLabel.Text = $"{tot} x {x} y {y} ({p.FlightData.Speed.Y} * {Math.Sin(p.LocalRotation.Y)}) z {z}";
	}

	public void SetWeight(float weight)
	{
		if (weightLabel == null)
			return;

		weightLabel.Text = $"{weight}";
	}

	public void SetPitch(float angleDeg)
	{
		if (pitchLabel == null)
			return;

		string text = $"{angleDeg}";
		pitchLabel.Text = text;
		ahControl.Position = new Vector2(ahControl.Position.x, ahData.GetPitchPixelsForDegree() * angleDeg);
	}

	public void SetYaw(float angleDeg)
	{
		if (yawLabel == null)
			return;

		string text = $"{angleDeg}";
		yawLabel.Text = text;
		//ahControl.Position = new Vector2(ahData.GetYawPixelsForDegree() * angleDeg, ahControl.Position.y);
	}

	public void SetRoll(float angleDeg)
	{
		if (rollLabel == null)
			return;

		string text = $"{angleDeg}";
		rollLabel.Text = text;
		ahControl.RotationDegrees = angleDeg;
	}
}
