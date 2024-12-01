using Godot;
using System;

public partial class Broccoli : Node, IItem {
    public void Use(GameDriver driver, Player holder) {
		holder.HealPlayer(1); // heal player by 1
    }

}
