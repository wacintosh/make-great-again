using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	[SerializeField] private Transform exitPoint;
	[SerializeField] private Transform[] wayPoints;
	[SerializeField] private float navigationUpdate;
	[SerializeField] private int healthPoints;
	[SerializeField] private int rewardAmount;

	private bool isDead = false;
	private int target = 0;
	private Transform enemy;
	private float navigationTime = 0;
	private Collider2D enemyCollider;
	private Animator animator;

	public bool IsDead {
		get {
			return isDead;
		}
	}

	// Use this for initialization
	void Start () {
		enemy = GetComponent<Transform>();
		enemyCollider = GetComponent<Collider2D>();
		animator = GetComponent<Animator>();
		GameManager.Instance.RegisterEnemy(this);
	}
	
	// Update is called once per frame
	void Update () {
		if (wayPoints != null && !isDead) {
			navigationTime += Time.deltaTime;
			if (navigationTime > navigationUpdate) {
				if (target < wayPoints.Length) {
					enemy.position = Vector2.MoveTowards(enemy.position, wayPoints[target].position, navigationTime);
				} else {
					enemy.position = Vector2.MoveTowards(enemy.position, exitPoint.position, navigationTime);
				}
				navigationTime = 0;
			}
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Checkpoint") {
			target += 1;
		} else if (other.tag == "Finish") {
			GameManager.Instance.RoundEscaped += 1;
			GameManager.Instance.TotalEscaped += 1;
			GameManager.Instance.UnregisterEnemy(this);
			GameManager.Instance.isWaveOver();
		} else if (other.tag == "Projectile") {
			Projectile newP = other.gameObject.GetComponent<Projectile>();
			enemyHit(newP.AttackStrength);
			Destroy(other.gameObject);
		}
	}

	public void enemyHit(int hitPoints) {
		if (healthPoints - hitPoints > 0) {
			healthPoints -= hitPoints;
			animator.Play("Hurt");
		} else {
			animator.SetTrigger("didDie");
			die();
		}
	}

	public void die() {
		isDead = true;
		enemyCollider.enabled = false;
		GameManager.Instance.TotalKilled += 1;
		GameManager.Instance.addMoney(rewardAmount);
		GameManager.Instance.isWaveOver();
	}
}
