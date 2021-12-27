using Godot;
using System;

public class MainMenu : Panel
{
	private Button playButton;
	public override void _Ready()
	{
		loadResources();
		connectSignals();
	}

	void loadResources()
	{
		playButton = (Button)GetNode("Button");
	}

	void disconnectSignals()
	{
		playButton.Disconnect("pressed", this, nameof(onPlayButtonPressesd));
	}
	void connectSignals()
	{
		playButton.Connect("pressed", this, nameof(onPlayButtonPressesd));
	}

	void onPlayButtonPressesd()
	{
		disconnectSignals();
		GetTree().ChangeScene("Scenes/UI/ChooseAircraft.tscn");
	}
}
