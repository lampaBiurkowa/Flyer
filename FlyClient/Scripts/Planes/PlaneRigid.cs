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
		windPhysics.Direction = new GeoLib.Vector3F(1, 0, 0);
		windPhysics.Speed = new GeoLib.Vector3F(5, 0, 0);

		//temp /drag,lift,side
		GenericSurfaceData aileronSurface = new GenericSurfaceData(0, 10, 0);
		GenericSurfaceData elevatorSurface = new GenericSurfaceData(0, 1, 0);
		GenericSurfaceData flapSurface = new GenericSurfaceData(0, 1, 0);
		GenericSurfaceData rudderSurface = new GenericSurfaceData(0, 0, 5);
		GenericSurfaceData wingSurface = new GenericSurfaceData(1, 60, 0);
		GenericSurfaceData slatSurface = new GenericSurfaceData(1, 1, 0);
		GenericSurfaceData gearSurface = new GenericSurfaceData(2, 0, 0);
		int length = 40;
		//max,fuelc, fuelpersec ,restart,surface,acc,dec
		EngineData engine = new EngineData(30, 8000, 1,5f, 2.5f);
		List<Tuple<EngineData, Localization>> engines = new List<Tuple<EngineData, Localization>>();
		engines.Add(new Tuple<EngineData, Localization>(engine, Localization.LEFT));
		engines.Add(new Tuple<EngineData, Localization>(engine, Localization.RIGHT));
		machineData = new MachineData(aileronSurface, elevatorSurface, flapSurface, gearSurface, rudderSurface, slatSurface, wingSurface, engines, length, 300);
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
		handleAngularDamp(state);
		updateMass();

		float delta = state.Step;
		planeData.Flight = getFlightData(state);
		planePhysics = new PlanePhysics(planeData);
		Vector3 left = new Vector3(GlobalTransform.basis.x * planeData.Machine.Left.X);
		Vector3 right = new Vector3(GlobalTransform.basis.x * planeData.Machine.Right.X);
		Vector3 tail = new Vector3(GlobalTransform.basis.z * planeData.Machine.Tail.Y);

		float leftLift = planePhysics.GetLeftLift(windPhysics) * delta;
		float rightLift = planePhysics.GetRightLift(windPhysics) * delta;
		float totalLift = leftLift + rightLift;
		float originalTotalLift = totalLift;
		float leftLiftPercentage = (float)GeoLib.GameMath.GetPercentage(leftLift, rightLift) / 100f;
		if (totalLift > Weight * delta)
			totalLift = Weight * delta;

		float scale = (float)GeoLib.GameMath.GetPercentage(totalLift, originalTotalLift) / 100f;
		leftLift = totalLift * leftLiftPercentage;
		rightLift = totalLift - leftLift;

		state.ApplyImpulse(left, new Vector3(0, leftLift, 0));
		state.ApplyImpulse(right, new Vector3(0, rightLift, 0));

		List<Tuple<float, float, float>> thrusts = new List<Tuple<float, float, float>>();
		foreach (var e in planeData.Configuration.Engines)
			thrusts.Add(new Tuple<float, float, float>(e.CurrentSpeed, e.GetThrust(delta, windPhysics.GetDensity(planeData.Flight.Altitude)), e.Fuel.Remaining));
		cockpit.SetEngines(thrusts, 30);
		state.ApplyImpulse(left, GlobalTransform.basis.z * thrusts[0].Item2 * delta * -1);
		state.ApplyImpulse(right, GlobalTransform.basis.z * thrusts[1].Item2 * delta * -1);

		float fallForwardSpeed = planePhysics.GetDiveForwardSpeed() * 0.005f;
		float realYSpeed = state.LinearVelocity.y;
		float realXSpeed = state.LinearVelocity.x + (float)(fallForwardSpeed * Math.Sin(planeData.Flight.Pitch));
		float realZSpeed = state.LinearVelocity.z + (float)(-fallForwardSpeed * Math.Cos(planeData.Flight.Pitch));
		state.LinearVelocity = new Vector3(realXSpeed, realYSpeed, realZSpeed);
		
		state.ApplyImpulse(left, GlobalTransform.basis.z * planePhysics.GetLeftDrag(windPhysics) * delta);
		state.ApplyImpulse(right, GlobalTransform.basis.z * planePhysics.GetRightDrag(windPhysics) * delta);
		state.ApplyImpulse(tail, GlobalTransform.basis.z * planePhysics.GetTailDrag(windPhysics) * delta);

		state.ApplyImpulse(tail, GlobalTransform.basis.x * planePhysics.GetTailSide(windPhysics) * delta);
		state.ApplyImpulse(tail, GlobalTransform.basis.x * -planePhysics.GetCentralSide(windPhysics) * 0.1f * delta * scale);

		float elevatorLift = planePhysics.GetPartLift(planeData.Configuration.Elevator, windPhysics) * delta;// * scale;
		state.ApplyImpulse(tail, GlobalTransform.basis.y * elevatorLift);

		cockpit.Update(planePhysics, delta, windPhysics, Weight, totalLift, leftLift, rightLift);

		handleInput(state, delta);
		planeData.Update(delta);
	}

	void handleAngularDamp(PhysicsDirectBodyState state)
	{
		if (state.AngularVelocity.Length() > 1)
			state.AngularVelocity *= new Vector3(0.999f, 0.999f, 0.99f);
	}

	void updateMass()
	{
		Mass = machineData.Mass;
		foreach (var e in planeData.Configuration.Engines)
			Mass += e.Fuel.GetMass();
	}

	FlightData getFlightData(PhysicsDirectBodyState state)
	{
		GeoLib.Vector3F velocity = new GeoLib.Vector3F(state.LinearVelocity.x, state.LinearVelocity.y, state.LinearVelocity.z);
		GeoLib.Vector3F rotation = new GeoLib.Vector3F(Rotation.x, Rotation.y, Rotation.z + (float)GeoLib.GameMath.DegToRad(180));
		GeoLib.Vector3F translation = new GeoLib.Vector3F(Translation.x, Translation.y, Translation.z);
		return new FlightData(translation, rotation, velocity);
	}

	void handleInput(PhysicsDirectBodyState state, float delta)
	{
		handleBrakesInput(state);
		handleEmergencyInput(state);
		handleFlapsInput();
		handleGearInput();
		handleHeightmapInput();
		handlePitchInput();
		handleRollInput();
		handleRudderInput();
		handleSlatsInput();
		handleThrustInput(delta);
	}


	void handleBrakesInput(PhysicsDirectBodyState state)
	{
		if (Input.IsActionPressed("brakes") && gearTouchingGround())
		{
			planeData.Configuration.Brakes.Enabled = true;
			state.LinearVelocity *= new Vector3(1, 1, planeData.Configuration.Brakes.Decceleration);
		}
		else
			planeData.Configuration.Brakes.Enabled = false;
	}

	void handleEmergencyInput(PhysicsDirectBodyState state)
	{
		if (Input.IsActionJustPressed("emergency"))
		{
			state.LinearVelocity = new Vector3(0, 0, -50);
			state.Transform = new Transform(new Vector3(-1, 0, 0), new Vector3(0, -1, 0), new Vector3(0, 0, 1), new Vector3(200,200,1850));
			state.AngularVelocity = new Vector3(0, 0, 0);
			return;
		}
	}

	void handleFlapsInput()
	{
		if (Input.IsActionJustPressed("flaps"))
		{
			if (Input.IsActionPressed("left"))
				planeData.Configuration.LeftFlap.SwitchConfiguration();
			if (Input.IsActionPressed("right"))
				planeData.Configuration.RightFlap.SwitchConfiguration();
			else
			{
				planeData.Configuration.LeftFlap.SwitchConfiguration();
				planeData.Configuration.RightFlap.SwitchConfiguration();
			}
		}
	}

	void handleGearInput()
	{
		if (Input.IsActionJustPressed("gear"))
			planeData.Configuration.Gear.Enabled = !planeData.Configuration.Gear.Enabled;
	}

	void handleHeightmapInput()
	{
		if (Input.IsActionJustPressed("heightmap"))
			cockpit.SwitchHeightmap();
	}

	void handlePitchInput()
	{
		if (Input.IsActionPressed("pitchUp"))
			planeData.Configuration.Elevator.Move(false);
		else if (Input.IsActionPressed("pitchDown"))
			planeData.Configuration.Elevator.Move(true);
		else
			planeData.Configuration.Elevator.Level();
	}

	void handleRollInput()
	{
		if (Input.IsActionPressed("rollLeft"))
		{
			planeData.Configuration.LeftAileron.Move(false);
			planeData.Configuration.RightAileron.Move(true);
		}
		else if (Input.IsActionPressed("rollRight"))
		{
			planeData.Configuration.LeftAileron.Move(true);
			planeData.Configuration.RightAileron.Move(false);
		}
		else
		{
			planeData.Configuration.LeftAileron.Level();
			planeData.Configuration.RightAileron.Level();
		}
	}

	void handleRudderInput()
	{
		if (Input.IsActionPressed("rudderLeft"))
			planeData.Configuration.Rudder.Move(false);
		else if (Input.IsActionPressed("rudderRight"))
			planeData.Configuration.Rudder.Move(true);
		else
			planeData.Configuration.Rudder.Level();
	}

	void handleSlatsInput()
	{
		if (Input.IsActionJustPressed("slats"))
		{
			if (Input.IsActionPressed("left"))
				planeData.Configuration.LeftSlat.Enabled = !planeData.Configuration.LeftSlat.Enabled;
			if (Input.IsActionPressed("right"))
				planeData.Configuration.RightSlat.Enabled = !planeData.Configuration.RightSlat.Enabled;
			else
			{
				planeData.Configuration.LeftSlat.Enabled = !planeData.Configuration.LeftSlat.Enabled;
				planeData.Configuration.RightSlat.Enabled = !planeData.Configuration.RightSlat.Enabled;
			}
		}
	}

	void handleThrustInput(float delta)
	{
		if (Input.IsActionPressed("thrustUp"))
			udpateThrust(true, delta);
		else if (Input.IsActionPressed("thrustDown"))
			udpateThrust(false, delta);
	}

	void udpateThrust(bool isThrustUp, float delta)
	{
		if (Input.IsActionPressed("1") && planeData.Configuration.Engines.Count >= 1)
			planeData.Configuration.Engines[0].UpdateThrust(isThrustUp, delta);
		else if (Input.IsActionPressed("2") && planeData.Configuration.Engines.Count >= 2)
			planeData.Configuration.Engines[1].UpdateThrust(isThrustUp, delta);
		else if (Input.IsActionPressed("3") && planeData.Configuration.Engines.Count >= 3)
			planeData.Configuration.Engines[2].UpdateThrust(isThrustUp, delta);
		else if (Input.IsActionPressed("4") && planeData.Configuration.Engines.Count >= 4)
			planeData.Configuration.Engines[3].UpdateThrust(isThrustUp, delta);
		else
			foreach (var e in planeData.Configuration.Engines)
				e.UpdateThrust(isThrustUp, delta);
	}

	bool gearTouchingGround() => planeData.Configuration.Gear.Enabled && gear.IsColliding();
}

