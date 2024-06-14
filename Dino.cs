using Godot;
using System;

public partial class Dino : Area2D
{
	[Signal]
	public delegate void GameOverEventHandler();
	
	[Export]
	public int MaxJumpVelocity = 100;
	
	[Export]
	public int DinoGravity = 10;
	
	private AnimatedSprite2D _dinoAnimator;

	private Vector2 _initPosition;
	
	private bool _bGameOver = false;
	
	private int _velocity = 0;
	private int _gravity = 0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_dinoAnimator = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AreaEntered += OnAreaEntered;
		_initPosition = Position;
		_gravity = DinoGravity;
	}

	public void OnRestart()
	{
		_bGameOver = false;
		Position = _initPosition;
		_velocity = 0;
		_gravity = DinoGravity;
		_dinoAnimator.Play("dino_run");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_bGameOver)
		{
			_dinoAnimator.Play("dino_oops");
			return;
		}
		
		if (Input.IsActionPressed("dodge"))
		{
			_dinoAnimator.Play("dino_dodge");
			_gravity = 2 * DinoGravity;
		}
		else
		{
			_dinoAnimator.Play("dino_run");
			_gravity = DinoGravity;
		}
		if (Input.IsActionJustPressed("jump"))
		{
			//_dinoAnimator.Play("dino_jump");
			if (_velocity == 0)
			{
				_velocity += MaxJumpVelocity;
			}
		}
		Position -= new Vector2(0, _velocity * (float)delta);
		if(Position.Y < _initPosition.Y)
		{
			_velocity -= _gravity;
		}
		else
		{
			_velocity = 0;
			Position = _initPosition;
		}
	}
	
	private void OnAreaEntered(Area2D area)
	{
		if (area is Cactus)
		{
			GD.Print("Collision Detected");
			SetGameOver();
		}
	}

	private void SetGameOver()
	{
		_bGameOver = true;
		EmitSignal(nameof(GameOver));
	}
}
