using Godot;

public class Minimap : Sprite
{
	const float SIZE = 232;
	Sprite indicator;
	Vector2 terrainSize = new Vector2(1, 1);
	public override void _Ready()
	{
		loadComponents();
	}

	public void SetTerrainSize(Vector2 size)
	{
		terrainSize = size;
	}

	void loadComponents()
	{
		indicator = (Sprite)GetNode("Indicator");
	}

	public void Update(Vector2 position, float yaw)
	{
		float x = (position.y / terrainSize.y) * SIZE - SIZE / 2;
		float y = (position.x / terrainSize.x) * SIZE - SIZE / 2;
		indicator.Position = new Vector2(x, y);
		indicator.RotationDegrees = -yaw;
	}


	/*MinimapData minimapData = new MinimapData(1);
	const float SIZE = 200;
	
	public override void _Ready()
	{
		loadMinimapTexture();
		setScale();
	}

	void loadMinimapTexture()
	{
		ImageTexture texture = new ImageTexture();
		Image image = new Image();	
		image.Load(minimapData.GetCapturePath());
		texture.CreateFromImage(image);
		Texture = texture;
	}

	void setScale()
	{
		float scale = SIZE / MinimapData.CAPTURE_CUT_SIZE;
		Scale = new Vector2(scale, scale);
	}*/
}
