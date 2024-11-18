using Godot;
using System;

public interface IInteractableEntity {
	string GetEntityName();
	Node3D GetModel();
	void Interact();
}
