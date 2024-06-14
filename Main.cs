using Godot;
using System;

public partial class Main : Node
{
	// Called when the node enters the scene tree for the first time.
	private Dino _dinosaur;
	private HUD _hud;
	private TerrainSpawner _terrainSpawner;
	public override void _Ready()
	{
		_dinosaur = GetNode<Dino>("Dino");
		_dinosaur.GameOver += OnGameOver;
		_terrainSpawner = GetNode<TerrainSpawner>("TerrainSpawner");
		_hud = GetNode<HUD>("HUD");
		_hud.GameRestart += OnGameRestart;
	}
	
	public void OnGameOver()
	{
		_terrainSpawner.OnGameOver();
		_hud.OnGameOver();
	}

	public void OnGameRestart()
	{
		_dinosaur.OnRestart();
		_terrainSpawner.OnRestart();
	}
}
