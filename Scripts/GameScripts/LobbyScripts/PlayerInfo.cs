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

	public bool IsRemote { get; set; } = true;
	public string Name { get; private set; } // randomly generate name in MVP
	public int ChosenModel { get; set; }
	public long NetworkID { get; private set; }

	// probably socket id here when networking is done

	public PlayerInfo(long id, string name = "") {
		NetworkID = id;
		Name = name == "" ? generateRandomName() : name;
	}

	private string generateRandomName() {
		Random random = new();
		string name = firstName[random.Next(0, firstName.Length)] + 
			lastName[random.Next(0, lastName.Length)] + 
			random.Next(0, 9999).ToString();
		return name;
	}
}