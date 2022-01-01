using Godot;
using System;

public class Lever : Sprite
{
	int height;
	int levelCount;
	Sprite leverHead;

	private int placement;
	public int Placement
	{
		get => placement;
		set
		{
			if (value < 0 || value >= levelCount)
				return;

			placement = value;
			updateLeverHead();
		}
	}
	
	public override void _Ready()
	{
		loadComponents();
		height = Texture.GetHeight();
	}

	void loadComponents()
	{
		leverHead = (Sprite)GetNode("LeverHead");
	}

	public void Initialize(string leverName, int levelCount, int defaultLevel)
	{
		this.levelCount = levelCount;
		Placement = defaultLevel;
		loadComponents();
		loadLeverHeadTexture(leverName);
	}

	void loadLeverHeadTexture(string leverName)
	{
		ImageTexture texture = new ImageTexture();
		Image image = new Image();	
		image.Load($"Resources/Planes/Cockpit/{leverName}Lever.png");
		texture.CreateFromImage(image);
		leverHead.Texture = texture;
	}

	void updateLeverHead()
	{
		leverHead.Position = new Vector2(0, (height / (float)(levelCount - 1)) * Placement);
	}
}
