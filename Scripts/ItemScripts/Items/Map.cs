using Godot;
using System;

public partial class Map : Node, IItem {
	public void Use(GameDriver driver, Player holder) {
        holder.NerfGun.RollToSafe();
    }
}
