using Godot;
using System;
using ClientCore.Cockpit;

public class BasicT : Panel
{
	Sprite verticalSpeedArrow;
	Sprite altimeter100Arrow;
	Sprite altimeter1000Arrow;
	Sprite altimeter10000Arrow;
	Sprite heading;
	Sprite ahControl;
	Sprite turnCoordinatorArrow;
	Sprite airspeedArrow;
	Label altimeterAltitude;


	AirspeedData airspeedData = new AirspeedData();
	AltimeterData altimeterData = new AltimeterData();
	HeadingData headingData = new HeadingData();
	VerticalSpeedData verticalSpeedData = new VerticalSpeedData();
	TurnCoordinatorData turnCoordinatorData = new TurnCoordinatorData();
	ArtificialHorizonData ahData;

	public override void _Ready()
	{
		loadComponents();
		ahData = new ArtificialHorizonData("Resources/Planes/Cockpit/ahContent.png");
	}

	void loadComponents()
	{
		verticalSpeedArrow = (Sprite)GetNode("VerticalSpeed/VerticalSpeedArrow");
		altimeter100Arrow = (Sprite)GetNode("Altimeter/Altimeter100Arrow");
		altimeter1000Arrow = (Sprite)GetNode("Altimeter/Altimeter1000Arrow");
		altimeter10000Arrow = (Sprite)GetNode("Altimeter/Altimeter10000Arrow");
		altimeterAltitude = (Label)GetNode("Altimeter/Altitude");
		heading = (Sprite)GetNode("HeadingArrow/Heading");
		ahControl = (Sprite)GetNode("AH/AHControl");
		turnCoordinatorArrow = (Sprite)GetNode("TurnCoordinator/TurnCoordinatorArrow");
		airspeedArrow = (Sprite)GetNode("Airspeed/AirspeedArrow");
	}

	public void SetAirspeed(float airspeed)
	{
		airspeedArrow.RotationDegrees = airspeedData.GetAngleForSpeed(airspeed);
	}

	public void SetAltimeter(float altitude)
	{
		string text = $"{altitude}";
		while (text.Length < altimeterData.DigitsToDisplay)
			text = text.Insert(0, "0");

		altimeterAltitude.Text = text;
		float thirdDigit = (altitude / 100) % 10;
		altimeter100Arrow.RotationDegrees = altimeterData.GetAngleForNumber(thirdDigit);
		float fourthDigit = (altitude / 1000) % 10;
		altimeter1000Arrow.RotationDegrees = altimeterData.GetAngleForNumber(fourthDigit);
		float fifthDigit = (altitude / 10000) % 10;
		altimeter10000Arrow.RotationDegrees = altimeterData.GetAngleForNumber(fifthDigit);
	}

	public void SetHeading(float yaw)
	{
		heading.RotationDegrees = headingData.GetAngleForYaw(yaw);
	}

	public void SetAH(float pitch, float roll)
	{
		ahControl.Position = new Vector2(ahControl.Position.x, ahData.GetPitchPixelsForDegree() * pitch);
		ahControl.RotationDegrees = roll;
	}

	public void SetVerticalSpeed(float verticalSpeed)
	{
		verticalSpeedArrow.RotationDegrees = verticalSpeedData.GetAngleForSpeed(verticalSpeed);
	}

	public void SetTurnCoordinator(float yawRate)
	{
		turnCoordinatorArrow.RotationDegrees = turnCoordinatorData.GetAngleForYawRate(yawRate);
	}
}
