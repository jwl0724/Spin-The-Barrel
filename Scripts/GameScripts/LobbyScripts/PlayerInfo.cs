using System;

public class PlayerInfo {
	private string[] firstName = {
		"Goldenrod",
		"Teal",
		"Orchid",
		"Crimson",
		"ForestGreen",
		"Gold",
		"SkyBlue",
		"Tomato",
		"LimeGreen",
		"Maroon",
		"SeaGreen",
		"DeepPink",
		"RoyalBlue",
		"Salmon",
		"Turquoise",
		"Sienna",
		"Chartreuse",
		"DarkGray",
		"Lavender",
		"OliveDrab",
		"Firebrick",
		"PowderBlue",
		"Coral",
		"MediumPurple",
		"Dark Orange"
	};
	private string[] lastName = {
		"Elephant",
		"Penguin",
		"Kangaroo",
		"Dolphin",
		"Panda",
		"Tiger",
		"Giraffe",
		"Zebra",
		"Koala",
		"Cheetah",
		"Rhinoceros",
		"Hippopotamus",
		"Armadillo",
		"Eagle",
		"Wolf",
		"Otter",
		"Peacock",
		"Sloth",
		"Chimpanzee",
		"Flamingo",
		"Lemur",
		"Whale",
		"Iguana",
		"Ocelot",
		"Platypus"
	};

	// TODO: Add more stuff here later that needs to be tracked in lobby	
	public bool IsRemote { get; private set; } = false;
	public string Name { get; private set; } // randomly generate name in MVP
	public int ChosenModel { get; set; }

	// probably socket id here when networking is done

	public PlayerInfo() {
		Random random = new();
		Name = firstName[random.Next(0, firstName.Length)] + 
			lastName[random.Next(0, lastName.Length)] + 
			random.Next(0, 9999).ToString();
	}

	// call this constructor for remote players
	public PlayerInfo(string name) {
		IsRemote = true;
		Name = name;
	}
}