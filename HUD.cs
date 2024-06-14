using Godot;
using System;

public partial class HUD : Node
{
	[Signal] public delegate void GameRestartEventHandler();

	private TextureButton _restartBtn;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_restartBtn = GetNode<TextureButton>("TextureButton");
		_restartBtn.Pressed += OnRestartBtnPressed;
		_restartBtn.Hide();
	}
	
	private void OnRestartBtnPressed()
	{
		_restartBtn.Hide();
		EmitSignal(nameof(GameRestart));
		GD.Print("Restart button pressed!");
	}

	public void OnGameOver()
	{
		_restartBtn.Show();
	}
}
