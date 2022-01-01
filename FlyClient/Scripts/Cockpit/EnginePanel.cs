using Godot;
using System;
using ClientCore.Cockpit;

public class EnginePanel : Panel
{
	FuelData fuelData = new FuelData();
	RPMData rpmData = new RPMData();
	Sprite fuelArrow;
	Sprite rpmBigArrow;
	Sprite rpmSmallArrow;

	public override void _Ready()
	{
		loadComponents();
		initializeSmallArrow();
	}

	void loadComponents()
	{
		fuelArrow = (Sprite)GetNode("Fuel/FuelArrow");
		rpmBigArrow = (Sprite)GetNode("RPM/RPMArrow10");
		rpmSmallArrow = (Sprite)GetNode("RPM/RPMArrow1");
	}

	void initializeSmallArrow()
	{
		fuelArrow.Centered = false;
		fuelArrow.Offset = new Vector2(-fuelData.OriginX, -fuelData.OriginY);
		fuelArrow.Position = new Vector2(-fuelArrow.Texture.GetWidth() / 2 - fuelArrow.Offset.x, -fuelArrow.Texture.GetHeight() / 2 - fuelArrow.Offset.y);
	}

	public void SetFuel(float fuel)
	{
		fuelArrow.RotationDegrees = fuelData.GetAngleForFuel(fuel);
	}

	public void SetThrustPercentage(float thrustPercentage)
	{
		rpmBigArrow.RotationDegrees = rpmData.GetBigArrowAngleForNumber(thrustPercentage);
		rpmSmallArrow.RotationDegrees = rpmData.GetSmallArrowAngleForNumber(thrustPercentage % 10);
	}
}
