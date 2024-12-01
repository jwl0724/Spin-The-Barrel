using Godot;
using System;

public partial class IceCream : Node, IItem {
    public void Use(GameDriver driver, Player holder) {
        holder.EndTurn(); // skips turn
    }
}
