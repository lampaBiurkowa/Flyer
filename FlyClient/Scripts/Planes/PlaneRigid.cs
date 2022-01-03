using Godot;
using System;
using System.Collections.Generic;
using ClientCore;
using ClientCore.Cockpit;
using ClientCore.Physics;
using ClientCore.Physics.PlaneParts;
using ClientCore.Sky;
using Shared.Objects;
using Shared.Plane;

public class PlaneRigid : RigidBody
{
	PlaneBase plane;
	PlanePhysics planePhysics;
	WindPhysics windPhysics;
	Cockpit cockpit;
	PlaneData planeData;
	MachineData machineData;
	RayCast gear;
	RayCast gpDown;
	RayCast gpForward;
	Camera planeCamera;
	MinimapData minimapData = new MinimapData(1);

	public override void _Ready()
	{
		loadComponents();
		windPhysics = new WindPhysics();
		windPhysics.Direction = new GeoLib.Vector3(1, 0, 0);
		windPhysics.Speed = new GeoLib.Vector3(5, 0, 0);

		//temp /drag,lift,side
		GenericSurfaceData aileronSurface = new GenericSurfaceData(0, 10, 0);
		GenericSurfaceData elevatorSurface = new GenericSurfaceData(0, 1, 0);
		GenericSurfaceData flapSurface = new GenericSurfaceData(0, 1, 0);
		GenericSurfaceData rudderSurface = new GenericSurfaceData(0, 0, 5);
		GenericSurfaceData wingSurface = new GenericSurfaceData(1, 60, 0);
		GenericSurfaceData slatSurface = new GenericSurfaceData(1, 1, 0);
		GenericSurfaceData gearSurface = new GenericSurfaceData(2, 0, 0);
		int length = 40;
		//max,fuel ,restart,surface,acc,dec
		EngineData engine = new EngineData(30, 1, 8000, 1,5f, 2.5f);
		List<Tuple<EngineData, Localization>> engines = new List<Tuple<EngineData, Localization>>();
		engines.Add(new Tuple<EngineData, Localization>(engine, Localization.LEFT));
		engines.Add(new Tuple<EngineData, Localization>(engine, Localization.RIGHT));
		machineData = new MachineData(aileronSurface, elevatorSurface, flapSurface, gearSurface, rudderSurface, slatSurface, wingSurface, engines, length);
		planeData = new PlaneData(machineData);
		//
		Spatial terrain = (Spatial)GetNode("../");
		cockpit.SetTerrainSize(new Vector2(-terrain.Translation.z, -terrain.Translation.x));

		addFog(new Vector3(180, 10, 1800), FogDensity.THICK);
		addFog(new Vector3(150, 50, 1600));
		addFog(new Vector3(250, 200, 1200), FogDensity.THIN);
	}

	void loadComponents()
	{
		plane = (PlaneBase)GetChild(0);
		gear = (RayCast)GetNode("Gear");
		gpDown = (RayCast)GetNode("GPDown");
		gpForward = (RayCast)GetNode("GPForward");
		cockpit = (Cockpit)GetNodeOrNull("../../../Cockpit");//ez
		planeCamera = (Camera)GetNode("Camera");
		planeCamera.Current = true;
	}
	
	void addFog(Vector3 position, FogDensity density = FogDensity.STANDARD)
	{
		PackedScene scene = (PackedScene)ResourceLoader.Load($"Scenes/Sky/Fog.tscn");
		Fog fog = (Fog)scene.Instance();
		fog.Initialize();
		fog.SetDensity(density);
		fog.Translation = position;
		GetNode("../").GetNode("Sky").AddChild(fog);
	}

	public override void _IntegrateForces(PhysicsDirectBodyState state)
	{
		if (state.AngularVelocity.Length() > 1)
		{
			state.AngularVelocity *= new Vector3(0.999f, 0.999f, 0.99f);
		}
		if (Input.IsActionJustPressed("emergency"))
		{
			state.LinearVelocity = new Vector3(0, 0, -50);
			state.Transform = new Transform(new Vector3(-1, 0, 0), new Vector3(0, -1, 0), new Vector3(0, 0, 1), new Vector3(200,200,1850));
			state.AngularVelocity = new Vector3(0, 0, 0);
			return;
		}

		float delta = state.Step;
		float weight = state.TotalGravity.y * Mass;
		
		GeoLib.Vector3 velocity = new GeoLib.Vector3(state.LinearVelocity.x, state.LinearVelocity.y, state.LinearVelocity.z);
		GeoLib.Vector3 rotation = new GeoLib.Vector3(RotationDegrees.x, RotationDegrees.y, RotationDegrees.z + 180);
		GeoLib.Vector3 translation = new GeoLib.Vector3(Translation.x, Translation.y, Translation.z);
		float roll = Rotation.z + (float)GeoLib.GameMath.DegToRad(180);
		float pitch = Rotation.x;
		float yaw = Rotation.y;
		GeoLib.Vector3 localRotation = new GeoLib.Vector3(roll, pitch, yaw);

		FlightData flightData = new FlightData(translation, rotation, velocity);
		planePhysics = new PlanePhysics(flightData, machineData, planeData);
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

		Vector3 left = new Vector3(GlobalTransform.basis.x * (float)machineData.Left.X);
		Vector3 right = new Vector3(GlobalTransform.basis.x * (float)machineData.Right.X);
		Vector3 tail = new Vector3(GlobalTransform.basis.z * (float)machineData.Tail.Y);
		state.ApplyImpulse(left, new Vector3(0, leftLift, 0));
		state.ApplyImpulse(right, new Vector3(0, rightLift, 0));
		GD.Print("-----------------");
		GD.Print($"LIFT-LEFT {new Vector3(0, leftLift, 0)}  {state.LinearVelocity}");
		GD.Print($"LIFT-RIGHT {new Vector3(0, rightLift, 0)}");

		List<Tuple<float, float, float>> thrusts = new List<Tuple<float, float, float>>();
		foreach (var e in planeData.Engines)
			thrusts.Add(new Tuple<float, float, float>(e.CurrentSpeed, e.GetThrust(delta, windPhysics.GetDensity(flightData.Altitude)), 55));
		cockpit.SetEngines(thrusts, 30);
		state.ApplyImpulse(left, GlobalTransform.basis.z * thrusts[0].Item2 * delta * -1);
		state.ApplyImpulse(right, GlobalTransform.basis.z * thrusts[1].Item2 * delta * -1);
		GD.Print($"ENGINE-LEFT {GlobalTransform.basis.z * thrusts[0].Item2 * delta * -1}");
		GD.Print($"ENGINE-RIGHT {GlobalTransform.basis.z * thrusts[1].Item2 * delta * -1}");

		float fallForwardSpeed = planePhysics.GetDiveForwardSpeed() * 0.005f;
		float realYSpeed = state.LinearVelocity.y;
		float realXSpeed = state.LinearVelocity.x + (float)(fallForwardSpeed * Math.Sin(localRotation.Z));
		float realZSpeed = state.LinearVelocity.z + (float)(-fallForwardSpeed * Math.Cos(localRotation.Z));
		state.LinearVelocity = new Vector3(realXSpeed, realYSpeed, realZSpeed);
		GD.Print($"{fallForwardSpeed * Math.Sin(localRotation.Z)} {-fallForwardSpeed * Math.Cos(localRotation.Z)}");
		
		state.ApplyImpulse(left, GlobalTransform.basis.z * planePhysics.GetLeftDrag(windPhysics) * delta);
		state.ApplyImpulse(right, GlobalTransform.basis.z * planePhysics.GetRightDrag(windPhysics) * delta);
		state.ApplyImpulse(tail, GlobalTransform.basis.z * planePhysics.GetTailDrag(windPhysics) * delta);
		GD.Print($"DRAG-LEFT {GlobalTransform.basis.z * planePhysics.GetLeftDrag(windPhysics) * delta}");
		GD.Print($"DRAG-RIGHT {GlobalTransform.basis.z * planePhysics.GetRightDrag(windPhysics) * delta}");
		GD.Print($"DRAG-TAIL {GlobalTransform.basis.z * planePhysics.GetTailDrag(windPhysics) * delta}");

		state.ApplyImpulse(tail, GlobalTransform.basis.x * planePhysics.GetTailSide(windPhysics) * delta);
		state.ApplyImpulse(tail, GlobalTransform.basis.x * -planePhysics.GetCentralSide(windPhysics) * 0.1f * delta * scale);
		//state.ApplyImpulse(tail, GlobalTransform.basis.x * planePhysics.GetCentralSide(windPhysics) * delta * scale * 0.001f);
		GD.Print($"RUDDER-TAIL {GlobalTransform.basis.x * planePhysics.GetTailSide(windPhysics) * delta * scale}");
		GD.Print($"TURN-CENTRE {GlobalTransform.basis.x * -planePhysics.GetCentralSide(windPhysics) * 0.1f *  delta * scale}");
		//GD.Print($"TURN-TAIL {GlobalTransform.basis.x * planePhysics.GetCentralSide(windPhysics) * delta * scale * 0.001f}");

		float elevatorLift = planePhysics.GetPartLift(planeData.Elevator, windPhysics) * delta;// * scale;
		state.ApplyImpulse(tail, GlobalTransform.basis.y * elevatorLift);
		GD.Print($"ELEV-TAIL {GlobalTransform.basis.y * elevatorLift}  {state.LinearVelocity}");

		
		cockpit.SetSpeed(planePhysics);//.GetAirspeed()
		cockpit.SetLift(totalLift, leftLift, rightLift);
		cockpit.SetAltitude(flightData.Altitude);
		cockpit.SetWeight(weight * delta);

		Tuple<float, float> aLift = new Tuple<float, float>(planePhysics.GetPartLift(planeData.LeftAileron, windPhysics), planePhysics.GetPartLift(planeData.RightAileron, windPhysics));
		Tuple<float, float> aDrag = new Tuple<float, float>(planePhysics.GetPartDrag(planeData.LeftAileron, windPhysics), planePhysics.GetPartDrag(planeData.RightAileron, windPhysics));
		Tuple<float, float> aSide = new Tuple<float, float>(planePhysics.GetPartSide(planeData.LeftAileron, windPhysics), planePhysics.GetPartSide(planeData.RightAileron, windPhysics));
		cockpit.SetAilerons(aLift, aDrag, aSide);

		Tuple<float, float> fLift = new Tuple<float, float>(planePhysics.GetPartLift(planeData.LeftFlap, windPhysics), planePhysics.GetPartLift(planeData.RightFlap, windPhysics));
		Tuple<float, float> fDrag = new Tuple<float, float>(planePhysics.GetPartDrag(planeData.LeftFlap, windPhysics), planePhysics.GetPartDrag(planeData.RightFlap, windPhysics));
		Tuple<float, float> fSide = new Tuple<float, float>(planePhysics.GetPartSide(planeData.LeftFlap, windPhysics), planePhysics.GetPartSide(planeData.RightFlap, windPhysics));
		cockpit.SetFlaps(planeData.LeftFlap.CurrentConfiguration,fLift, fDrag, fSide);

		Tuple<float, float> sLift = new Tuple<float, float>(planePhysics.GetPartLift(planeData.LeftSlat, windPhysics), planePhysics.GetPartLift(planeData.RightSlat, windPhysics));
		Tuple<float, float> sDrag = new Tuple<float, float>(planePhysics.GetPartDrag(planeData.LeftSlat, windPhysics), planePhysics.GetPartDrag(planeData.RightSlat, windPhysics));
		Tuple<float, float> sSide = new Tuple<float, float>(planePhysics.GetPartSide(planeData.LeftSlat, windPhysics), planePhysics.GetPartSide(planeData.RightSlat, windPhysics));
		cockpit.SetSlats(planeData.LeftSlat.Enabled, sLift, sDrag, sSide);

		Tuple<float, float> wLift = new Tuple<float, float>(planePhysics.GetPartLift(planeData.LeftWing, windPhysics), planePhysics.GetPartLift(planeData.LeftWing, windPhysics));
		Tuple<float, float> wDrag = new Tuple<float, float>(planePhysics.GetPartDrag(planeData.RightWing, windPhysics), planePhysics.GetPartDrag(planeData.RightWing, windPhysics));
		Tuple<float, float> wSide = new Tuple<float, float>(planePhysics.GetPartSide(planeData.LeftWing, windPhysics), planePhysics.GetPartSide(planeData.RightWing, windPhysics));
		cockpit.SetWings(wLift, wDrag, wSide);

		float rLift = planePhysics.GetPartLift(planeData.Rudder, windPhysics);
		float rDrag = planePhysics.GetPartDrag(planeData.Rudder, windPhysics);
		float rSide = planePhysics.GetPartSide(planeData.Rudder, windPhysics);
		cockpit.SetRudder(rLift, rDrag, rSide);

		float eLift = planePhysics.GetPartLift(planeData.Elevator, windPhysics);
		float eDrag = planePhysics.GetPartDrag(planeData.Elevator, windPhysics);
		float eSide = planePhysics.GetPartSide(planeData.Elevator, windPhysics);
		cockpit.SetElevator(eLift, eDrag, eSide);

		float gLift = planePhysics.GetPartLift(planeData.Gear, windPhysics);
		float gDrag = planePhysics.GetPartDrag(planeData.Gear, windPhysics);
		float gSide = planePhysics.GetPartSide(planeData.Gear, windPhysics);
		cockpit.SetGear(planeData.Gear.Enabled, gLift, gDrag, gSide);

		cockpit.SetPitch((float)GeoLib.GameMath.RadToDeg(pitch));
		cockpit.SetRoll((float)GeoLib.GameMath.RadToDeg(roll));
		cockpit.SetYaw((float)GeoLib.GameMath.RadToDeg(yaw));

		cockpit.SetAH((float)GeoLib.GameMath.RadToDeg(pitch), (float)GeoLib.GameMath.RadToDeg(roll));
		cockpit.SetMinimap(Translation.x, Translation.z, (float)GeoLib.GameMath.RadToDeg(yaw));
		cockpit.SetTurnCoordinator((float)GeoLib.GameMath.RadToDeg(yaw), (float)GeoLib.GameMath.RadToDeg(roll));

		if (Input.IsActionPressed("thrustUp"))
		{
			if (Input.IsActionPressed("1") && planeData.Engines.Count >= 1)
				planeData.Engines[0].Update(true, delta);
			else if (Input.IsActionPressed("2") && planeData.Engines.Count >= 2)
				planeData.Engines[1].Update(true, delta);
			else if (Input.IsActionPressed("3") && planeData.Engines.Count >= 3)
				planeData.Engines[2].Update(true, delta);
			else if (Input.IsActionPressed("4") && planeData.Engines.Count >= 4)
				planeData.Engines[3].Update(true, delta);
			else
				foreach (var e in planeData.Engines)
					e.Update(true, delta);
		}
		else if (Input.IsActionPressed("thrustDown"))
		{
			if (Input.IsActionPressed("1") && planeData.Engines.Count >= 1)
				planeData.Engines[0].Update(false, delta);
			else if (Input.IsActionPressed("2") && planeData.Engines.Count >= 2)
				planeData.Engines[1].Update(false, delta);
			else if (Input.IsActionPressed("3") && planeData.Engines.Count >= 3)
				planeData.Engines[2].Update(false, delta);
			else if (Input.IsActionPressed("4") && planeData.Engines.Count >= 4)
				planeData.Engines[3].Update(false, delta);
			else
				foreach (var e in planeData.Engines)
					e.Update(false, delta);
		}

		if (Input.IsActionPressed("pitchUp"))
			planeData.Elevator.Move(false);
		else if (Input.IsActionPressed("pitchDown"))
			planeData.Elevator.Move(true);
		else
			planeData.Elevator.Level();

		if (Input.IsActionPressed("rudderLeft"))
			planeData.Rudder.Move(false);
		else if (Input.IsActionPressed("rudderRight"))
			planeData.Rudder.Move(true);
		else
			planeData.Rudder.Level();

		if (Input.IsActionPressed("rollLeft"))
		{
			planeData.LeftAileron.Move(false);
			planeData.RightAileron.Move(true);
		}
		else if (Input.IsActionPressed("rollRight"))
		{
			planeData.LeftAileron.Move(true);
			planeData.RightAileron.Move(false);
		}
		else
		{
			planeData.LeftAileron.Level();
			planeData.RightAileron.Level();
		}

		if (Input.IsActionJustPressed("slats"))
		{
			if (Input.IsActionPressed("left"))
				planeData.LeftSlat.Enabled = !planeData.LeftSlat.Enabled;
			if (Input.IsActionPressed("right"))
				planeData.RightSlat.Enabled = !planeData.RightSlat.Enabled;
			else
			{
				planeData.LeftSlat.Enabled = !planeData.LeftSlat.Enabled;
				planeData.RightSlat.Enabled = !planeData.RightSlat.Enabled;
			}
		}

		if (Input.IsActionJustPressed("flaps"))
		{
			if (Input.IsActionPressed("left"))
				planeData.LeftFlap.SwitchConfiguration();
			if (Input.IsActionPressed("right"))
				planeData.RightFlap.SwitchConfiguration();
			else
			{
				planeData.LeftFlap.SwitchConfiguration();
				planeData.RightFlap.SwitchConfiguration();
			}
		}

		if (Input.IsActionJustPressed("gear"))
			planeData.Gear.Enabled = !planeData.Gear.Enabled;

		if (Input.IsActionPressed("brakes") && gearTouchingGround())
		{
			planeData.Brakes.Enabled = true;
			state.LinearVelocity *= new Vector3(1, 1, planeData.Brakes.Decceleration);
		}
		else
			planeData.Brakes.Enabled = false;

		if (Input.IsActionJustPressed("heightmap"))
			cockpit.SwitchHeightmap();
		
		planeData.Update(delta);
	}

	bool gearTouchingGround() => planeData.Gear.Enabled && gear.IsColliding();
}

