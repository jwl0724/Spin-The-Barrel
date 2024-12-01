using Godot;
using System;

public partial class Shield : Node, IItem {
    public void Use(GameDriver driver, Player holder) {
        holder.IsProtected = true; // blocks a dart for the round
    }

}
