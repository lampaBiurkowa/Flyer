using Godot;
using System;

public class ChooseAircraft : ViewportContainer
{
	Button cancelButton;
	Button selectButton;
	TextureButton leftButton;
	TextureButton rightButton;
	TextureButton downButton;
	TextureButton upButton;
	Viewport viewport;
	MeshInstance plane;
	Spatial planeSpace;
	int currentPlane = 1;
	int currentSkin = 1;
	public override void _Ready()
	{
		loadResources();
		connectSignals();
		updatePlane();
	}

	void loadResources()
	{
		cancelButton = (Button)GetNode("CancelButton");
		selectButton = (Button)GetNode("SelectButton");
		leftButton = (TextureButton)GetNode("LeftButton");
		rightButton = (TextureButton)GetNode("RightButton");
		downButton = (TextureButton)GetNode("DownButton");
		upButton = (TextureButton)GetNode("UpButton");
		viewport = (Viewport)GetNode("Viewport");
		planeSpace = (Spatial)GetNode("Viewport/PlaneSpace");
	}

	void connectSignals()
	{
		cancelButton.Connect("pressed", this, nameof(onCancelButtonPressesd));
		selectButton.Connect("pressed", this, nameof(onSelectButtonPressesd));
		leftButton.Connect("pressed", this, nameof(onLeftButtonPressesd));
		rightButton.Connect("pressed", this, nameof(onRightButtonPressesd));
		downButton.Connect("pressed", this, nameof(onDownButtonPressesd));
		upButton.Connect("pressed", this, nameof(onUpButtonPressesd));
	}

	void onCancelButtonPressesd()
	{
		disconnectSignals();
		GetTree().ChangeScene("Scenes/UI/MainMenu.tscn");
	}

	void disconnectSignals()
	{
		cancelButton.Disconnect("pressed", this, nameof(onCancelButtonPressesd));
		selectButton.Disconnect("pressed", this, nameof(onSelectButtonPressesd));
		leftButton.Disconnect("pressed", this, nameof(onLeftButtonPressesd));
		rightButton.Disconnect("pressed", this, nameof(onRightButtonPressesd));
		downButton.Disconnect("pressed", this, nameof(onDownButtonPressesd));
		upButton.Disconnect("pressed", this, nameof(onUpButtonPressesd));
	}

	void onSelectButtonPressesd()
	{
		disconnectSignals();
		GetTree().ChangeScene("Scenes/Terrains/Terrain1.tscn");
	}

	void onLeftButtonPressesd()
	{
		if (currentPlane <= 1)
			return;

		currentPlane--;
		updatePlane();
	}

	void updatePlane()
	{
		if (planeSpace.GetChildCount() > 0)
			planeSpace.GetChild(0).QueueFree();
		PackedScene scene = (PackedScene)ResourceLoader.Load($"Scenes/Planes/Plane{currentPlane}.tscn");
		var mesh = scene.Instance();
		plane = (PlaneBase)(mesh.GetNode("Plane"));
		plane.Scale = new Vector3(0.1f, 0.1f, 0.1f);
		planeSpace.AddChild(plane.GetNode("../"));
		currentSkin = 1;
	}

	void onRightButtonPressesd()
	{
		if (currentPlane >= getPlanesCount())
			return;

		currentPlane++;
		updatePlane();
	}

	int getPlanesCount()
	{
		int i = 0;
		while (true)
		{
			if (System.IO.Directory.Exists($"Resources/Planes/Plane{i + 1}"))
				i++;
			else
				break;
		}

		return i;
	}

	void onUpButtonPressesd()
	{
		if (((PlaneBase)plane).TryLoadSkin(currentSkin + 1))
			currentSkin++;
	}

	void onDownButtonPressesd()
	{
		if (((PlaneBase)plane).TryLoadSkin(currentSkin - 1))
			currentSkin--;
	}


	public override void _Process(float delta)
	{
		planeSpace.RotateY(0.01f);
	}
}
