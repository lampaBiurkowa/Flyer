using Godot;
using ClientCore.Sky;
using System.Collections.Generic;

public class Fog : Particles
{
	Dictionary<FogDensity, float> boxSizes = new Dictionary<FogDensity, float>();
	ParticlesMaterial material;

	public override void _Ready()
	{
		Initialize();
	}

	public void Initialize()
	{
		fillDictionary();
		material = (ParticlesMaterial)ProcessMaterial;
	}

	void fillDictionary()
	{
		if (boxSizes.Count > 0)
			return;
		
		boxSizes.Add(FogDensity.THICK, 10);
		boxSizes.Add(FogDensity.STANDARD, 25);
		boxSizes.Add(FogDensity.THIN, 100);
	}

	public void SetDensity(FogDensity density = FogDensity.STANDARD)
	{
		material.EmissionBoxExtents = new Vector3(boxSizes[density], boxSizes[density], boxSizes[density]);
	}
}
