using Godot;
using ClientCore.Physics;
using ClientCore.Cockpit;
using ClientCore.Physics.PlaneParts;
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
	Label gearLabel;
	Label rudderLabel;
	Label slatsLabel;
	Label wingsLabel;
	Label enginesLabel;
	Label pitchLabel;
	Label rollLabel;
	Label yawLabel;
	BasicT basicT;
	LeverPanel leverPanel;
	Minimap minimap;
	Radar radar;
	List<EnginePanel> enginePanels = new List<EnginePanel>();
	float previousYaw = 0;
	Vector2 terrainSize;
	public override void _Ready()
	{
		Initialize();
	}

	public void Initialize()
	{
		loadComponents();
		minimap.SetTerrainSize(terrainSize);
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
		gearLabel = (Label)GetNode("Gear");
		rudderLabel = (Label)GetNode("Rudder");
		slatsLabel = (Label)GetNode("Slats");
		wingsLabel = (Label)GetNode("Wings");
		enginesLabel = (Label)GetNode("Engines");
		pitchLabel = (Label)GetNode("Pitch");
		rollLabel = (Label)GetNode("Roll");
		yawLabel = (Label)GetNode("Yaw");
		basicT = (BasicT)GetNode("BasicT");
		leverPanel = (LeverPanel)GetNode("LeverPanel");
		minimap = (Minimap)GetNode("NavigationPanel").GetNode("Minimap");
		radar = (Radar)GetNode("NavigationPanel").GetNode("Radar");
		enginePanels.Add((EnginePanel)GetNode("Engine1Panel"));
		enginePanels.Add((EnginePanel)GetNode("Engine2Panel"));
	}

	public void SetEngines(List<Tuple<float, float, float>> thrust, float maxThrust)
	{
		if (enginesLabel == null)
			return;
		string text = "";
		for (int i = 0; i < thrust.Count; i++)
		{
			float thrustPercentage = (thrust[i].Item1 / maxThrust) * 100;
			enginePanels[i].SetFuel(thrust[i].Item3 / 10);
			enginePanels[i].SetThrustPercentage(thrustPercentage);
			text += $"{i+1}:{thrust[i].Item1} ({thrust[i].Item2} {thrustPercentage} {thrustPercentage % 10})";
		}
		
		enginesLabel.Text = text;
	}

	public void SetAilerons(Tuple<float, float> lift, Tuple<float, float> drag, Tuple<float, float> side)
	{
		if (aileronsLabel == null)
			return;

		aileronsLabel.Text = $"L {lift.Item1} {lift.Item2} | D {drag.Item1} {drag.Item2} | S {side.Item1} {side.Item2}";
	}

	public void SetFlaps(int flapConfiguration, Tuple<float, float> lift, Tuple<float, float> drag, Tuple<float, float> side)
	{
		if (flapsLabel == null)
			return;

		flapsLabel.Text = $"L {lift.Item1} {lift.Item2} | D {drag.Item1} {drag.Item2} | S {side.Item1} {side.Item2}";
		
		int state = (int)Flap.CONFIGURATION_COUNT - flapConfiguration - 1;
		leverPanel.FlapsLever.Placement = state;
	}


	public void SetSlats(bool enabled, Tuple<float, float> lift, Tuple<float, float> drag, Tuple<float, float> side)
	{
		if (slatsLabel == null)
			return;

		slatsLabel.Text = $"L {lift.Item1} {lift.Item2} | D {drag.Item1} {drag.Item2} | S {side.Item1} {side.Item2}";
		leverPanel.SlatsLever.Placement = enabled ? 0 : 1;
	}

	public void SetWings(Tuple<float, float> lift, Tuple<float, float> drag, Tuple<float, float> side)
	{
		if (wingsLabel == null)
			return;

		wingsLabel.Text = $"L {lift.Item1} {lift.Item2} | D {drag.Item1} {drag.Item2} | S {side.Item1} {side.Item2}";
	}

	public void SetGear(bool enabled, float lift, float drag, float side)
	{
		if (gearLabel == null)
			return;

		gearLabel.Text = $"L {lift} | D {drag} | S {side}";
		leverPanel.GearLever.Placement = enabled ? 0 : 1;
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

		basicT.SetAltimeter(altitude);
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
			
		speedLabel.Text = $"{p.GetAirspeed()} x {p.FlightData.Speed.X} y {p.FlightData.Speed.Y} z {p.FlightData.Speed.Z}";
	
		basicT.SetAirspeed(p.GetAirspeed());
		basicT.SetVerticalSpeed((float)p.FlightData.Speed.Y);
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
	}

	public void SetYaw(float angleDeg)
	{
		if (yawLabel == null)
			return;

		string text = $"{angleDeg}";
		yawLabel.Text = text;

		basicT.SetHeading(angleDeg);
		basicT.SetTurnCoordinator(angleDeg - previousYaw);
		previousYaw = angleDeg;
	}

	public void SetRoll(float angleDeg)
	{
		if (rollLabel == null)
			return;

		string text = $"{angleDeg}";
		rollLabel.Text = text;
	}

	public void SetAH(float pitch, float roll)
	{
		basicT.SetAH(pitch, roll);
	}

	public void SetMinimap(float x, float z, float yaw)
	{
		minimap.Update(new Vector2(z, x), yaw);
	}

	public void SetTerrainSize(Vector2 size)
	{
		terrainSize = size;
	}
}
