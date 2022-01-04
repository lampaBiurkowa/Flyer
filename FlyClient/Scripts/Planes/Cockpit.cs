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

	public void Update(PlanePhysics data, float delta, WindPhysics windPhysics, float weight, float totalLift, float leftLift, float rightLift)
	{
		SetSpeed(data);//.GetAirspeed()
		SetLift(totalLift, leftLift, rightLift);
		SetAltitude(data.Plane.Flight.Altitude);
		SetWeight(-weight * delta);

		Tuple<float, float> aLift = new Tuple<float, float>(data.GetPartLift(data.Plane.Configuration.LeftAileron, windPhysics), data.GetPartLift(data.Plane.Configuration.RightAileron, windPhysics));
		Tuple<float, float> aDrag = new Tuple<float, float>(data.GetPartDrag(data.Plane.Configuration.LeftAileron, windPhysics), data.GetPartDrag(data.Plane.Configuration.RightAileron, windPhysics));
		Tuple<float, float> aSide = new Tuple<float, float>(data.GetPartSide(data.Plane.Configuration.LeftAileron, windPhysics), data.GetPartSide(data.Plane.Configuration.RightAileron, windPhysics));
		SetAilerons(aLift, aDrag, aSide);

		Tuple<float, float> fLift = new Tuple<float, float>(data.GetPartLift(data.Plane.Configuration.LeftFlap, windPhysics), data.GetPartLift(data.Plane.Configuration.RightFlap, windPhysics));
		Tuple<float, float> fDrag = new Tuple<float, float>(data.GetPartDrag(data.Plane.Configuration.LeftFlap, windPhysics), data.GetPartDrag(data.Plane.Configuration.RightFlap, windPhysics));
		Tuple<float, float> fSide = new Tuple<float, float>(data.GetPartSide(data.Plane.Configuration.LeftFlap, windPhysics), data.GetPartSide(data.Plane.Configuration.RightFlap, windPhysics));
		SetFlaps(data.Plane.Configuration.LeftFlap.CurrentConfiguration,fLift, fDrag, fSide);

		Tuple<float, float> sLift = new Tuple<float, float>(data.GetPartLift(data.Plane.Configuration.LeftSlat, windPhysics), data.GetPartLift(data.Plane.Configuration.RightSlat, windPhysics));
		Tuple<float, float> sDrag = new Tuple<float, float>(data.GetPartDrag(data.Plane.Configuration.LeftSlat, windPhysics), data.GetPartDrag(data.Plane.Configuration.RightSlat, windPhysics));
		Tuple<float, float> sSide = new Tuple<float, float>(data.GetPartSide(data.Plane.Configuration.LeftSlat, windPhysics), data.GetPartSide(data.Plane.Configuration.RightSlat, windPhysics));
		SetSlats(data.Plane.Configuration.LeftSlat.Enabled, sLift, sDrag, sSide);

		Tuple<float, float> wLift = new Tuple<float, float>(data.GetPartLift(data.Plane.Configuration.LeftWing, windPhysics), data.GetPartLift(data.Plane.Configuration.LeftWing, windPhysics));
		Tuple<float, float> wDrag = new Tuple<float, float>(data.GetPartDrag(data.Plane.Configuration.RightWing, windPhysics), data.GetPartDrag(data.Plane.Configuration.RightWing, windPhysics));
		Tuple<float, float> wSide = new Tuple<float, float>(data.GetPartSide(data.Plane.Configuration.LeftWing, windPhysics), data.GetPartSide(data.Plane.Configuration.RightWing, windPhysics));
		SetWings(wLift, wDrag, wSide);

		float rLift = data.GetPartLift(data.Plane.Configuration.Rudder, windPhysics);
		float rDrag = data.GetPartDrag(data.Plane.Configuration.Rudder, windPhysics);
		float rSide = data.GetPartSide(data.Plane.Configuration.Rudder, windPhysics);
		SetRudder(rLift, rDrag, rSide);

		float eLift = data.GetPartLift(data.Plane.Configuration.Elevator, windPhysics);
		float eDrag = data.GetPartDrag(data.Plane.Configuration.Elevator, windPhysics);
		float eSide = data.GetPartSide(data.Plane.Configuration.Elevator, windPhysics);
		SetElevator(eLift, eDrag, eSide);

		float gLift = data.GetPartLift(data.Plane.Configuration.Gear, windPhysics);
		float gDrag = data.GetPartDrag(data.Plane.Configuration.Gear, windPhysics);
		float gSide = data.GetPartSide(data.Plane.Configuration.Gear, windPhysics);
		SetGear(data.Plane.Configuration.Gear.Enabled, gLift, gDrag, gSide);

		SetPitch((float)GeoLib.GameMath.RadToDeg(data.Plane.Flight.Pitch));
		SetRoll((float)GeoLib.GameMath.RadToDeg(data.Plane.Flight.Roll));
		SetYaw((float)GeoLib.GameMath.RadToDeg(data.Plane.Flight.Yaw));

		SetAH((float)GeoLib.GameMath.RadToDeg(data.Plane.Flight.Pitch), (float)GeoLib.GameMath.RadToDeg(data.Plane.Flight.Roll));
		SetMinimap(data.Plane.Flight.Position.X, data.Plane.Flight.Position.Z, (float)GeoLib.GameMath.RadToDeg(data.Plane.Flight.Yaw));
		SetTurnCoordinator((float)GeoLib.GameMath.RadToDeg(data.Plane.Flight.Yaw), (float)GeoLib.GameMath.RadToDeg(data.Plane.Flight.Roll));
	}


	public void SetEngines(List<Tuple<float, float, float>> thrust, float maxThrust)
	{
		if (enginesLabel == null)
			return;
		string text = "";
		for (int i = 0; i < thrust.Count; i++)
		{
			float thrustPercentage = (thrust[i].Item1 / maxThrust) * 100;
			enginePanels[i].SetFuel(thrust[i].Item3);
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
			
		speedLabel.Text = $"{p.GetAirspeed()} x {p.Plane.Flight.Speed.X} y {p.Plane.Flight.Speed.Y} z {p.Plane.Flight.Speed.Z}";
	
		basicT.SetAirspeed(p.GetAirspeed());
		basicT.SetVerticalSpeed(p.Plane.Flight.Speed.Y);
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
	}

	public void SetTurnCoordinator(float yaw, float roll)
	{
		basicT.SetTurnCoordinator(yaw, yaw - previousYaw, roll);
		previousYaw = yaw;
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

	public void SwitchHeightmap()
	{
		minimap.HeightmapEnabled = !minimap.HeightmapEnabled;
	}
	public void SetTerrainSize(Vector2 size)
	{
		terrainSize = size;
	}
}
