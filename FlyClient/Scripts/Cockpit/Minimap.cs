using Godot;

public class Minimap : Sprite
{
	const float SIZE = 256;
	Sprite indicator;
	Sprite heightmapLayer;

	Vector2 terrainSize = new Vector2(1, 1);
	private bool heightmapEnabled;
	public bool HeightmapEnabled
	{
		get => heightmapEnabled;
		set
		{
			heightmapEnabled = value;
			heightmapLayer.Visible = value;
		}
	}
	
	
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
		heightmapLayer = (Sprite)GetNode("HeightmapLayer");
	}

	public void Update(Vector2 position, float yaw)
	{
		float x = (position.y / terrainSize.y) * SIZE - SIZE / 2;
		float y = (position.x / terrainSize.x) * SIZE - SIZE / 2;
		indicator.Position = new Vector2(x, y);
		indicator.RotationDegrees = -yaw;
	}
}
