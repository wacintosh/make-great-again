﻿using UnityEngine;

public enum projectileType {
	rock, arrow, fireball
};
public class Projectile : MonoBehaviour {
	[SerializeField] private int attackStrength;
	[SerializeField] private projectileType projectileType;

	public int AttackStrength {
		get {
			return attackStrength;
		}
	}

	public projectileType ProjectileType {
		get {
			return projectileType;
		}
	}
}
