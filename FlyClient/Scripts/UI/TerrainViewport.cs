using Godot;
using ClientCore.Cockpit;

public class TerrainViewport : Viewport
{

	public override void _Ready()
	{
		loadComponents();
	}

	/*public void CaptureTerrainForMinimap()
	{
		viewportCamera.Current = true;
		captureMinimapBase();
		minimapData.CutCapturedImage();
		planeCamera.Current = true;
	}*/

	void loadComponents()
	{
	}

}
