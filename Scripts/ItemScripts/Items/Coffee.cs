using Godot;
using System;

public partial class Coffee : Node, IItem {
    public void Use(GameDriver driver, Player holder) {
        holder.NerfGun.DoubleGunDamage(); // doubles the gun damage
    }
}
