using Godot;
using System;

public partial class Compass : Node, IItem {
	public void Use(GameDriver driver, Player holder) {
        driver.Reverse = !driver.Reverse;
    }
}
