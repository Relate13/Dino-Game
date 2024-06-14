using Godot;
using System;

public partial class TerrainSpawner : Node2D
{
	[Export] public int Speed;
	[Export] public int CloudSpeed;
	[Export] public int TerrainFrames = 12;
	[Export] public int TerrainLength = 200;
	
	[Export] public PackedScene TerrainBlockScene;
	[Export] public PackedScene CloudScene;
	[Export] public PackedScene[] CactusScenes;

	[Export] public Marker2D CactusSpawnCenter;

	[Export] public Timer CactusTimer;
	[Export] public Timer CloudTimer;
	
	[Export] public Vector2 CactusWaitTimerInterval = new Vector2(1, 3);

	private bool  _bMoving = true;

	public override void _Ready()
	{
		CactusTimer.Timeout += OnCactusTimerTimeout;
		CloudTimer.Timeout += OnCloudTimerTimeout;
		CactusTimer.WaitTime = (float)GD.RandRange(CactusWaitTimerInterval.X, CactusWaitTimerInterval.Y);
		CactusTimer.Start();
		CloudTimer.Start();
	}
	
	private void OnCactusTimerTimeout()
	{
		var cactus = CactusScenes[GD.Randi() % CactusScenes.Length].Instantiate() as Cactus;
		cactus.Position = CactusSpawnCenter.Position - new Vector2(0, GD.Randi() % 5);
		AddChild(cactus);
		CactusTimer.WaitTime = (float)GD.RandRange(CactusWaitTimerInterval.X, CactusWaitTimerInterval.Y);
		CactusTimer.Start();
	}

	private void OnCloudTimerTimeout()
	{
		var cloud = CloudScene.Instantiate() as Cloud;
		cloud.Position = CactusSpawnCenter.Position - new Vector2(0, 200 + GD.Randi() % 50);
		AddChild(cloud);
	}
	
	public void SetMoving(bool bMoving)
	{
		_bMoving = bMoving;
		if (!_bMoving)
		{
			CactusTimer.Stop();
			CloudTimer.Stop();
		}
	}

	public void ResetTerrain()
	{
		foreach (Node child in GetChildren())
		{
			if (child is TerrainBlock block)
			{
				block.GetNode<AnimatedSprite2D>("AnimatedSprite2D").Frame = 0;
			}
			else
			{
				child.QueueFree();
			}
		}
	}

	public void OnGameOver()
	{
		SetMoving(false);
	}

	public void OnRestart()
	{
		ResetTerrain();
		SetMoving(true);
		CactusTimer.Start();
		CloudTimer.Start();
	}
	
	public override void _Process(double delta)
	{
		//get every child of the spawner
		var furthestBlockPosition = new Vector2(0, 0);
		var bNewBlock = false;

		if (_bMoving)
		{
			foreach (Node child in GetChildren())
			{
				//if the child is a TerrainBlock
				if (child is TerrainBlock block)
				{
					//move the block to the left
					block.Position -= new Vector2((float)delta * Speed, 0);
					furthestBlockPosition = new Vector2(Math.Max(furthestBlockPosition.X, block.Position.X),
						Math.Max(furthestBlockPosition.Y, block.Position.Y));

					//if the block is out of the screen
					if (block.Position.X <= 0)
					{
						//remove the block
						block.QueueFree();
						bNewBlock = true;
					}
				}
				// if child is Cactus
				else if (child is Cactus cactus)
				{
					cactus.Position -= new Vector2((float)delta * Speed, 0);
					if (cactus.Position.X <= 0)
					{
						//remove the cactus
						cactus.QueueFree();
					}
				}
				else if (child is Cloud cloud)
				{
					cloud.Position -= new Vector2((float)delta * CloudSpeed, 0);
					if (cloud.Position.X <= 0)
					{
						//remove the cloud
						cloud.QueueFree();
					}
				}
			}
		}
		if (bNewBlock)
		{
			//create a new block
			//GD.Print("Created New block, current furthest block position: " + furthestBlockPosition);
			var block = TerrainBlockScene.Instantiate() as TerrainBlock;
			AddChild(block);
			block.GetNode<AnimatedSprite2D>("AnimatedSprite2D").Frame = (int)(GD.Randi() % TerrainFrames);
			block.Position = new Vector2(furthestBlockPosition.X + TerrainLength, furthestBlockPosition.Y);
		}
	}
}
