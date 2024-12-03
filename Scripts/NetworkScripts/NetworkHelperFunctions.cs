using Godot;
using System;
using System.Collections.Generic;

public partial class NetworkHelperFunctions : Node {
	// converts Players info in lobby driver to something friendly and sendable on network
	public static Godot.Collections.Array<Godot.Collections.Array> ConvertPlayersToNetwork(List<PlayerInfo> infos) {
		Godot.Collections.Array<Godot.Collections.Array> arrays = new();
		// index 0: networkID, index 1: playerName, index 2: ChosenModel
		foreach(PlayerInfo info in infos) {
			Godot.Collections.Array data = new() {info.NetworkID, info.Name, info.ChosenModel};
			arrays.Add(data);
		}
		return arrays;
	}
	// converts network friendly info into List<PlayerInfo> used by lobby driver
	public static List<PlayerInfo> ConvertPlayersFromNetwork(Godot.Collections.Array<Godot.Collections.Array> array) {
		List<PlayerInfo> playerList = new();
		// index 0: networkID, index 1: playerName, index 2: ChosenModel
		foreach(var data in array) {
			PlayerInfo player = new((long) data[0], (string) data[1]);
			player.ChosenModel = (int) data[2];
			playerList.Add(player);
		}
		return playerList;
	}
}
