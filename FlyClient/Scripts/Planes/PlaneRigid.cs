using Godot;
using System;
using System.Collections.Generic;
using ClientCore;
using ClientCore.Physics;
using ClientCore.Physics.PlaneParts;
using Shared.Objects;

public class PlaneRigid : RigidBody
{
	PlaneBase plane;
	PlanePhysics planePhysics;
	WindPhysics windPhysics;
	Cockpit cockpit;
	AerodynamicsData aerodynamics;
	PlaneData planeData;

	public override void _Ready()
	{
		loadComponents();
		//planePhysics = new PlanePhysics();
		windPhysics = new WindPhysics();
		windPhysics.Direction = new GeoLib.Vector3(1, 0, 0);
		windPhysics.Speed = new GeoLib.Vector3(5, 0, 0);

		//temp /drag,lift,side
		GenericSurfaceData aileronSurface = new GenericSurfaceData(0, 10, 0);
		GenericSurfaceData elevatorSurface = new GenericSurfaceData(0, 10, 0);
		GenericSurfaceData flapSurface = new GenericSurfaceData(0, 5, 0);
		GenericSurfaceData rudderSurface = new GenericSurfaceData(0, 0, 10);
		GenericSurfaceData wingSurface = new GenericSurfaceData(1, 50, 0);
		GenericSurfaceData slatSurface = new GenericSurfaceData(1, 5, 0);
		int length = 25;
		//max,fuel ,restart,surface,acc,dec
		EngineData engine = new EngineData(50, 1, 8000, 1,0.5f, 1.5f);
		List<Tuple<EngineData, Localization>> engines = new List<Tuple<EngineData, Localization>>();
		engines.Add(new Tuple<EngineData, Localization>(engine, Localization.LEFT));
		engines.Add(new Tuple<EngineData, Localization>(engine, Localization.RIGHT));
		planeData = new PlaneData(aileronSurface, elevatorSurface, flapSurface, rudderSurface, slatSurface, wingSurface, engines, length);
		aerodynamics = new AerodynamicsData(planeData);
		//
		ApplyCentralImpulse(new Vector3(0, 0, -0.001f));
	}

	void loadComponents()
	{
		plane = (PlaneBase)GetChild(0);
		cockpit = (Cockpit)GetNode("../../../Cockpit");
	}
	
	public override void _IntegrateForces(PhysicsDirectBodyState state)
	{
		float delta = state.Step;
		float weight = state.TotalGravity.y;
		
		GeoLib.Vector3 velocity = new GeoLib.Vector3(state.LinearVelocity.x, state.LinearVelocity.y, state.LinearVelocity.z);
		GeoLib.Vector3 rotation = new GeoLib.Vector3(RotationDegrees.x, RotationDegrees.y, RotationDegrees.z);
		GeoLib.Vector3 translation = new GeoLib.Vector3(Translation.x, Translation.y, Translation.z);
		FlightData flightData = new FlightData(translation, rotation, velocity);
		planePhysics = new PlanePhysics(flightData, planeData, aerodynamics);
		planePhysics.Update(flightData);

		float leftLift = planePhysics.GetLeftLift(windPhysics);
		float rightLift = planePhysics.GetRightLift(windPhysics);
		float totalLift = leftLift + rightLift;
		float leftLiftPercentage = (float)GeoLib.GameMath.GetPercentage(leftLift, rightLift);

		const float OVERLIFT = 1.9f;
		if (state.LinearVelocity.y >= 0 && totalLift > -weight * delta)
			totalLift = -weight * delta;
		else if (totalLift > -weight * delta * OVERLIFT)
			totalLift = -weight * delta * OVERLIFT;

		leftLift = (totalLift * leftLiftPercentage) / 100f;
		rightLift = totalLift - leftLift;

		Vector3 left = new Vector3((float)planeData.Left.X, 0, (float)planeData.Left.Y);
		Vector3 right = new Vector3((float)planeData.Right.X, 0, (float)planeData.Right.Y);
		Vector3 tail = new Vector3((float)planeData.Tail.X, 0, (float)planeData.Tail.Y);
		state.ApplyImpulse(left, new Vector3(0, leftLift, 0));
		state.ApplyImpulse(right, new Vector3(0, rightLift, 0));

		List<float> thrusts = new List<float>();
		foreach (var e in aerodynamics.Engines)
			thrusts.Add(e.GetThrust(delta, windPhysics.GetDensity(flightData.Altitude)));
		cockpit.SetEngines(thrusts);
		state.ApplyImpulse(left, GlobalTransform.basis.z * thrusts[0] * delta * -1);
		state.ApplyImpulse(right, GlobalTransform.basis.z * thrusts[1] * delta * -1);
		
		cockpit.SetSpeed((float)Math.Sqrt(Math.Pow(state.LinearVelocity.x, 2) + Math.Pow(state.LinearVelocity.z, 2)));
		cockpit.SetLift(totalLift, leftLift, rightLift);
		cockpit.SetAltitude(flightData.Altitude);
		cockpit.SetWeight(weight * delta);

		Tuple<float, float> aLift = new Tuple<float, float>(planePhysics.GetPartLift(aerodynamics.LeftAileron, windPhysics), planePhysics.GetPartLift(aerodynamics.RightAileron, windPhysics));
		Tuple<float, float> aDrag = new Tuple<float, float>(planePhysics.GetPartDrag(aerodynamics.LeftAileron, windPhysics), planePhysics.GetPartDrag(aerodynamics.RightAileron, windPhysics));
		Tuple<float, float> aSide = new Tuple<float, float>(planePhysics.GetPartSide(aerodynamics.LeftAileron, windPhysics), planePhysics.GetPartSide(aerodynamics.RightAileron, windPhysics));
		cockpit.SetAilerons(aLift, aDrag, aSide);

		Tuple<float, float> fLift = new Tuple<float, float>(planePhysics.GetPartLift(aerodynamics.LeftFlap, windPhysics), planePhysics.GetPartLift(aerodynamics.RightFlap, windPhysics));
		Tuple<float, float> fDrag = new Tuple<float, float>(planePhysics.GetPartDrag(aerodynamics.LeftFlap, windPhysics), planePhysics.GetPartDrag(aerodynamics.RightFlap, windPhysics));
		Tuple<float, float> fSide = new Tuple<float, float>(planePhysics.GetPartSide(aerodynamics.LeftFlap, windPhysics), planePhysics.GetPartSide(aerodynamics.RightFlap, windPhysics));
		cockpit.SetFlaps(fLift, fDrag, fSide);

		Tuple<float, float> sLift = new Tuple<float, float>(planePhysics.GetPartLift(aerodynamics.LeftSlat, windPhysics), planePhysics.GetPartLift(aerodynamics.RightSlat, windPhysics));
		Tuple<float, float> sDrag = new Tuple<float, float>(planePhysics.GetPartDrag(aerodynamics.LeftSlat, windPhysics), planePhysics.GetPartDrag(aerodynamics.RightSlat, windPhysics));
		Tuple<float, float> sSide = new Tuple<float, float>(planePhysics.GetPartSide(aerodynamics.LeftSlat, windPhysics), planePhysics.GetPartSide(aerodynamics.RightSlat, windPhysics));
		cockpit.SetSlats(sLift, sDrag, sSide);

		Tuple<float, float> wLift = new Tuple<float, float>(planePhysics.GetPartLift(aerodynamics.LeftWing, windPhysics), planePhysics.GetPartLift(aerodynamics.LeftWing, windPhysics));
		Tuple<float, float> wDrag = new Tuple<float, float>(planePhysics.GetPartDrag(aerodynamics.RightWing, windPhysics), planePhysics.GetPartDrag(aerodynamics.RightWing, windPhysics));
		Tuple<float, float> wSide = new Tuple<float, float>(planePhysics.GetPartSide(aerodynamics.LeftWing, windPhysics), planePhysics.GetPartSide(aerodynamics.RightWing, windPhysics));
		cockpit.SetWings(wLift, wDrag, wSide);

		float rLift = planePhysics.GetPartLift(aerodynamics.Rudder, windPhysics);
		float rDrag = planePhysics.GetPartDrag(aerodynamics.Rudder, windPhysics);
		float rSide = planePhysics.GetPartSide(aerodynamics.Rudder, windPhysics);
		cockpit.SetRudder(rLift, rDrag, rSide);

		float eLift = planePhysics.GetPartLift(aerodynamics.Elevator, windPhysics);
		float eDrag = planePhysics.GetPartDrag(aerodynamics.Elevator, windPhysics);
		float eSide = planePhysics.GetPartSide(aerodynamics.Elevator, windPhysics);
		cockpit.SetElevator(eLift, eDrag, eSide);

		if (Input.IsActionPressed("thrustUp"))
			foreach (var e in aerodynamics.Engines)
				e.Update(true, delta);
		else if (Input.IsActionPressed("thrustDown"))
			foreach (var e in aerodynamics.Engines)
				e.Update(false, delta);

		if (Input.IsActionPressed("pitchUp"))
			aerodynamics.Elevator.Move(true);
		else if (Input.IsActionPressed("pitchDown"))
			aerodynamics.Elevator.Move(false);

		if (Input.IsActionJustPressed("slats"))
		{
			if (Input.IsActionPressed("l"))
				aerodynamics.LeftSlat.Enabled = !aerodynamics.LeftSlat.Enabled;
			if (Input.IsActionPressed("r"))
				aerodynamics.RightSlat.Enabled = !aerodynamics.RightSlat.Enabled;
			else
			{
				aerodynamics.LeftSlat.Enabled = !aerodynamics.LeftSlat.Enabled;
				aerodynamics.RightSlat.Enabled = !aerodynamics.RightSlat.Enabled;
			}
		}

		if (Input.IsActionJustPressed("rollLeft"))
		{
			aerodynamics.LeftAileron.AngleDegrees = 30;
			aerodynamics.RightAileron.AngleDegrees = -30;
		}
		else if (Input.IsActionJustPressed("rollRight"))
		{
			aerodynamics.LeftAileron.AngleDegrees = -30;
			aerodynamics.RightAileron.AngleDegrees = +30;
		}

		aerodynamics.Update(delta);
		state.IntegrateForces();
	}

	void handleInput(PhysicsDirectBodyState state, float delta)
	{
		const float IMPULSE = 0.2f;

		//	state.ApplyCentralImpulse(GlobalTransform.basis.z * IMPULSE * delta * -1);
		/*if (Input.IsActionPressed("ui_down"))
			state.ApplyCentralImpulse(GlobalTransform.basis.z * IMPULSE * delta);
		if (Input.IsActionPressed("ui_left"))
			state.ApplyCentralImpulse(GlobalTransform.basis.x * IMPULSE * delta * -1);
		if (Input.IsActionPressed("ui_right"))
			state.ApplyCentralImpulse(GlobalTransform.basis.x * IMPULSE * delta);*/
	}
}

