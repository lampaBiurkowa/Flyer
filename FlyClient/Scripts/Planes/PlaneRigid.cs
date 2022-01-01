using Godot;
using System;
using System.Collections.Generic;
using ClientCore;
using ClientCore.Physics;
using ClientCore.Physics.PlaneParts;
using Shared;
using Shared.Objects;

public class PlaneRigid : RigidBody
{
	PlaneBase plane;
	PlanePhysics planePhysics;
	WindPhysics windPhysics;
	Cockpit cockpit;
	AerodynamicsData aerodynamics;
	PlaneData planeData;
	RayCast gear;
	RayCast gpDown;
	RayCast gpForward;

	float brakesDec = 0.999f;

	public override void _Ready()
	{
		loadComponents();
		windPhysics = new WindPhysics();
		windPhysics.Direction = new GeoLib.Vector3(1, 0, 0);
		windPhysics.Speed = new GeoLib.Vector3(5, 0, 0);

		//temp /drag,lift,side
		GenericSurfaceData aileronSurface = new GenericSurfaceData(0, 3, 0);
		GenericSurfaceData elevatorSurface = new GenericSurfaceData(0, 2, 0);
		GenericSurfaceData flapSurface = new GenericSurfaceData(0, 1, 0);
		GenericSurfaceData rudderSurface = new GenericSurfaceData(0, 0, 10);
		GenericSurfaceData wingSurface = new GenericSurfaceData(1, 60, 0);
		GenericSurfaceData slatSurface = new GenericSurfaceData(1, 1, 0);
		GenericSurfaceData gearSurface = new GenericSurfaceData(2, 0, 0);
		int length = 40;
		//max,fuel ,restart,surface,acc,dec
		EngineData engine = new EngineData(30, 1, 8000, 1,5f, 2.5f);
		List<Tuple<EngineData, Localization>> engines = new List<Tuple<EngineData, Localization>>();
		engines.Add(new Tuple<EngineData, Localization>(engine, Localization.LEFT));
		engines.Add(new Tuple<EngineData, Localization>(engine, Localization.RIGHT));
		planeData = new PlaneData(aileronSurface, elevatorSurface, flapSurface, gearSurface, rudderSurface, slatSurface, wingSurface, engines, length);
		aerodynamics = new AerodynamicsData(planeData);
		//
	}

	void loadComponents()
	{
		plane = (PlaneBase)GetChild(0);
		gear = (RayCast)GetNode("Gear");
		gpDown = (RayCast)GetNode("GPDown");
		gpForward = (RayCast)GetNode("GPForward");
		cockpit = (Cockpit)GetNodeOrNull("../../../Cockpit");//ez
	}
	
	public override void _IntegrateForces(PhysicsDirectBodyState state)
	{
		float delta = state.Step;
		float weight = state.TotalGravity.y * Mass;
		
		GeoLib.Vector3 velocity = new GeoLib.Vector3(state.LinearVelocity.x, state.LinearVelocity.y, state.LinearVelocity.z);
		GeoLib.Vector3 rotation = new GeoLib.Vector3(RotationDegrees.x, RotationDegrees.y, RotationDegrees.z);
		GeoLib.Vector3 translation = new GeoLib.Vector3(Translation.x, Translation.y, Translation.z);
		float localRotationScale = (float)GeoLib.GameMath.DegToRad(90);
		float roll = -GlobalTransform.basis.y.x * localRotationScale;
		float pitch = GlobalTransform.basis.y.z * localRotationScale;
		float yaw = GlobalTransform.basis.z.x * localRotationScale;
		GeoLib.Vector3 localRotation = new GeoLib.Vector3(roll, pitch, yaw);

		FlightData flightData = new FlightData(translation, rotation, velocity);
		planePhysics = new PlanePhysics(flightData, planeData, aerodynamics);
		planePhysics.Update(flightData, localRotation);

		float leftLift = planePhysics.GetLeftLift(windPhysics) * delta;
		float rightLift = planePhysics.GetRightLift(windPhysics) * delta;
		float totalLift = leftLift + rightLift;
		float originalTotalLift = totalLift;
		float leftLiftPercentage = (float)GeoLib.GameMath.GetPercentage(leftLift, rightLift);

		if (totalLift > -weight * delta)
			totalLift = -weight * delta;

		float scale = (float)GeoLib.GameMath.GetPercentage(totalLift, originalTotalLift) / 100f;
		leftLift = (totalLift * leftLiftPercentage) / 100f;
		rightLift = totalLift - leftLift;

		Vector3 left = new Vector3(GlobalTransform.basis.x * (float)planeData.Left.X);
		Vector3 right = new Vector3(GlobalTransform.basis.x * (float)planeData.Right.X);
		Vector3 tail = new Vector3(GlobalTransform.basis.z * (float)planeData.Tail.Y);
		state.ApplyImpulse(left, new Vector3(0, leftLift, 0));
		state.ApplyImpulse(right, new Vector3(0, rightLift, 0));

		List<Tuple<float, float, float>> thrusts = new List<Tuple<float, float, float>>();
		foreach (var e in aerodynamics.Engines)
			thrusts.Add(new Tuple<float, float, float>(e.CurrentSpeed, e.GetThrust(delta, windPhysics.GetDensity(flightData.Altitude)), 55));
		cockpit.SetEngines(thrusts, 30);
		state.ApplyImpulse(left, GlobalTransform.basis.z * thrusts[0].Item2 * delta * -1);
		state.ApplyImpulse(right, GlobalTransform.basis.z * thrusts[1].Item2 * delta * -1);

		float fallForwardSpeed = planePhysics.GetDiveForwardSpeed() * 0.005f;
		float realYSpeed = state.LinearVelocity.y;
		float realXSpeed = state.LinearVelocity.x + (float)(fallForwardSpeed * Math.Sin(localRotation.Z));
		float realZSpeed = state.LinearVelocity.z + (float)(-fallForwardSpeed * Math.Cos(localRotation.Z));
		state.LinearVelocity = new Vector3(realXSpeed, realYSpeed, realZSpeed);
		
		state.ApplyImpulse(left, new Vector3(0, 0, planePhysics.GetLeftDrag(windPhysics) * delta));
		state.ApplyImpulse(right, new Vector3(0, 0, planePhysics.GetRightDrag(windPhysics) * delta));
		state.ApplyImpulse(tail, new Vector3(0, 0, planePhysics.GetTailDrag(windPhysics) * delta));

		state.ApplyImpulse(tail, new Vector3(planePhysics.GetTotalSide(windPhysics) * delta, 0 ,0));

		float elevatorLift = planePhysics.GetPartLift(aerodynamics.Elevator, windPhysics) * delta;// * scale;
		state.ApplyImpulse(tail, new Vector3(0, elevatorLift, 0));
		
		cockpit.SetSpeed(planePhysics);//.GetAirspeed()
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
		cockpit.SetFlaps(aerodynamics.LeftFlap.CurrentConfiguration,fLift, fDrag, fSide);

		Tuple<float, float> sLift = new Tuple<float, float>(planePhysics.GetPartLift(aerodynamics.LeftSlat, windPhysics), planePhysics.GetPartLift(aerodynamics.RightSlat, windPhysics));
		Tuple<float, float> sDrag = new Tuple<float, float>(planePhysics.GetPartDrag(aerodynamics.LeftSlat, windPhysics), planePhysics.GetPartDrag(aerodynamics.RightSlat, windPhysics));
		Tuple<float, float> sSide = new Tuple<float, float>(planePhysics.GetPartSide(aerodynamics.LeftSlat, windPhysics), planePhysics.GetPartSide(aerodynamics.RightSlat, windPhysics));
		cockpit.SetSlats(aerodynamics.LeftSlat.Enabled, sLift, sDrag, sSide);

		Tuple<float, float> wLift = new Tuple<float, float>(planePhysics.GetPartLift(aerodynamics.LeftWing, windPhysics), planePhysics.GetPartLift(aerodynamics.LeftWing, windPhysics));
		Tuple<float, float> wDrag = new Tuple<float, float>(planePhysics.GetPartDrag(aerodynamics.RightWing, windPhysics), planePhysics.GetPartDrag(aerodynamics.RightWing, windPhysics));
		Tuple<float, float> wSide = new Tuple<float, float>(planePhysics.GetPartSide(aerodynamics.LeftWing, windPhysics), planePhysics.GetPartSide(aerodynamics.RightWing, windPhysics));
		cockpit.SetWings(wLift, wDrag, wSide);

		float rLift = planePhysics.GetPartLift(aerodynamics.Rudder, windPhysics);
		float rDrag = planePhysics.GetPartDrag(aerodynamics.Rudder, windPhysics);
		float rSide = planePhysics.GetPartSide(aerodynamics.Rudder, windPhysics);
		cockpit.SetRudder(rLift, rDrag, rSide);

		float eLift = planePhysics.GetPartLift(aerodynamics.Elevator, windPhysics) * scale;
		float eDrag = planePhysics.GetPartDrag(aerodynamics.Elevator, windPhysics);
		float eSide = planePhysics.GetPartSide(aerodynamics.Elevator, windPhysics);
		cockpit.SetElevator(eLift, eDrag, eSide);

		float gLift = planePhysics.GetPartLift(aerodynamics.Gear, windPhysics);
		float gDrag = planePhysics.GetPartDrag(aerodynamics.Gear, windPhysics);
		float gSide = planePhysics.GetPartSide(aerodynamics.Gear, windPhysics);
		cockpit.SetGear(aerodynamics.Gear.Enabled, gLift, gDrag, gSide);

		cockpit.SetPitch((float)GeoLib.GameMath.RadToDeg(pitch));
		cockpit.SetRoll((float)GeoLib.GameMath.RadToDeg(roll));
		cockpit.SetYaw((float)GeoLib.GameMath.RadToDeg(yaw));

		cockpit.SetAH((float)GeoLib.GameMath.RadToDeg(pitch), (float)GeoLib.GameMath.RadToDeg(roll));

		if (Input.IsActionPressed("thrustUp"))
		{
			if (Input.IsActionPressed("1") && aerodynamics.Engines.Count >= 1)
				aerodynamics.Engines[0].Update(true, delta);
			else if (Input.IsActionPressed("2") && aerodynamics.Engines.Count >= 2)
				aerodynamics.Engines[1].Update(true, delta);
			else if (Input.IsActionPressed("3") && aerodynamics.Engines.Count >= 3)
				aerodynamics.Engines[2].Update(true, delta);
			else if (Input.IsActionPressed("4") && aerodynamics.Engines.Count >= 4)
				aerodynamics.Engines[3].Update(true, delta);
			else
				foreach (var e in aerodynamics.Engines)
					e.Update(true, delta);
		}
		else if (Input.IsActionPressed("thrustDown"))
		{
			if (Input.IsActionPressed("1") && aerodynamics.Engines.Count >= 1)
				aerodynamics.Engines[0].Update(false, delta);
			else if (Input.IsActionPressed("2") && aerodynamics.Engines.Count >= 2)
				aerodynamics.Engines[1].Update(false, delta);
			else if (Input.IsActionPressed("3") && aerodynamics.Engines.Count >= 3)
				aerodynamics.Engines[2].Update(false, delta);
			else if (Input.IsActionPressed("4") && aerodynamics.Engines.Count >= 4)
				aerodynamics.Engines[3].Update(false, delta);
			else
				foreach (var e in aerodynamics.Engines)
					e.Update(false, delta);
		}

		if (Input.IsActionPressed("pitchUp"))
			aerodynamics.Elevator.Move(true);
		else if (Input.IsActionPressed("pitchDown"))
			aerodynamics.Elevator.Move(false);
		else
			aerodynamics.Elevator.Level();

		if (Input.IsActionPressed("rudderLeft"))
			aerodynamics.Rudder.Move(true);
		else if (Input.IsActionPressed("rudderRight"))
			aerodynamics.Rudder.Move(false);
		else
			aerodynamics.Rudder.Level();

		if (Input.IsActionPressed("rollLeft"))
		{
			aerodynamics.LeftAileron.Move(true);
			aerodynamics.RightAileron.Move(false);
		}
		else if (Input.IsActionPressed("rollRight"))
		{
			aerodynamics.LeftAileron.Move(false);
			aerodynamics.RightAileron.Move(true);
		}
		else
		{
			aerodynamics.LeftAileron.Level();
			aerodynamics.RightAileron.Level();
		}

		if (Input.IsActionJustPressed("slats"))
		{
			if (Input.IsActionPressed("left"))
				aerodynamics.LeftSlat.Enabled = !aerodynamics.LeftSlat.Enabled;
			if (Input.IsActionPressed("right"))
				aerodynamics.RightSlat.Enabled = !aerodynamics.RightSlat.Enabled;
			else
			{
				aerodynamics.LeftSlat.Enabled = !aerodynamics.LeftSlat.Enabled;
				aerodynamics.RightSlat.Enabled = !aerodynamics.RightSlat.Enabled;
			}
		}

		if (Input.IsActionJustPressed("flaps"))
		{
			if (Input.IsActionPressed("left"))
				aerodynamics.LeftFlap.SwitchConfiguration();
			if (Input.IsActionPressed("right"))
				aerodynamics.RightFlap.SwitchConfiguration();
			else
			{
				aerodynamics.LeftFlap.SwitchConfiguration();
				aerodynamics.RightFlap.SwitchConfiguration();
			}
		}

		if (Input.IsActionJustPressed("gear"))
			aerodynamics.Gear.Enabled = !aerodynamics.Gear.Enabled;

		if (Input.IsActionPressed("brakes") && gearTouchingGround())
			state.LinearVelocity *= new Vector3(1, 1, brakesDec);

		if (gearTouchingGround())
		{
			Vector3 groundNormal = gear.GetCollisionNormal();
			Transform landingRotation = state.Transform;
			landingRotation.basis.y = groundNormal;
			landingRotation.basis.x = -landingRotation.basis.z.Cross(groundNormal);
			landingRotation.basis = landingRotation.basis.Orthonormalized();
			state.Transform = state.Transform.InterpolateWith(landingRotation, 0.05f);
		}
		
		aerodynamics.Update(delta);
	}

	bool gearTouchingGround() => aerodynamics.Gear.Enabled && gear.IsColliding();
}

