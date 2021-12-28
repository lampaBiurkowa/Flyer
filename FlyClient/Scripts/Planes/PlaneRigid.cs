using Godot;
using System;
using ClientCore.Physics;

public class PlaneRigid : RigidBody
{
	PlaneBase plane;
	PlanePhysics planePhysics;
	WindPhysics windPhysics;
	Cockpit cockpit;

	public override void _Ready()
	{
		loadComponents();
		planePhysics = new PlanePhysics(new GeoLib.Vector2(25, 5));
		windPhysics = new WindPhysics();
		windPhysics.Direction = new GeoLib.Vector3(1, 0, 0);
		windPhysics.Speed = new GeoLib.Vector3(5, 0, 0);
	}

	void loadComponents()
	{
		plane = (PlaneBase)GetChild(0);
		cockpit = (Cockpit)GetNode("../../../Cockpit");
	}
	
	public override void _IntegrateForces(PhysicsDirectBodyState state)
	{
		float delta = state.Step;
		handleInput(state, delta);

		float weight = state.TotalGravity.y;
		float lift = getLift(state, delta, weight);

		state.ApplyCentralImpulse(new Vector3(0, lift, 0));
		
		cockpit.SetSpeed((float)Math.Sqrt(Math.Pow(state.LinearVelocity.x, 2) + Math.Pow(state.LinearVelocity.z, 2)));
		cockpit.SetLift(lift);
		cockpit.SetAltitude((int)Translation.y * 2);
		cockpit.SetWeight(weight * delta);
		GD.Print($"spid {state.LinearVelocity} rotation {RotationDegrees} pos {Translation} lift {lift}");

		state.IntegrateForces();
	}

	void handleInput(PhysicsDirectBodyState state, float delta)
	{
		const float IMPULSE = 1;
		if (Input.IsActionJustPressed("ui_up"))
			state.ApplyCentralImpulse(new Vector3(-IMPULSE, 0, 0) * delta);
	}

	float getLift(PhysicsDirectBodyState state, float delta, float weight)
	{
		GeoLib.Vector3 velocity = new GeoLib.Vector3(state.LinearVelocity.x, state.LinearVelocity.y, state.LinearVelocity.z);
		GeoLib.Vector3 rotation = new GeoLib.Vector3(RotationDegrees.x, RotationDegrees.y, RotationDegrees.z);
		GeoLib.Vector3 translation = new GeoLib.Vector3(Translation.x, Translation.y, Translation.z);
		float liftValue = planePhysics.GetLift(windPhysics, rotation, velocity, translation);

		const float OVERLIFT = 1.4f;
		if (state.LinearVelocity.y >= 0 && liftValue > -weight * delta)
			liftValue = -weight * delta;
		else if (liftValue > -weight * delta * OVERLIFT)
			liftValue = -weight * delta * OVERLIFT;

		return liftValue;
	}
}

