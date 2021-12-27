using Godot;
using System;

public abstract class PlaneBase : MeshInstance
{
    public string Alias {get; protected set;}
	public override void _Ready()
	{
	}

    protected int getSkinsCount()
    {
        int i = 0;
        while (true)
        {
            if (System.IO.File.Exists($"Resources/Planes/{Alias}/Skins/skin{i + 1}.jpg"))
                i++;
            else
                break;
        }

        return i;
    }

	protected bool tryLoadSkinFromFile(int number)
	{
        if (number < 1 || number > getSkinsCount())
            return false;
            
		ImageTexture texture = new ImageTexture();
		Image image = new Image();	
		image.Load($"Resources/Planes/{Alias}/Skins/skin{number}.jpg");
		texture.CreateFromImage(image);
		SpatialMaterial material = new SpatialMaterial();
		material.AlbedoTexture = texture;

		for (int i = 0; i < GetSurfaceMaterialCount(); i++)
			SetSurfaceMaterial(i, material);
        
        return true;
	}

    public abstract bool TryLoadSkin(int number);
}
