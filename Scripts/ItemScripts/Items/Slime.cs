using Godot;
using System;

public partial class Slime : Node, IItem {
    public void Use(GameDriver driver, Player holder) {
        holder.CanShootOther = true;
    }
}
