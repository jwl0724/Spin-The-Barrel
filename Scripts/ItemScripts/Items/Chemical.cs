using Godot;
using System;

public partial class Chemical : Node, IItem {
    public void Use(GameDriver driver, Player holder) {
        int randNum = (int) (GD.Randi() % 4);
		int healAmount;
		if (randNum == 0) healAmount = 1;
		if (randNum == 1) healAmount = Player.DEFAULT_PLAYER_HEATLH - holder.Health;
		else healAmount = -1;
		holder.HealPlayer(healAmount); // chance to heal player
    }
}
