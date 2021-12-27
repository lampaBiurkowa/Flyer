using Godot;
using System;

public class Plane2 : PlaneBase
{
	public override void _Ready()
	{
		Alias = "Plane2";
		tryLoadSkinFromFile(1);
	}

	public override bool TryLoadSkin(int number)
	{
		return tryLoadSkinFromFile(number);
	}
}
