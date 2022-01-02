using Godot;
using System;

public class Radar : AnimatedSprite
{
	const int STEP = 2;
	public override void _Ready()
	{
		Frames = loadSpriteFrames();
	}

	SpriteFrames loadSpriteFrames()
	{
		SpriteFrames frames = new SpriteFrames();
		for (int i = 0; i < 360; i += STEP)
			loadSingleFrame(frames, i);

		return frames;
	}

	void loadSingleFrame(SpriteFrames f, int index)
	{
		ImageTexture texture = new ImageTexture();
		Image image = new Image();	
		image.Load($"Resources/Planes/Cockpit/Radar/radar{index}.png");
		texture.CreateFromImage(image);
		f.AddFrame("default", texture);
	}
}
