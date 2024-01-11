using Godot;
using System;

public class DeathParticles : Particles2D
{
    public override void _Ready()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("explode");
    }
}
