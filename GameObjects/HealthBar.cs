using Godot;
using System;

public class HealthBar : CenterContainer
{
    private TextureProgress _damageBar;
    private TextureProgress _healthBar;
    private Label _healthLabel;

    [Export]
    public int maxHealth = 100;
    [Export]
    public int health = 100;

    public override void _Ready()
    {
        _damageBar = GetNode<TextureProgress>("DamageBar");
        _healthBar = GetNode<TextureProgress>("HealthBar");
        _healthLabel = GetParent().GetNode<Label>("HealthLabel");
    }

    // Called by player
    public void TakeDamage(int damage)
    {
        health -= damage;
        _healthLabel.Text = health + "/" + maxHealth;

        _healthBar.Value = health;

        SceneTreeTween tween = GetTree().CreateTween();
        tween.TweenProperty(_damageBar, "value", health, 0.5f);
    }

    public void ResetHealth()
    {
        _healthBar.Value = maxHealth;
        health = maxHealth;
        _damageBar.Value = maxHealth;
        _healthLabel.Text = health + "/" + maxHealth;
    }
}
