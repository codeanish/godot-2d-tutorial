using Godot;
using System;

public partial class player : Area2D
{
	[Signal]
	public delegate void HitEventHandler();

	[Export]
	public int Speed { get; set; } = 400;
	
	public Vector2 ScreenSize;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero;
		
		if ( Input.IsActionPressed("move_right") )
		{
			velocity.X += 1;
		}

		if ( Input.IsActionPressed("move_left") )
		{
			velocity.X -= 1;
		}

		if ( Input.IsActionPressed("move_down") )
		{
			velocity.Y += 1;
		}

		if ( Input.IsActionPressed("move_up") )
		{
			velocity.Y -= 1;
		}

		var animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if ( velocity.Length() > 0 )
		{
			velocity = velocity.Normalized() * Speed;
			animatedSprite.Play();
		}
		else
		{
			animatedSprite.Stop();
		}

		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);

		if ( velocity.X != 0 )
		{
			animatedSprite.Animation = "walk";
			animatedSprite.FlipV = false;
			animatedSprite.FlipH = velocity.X < 0;
		}
		else if ( velocity.Y != 0 )
		{
			animatedSprite.Animation = "up";
			animatedSprite.FlipV = velocity.Y > 0;
		}

		if (velocity.X < 0)
		{
			animatedSprite.FlipH = true;
		}
		else
		{
			animatedSprite.FlipH = false;
		}
	}

	private void _on_body_entered(Node2D body)
	{
		Hide();
		EmitSignal(SignalName.Hit);
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}

	public void Start(Vector2 pos)
	{
		Position = pos;
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}
}



