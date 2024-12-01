using Godot;
using System;

public partial class Dice : Node, IItem {
	public void Use(GameDriver driver, Player holder) {
        holder.NerfGun.Reroll();
    }
}
