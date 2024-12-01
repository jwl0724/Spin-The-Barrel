using Godot;
using System;

public partial class Finger : IItem {
	public void Use(GameDriver driver, Player holder) {
        holder.CanChooseNextTurn = true;
    }
}
