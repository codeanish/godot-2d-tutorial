using Godot;
using System;

public partial class Main : Node
{
	[Export]
	public PackedScene MobScene { get; set; }
	
	private int _score;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	public void NewGame()
	{
		_score = 0;

		var player = GetNode<player>("Player");
		var startPosition = GetNode<Marker2D>("StartPosition");
		player.Start(startPosition.Position);

		GetNode<Timer>("StartTimer").Start();
		
		var hud = GetNode<HUD>("HUD");
		hud.UpdateScore(_score);
		hud.ShowMessage("Get Ready!");
		GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
		var music = GetNode<AudioStreamPlayer>("Music");
		music.Play();
	}

	private void _on_start_timer_timeout()
	{
		GetNode<Timer>("MobTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
	}
	
	private void _on_player_hit()
	{
		GetNode<Timer>("MobTimer").Stop();
		GetNode<Timer>("ScoreTimer").Stop();
		GetNode<HUD>("HUD").ShowGameOver();
		GetNode<AudioStreamPlayer>("Music").Stop();
		GetNode<AudioStreamPlayer>("DeathSound").Play();
	}
	
	private void _on_score_timer_timeout()
	{
		_score++;
		GetNode<HUD>("HUD").UpdateScore(_score);
	}
	
	private void _on_mob_timer_timeout()
	{
		Mob mob = MobScene.Instantiate<Mob>();

		var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.ProgressRatio = GD.Randf();

		float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

		mob.Position = mobSpawnLocation.Position;

		direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		mob.Rotation = direction;

		var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
		mob.LinearVelocity = velocity.Rotated(direction);

		AddChild(mob);
	}
}
