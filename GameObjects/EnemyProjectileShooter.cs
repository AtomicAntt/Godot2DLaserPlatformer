using Godot;
using System;

public class EnemyProjectileShooter : Node2D
{
    private Timer _timer;
    private RayCast2D _raycast;
    private Player _player;
    private EnemyEntity _enemyEntity;

    private bool _seePlayer = false;

    [Export]
    public int waitTime = 2;

    [Export]
    public int attackRange = 1000;

    [Export]
    public int damage = 10;

    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
        _raycast = GetNode<RayCast2D>("RayCast2D");
        _player = GetTree().GetNodesInGroup("player")[0] as Player;
        _enemyEntity = GetParent() as EnemyEntity;

        _timer.WaitTime = waitTime;
    }

    public void Shoot()
    {
        if (_seePlayer)
        {
            PackedScene scene = GD.Load<PackedScene>("res://GameObjects/EnemyProjectile.tscn");
            EnemyProjectile enemyProjectile = scene.Instance<EnemyProjectile>();
            enemyProjectile.GlobalPosition = GlobalPosition;
            enemyProjectile.damage = damage;
            Level level = GetTree().GetNodesInGroup("level")[0] as Level;
            level.AddChild(enemyProjectile);
        }
    }

    public void _on_Timer_timeout()
    {
        Shoot();
    }

    public override void _PhysicsProcess(float delta)
    {
        if (!IsInstanceValid(_player))
        {
            return;
        }
        Vector2 directionToPlayer = GlobalPosition.DirectionTo(_player.GlobalPosition);
        _raycast.CastTo = directionToPlayer * attackRange;

        bool localSawPlayer = false;

        if (_raycast.IsColliding())
        {
            if (_raycast.GetCollider().IsClass("KinematicBody2D"))
            {
                KinematicBody2D collider = _raycast.GetCollider() as KinematicBody2D;
                if (collider.IsInGroup("player"))
                {
                    localSawPlayer = true;
                    _seePlayer = true;
                }
            }
        }

        if (localSawPlayer == false)
        {
            _seePlayer = false;
        }

        if (_seePlayer && (_enemyEntity.GlobalPosition.x > _player.GlobalPosition.x))
        {
            _enemyEntity.FlipRight(false);
        }
        else if (_seePlayer)
        {
            _enemyEntity.FlipRight(true);
        }

    }

    // Called by enemytorus
    public bool SeesPlayer()
    {
        return _seePlayer;
    }
}
