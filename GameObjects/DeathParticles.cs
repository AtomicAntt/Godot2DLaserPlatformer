using Godot;
using System;

public class DeathParticles : Particles2D
{
    public override void _Ready()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("explode");
        GetNode<AudioStreamPlayer2D>("Explode" + (GD.Randi() % 6 + 1)).Play();
    }
}
