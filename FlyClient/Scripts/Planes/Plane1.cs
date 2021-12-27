using Godot;
using System;

public class Plane1 : PlaneBase
{
	public override void _Ready()
	{
		Alias = "Plane1";
		tryLoadSkinFromFile(1);
	}

	public override bool TryLoadSkin(int number)
	{
		return tryLoadSkinFromFile(number);
	}
}
