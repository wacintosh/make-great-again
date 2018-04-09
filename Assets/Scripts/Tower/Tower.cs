using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {

	[SerializeField] private float timeBetweenAttacks;
	[SerializeField] private float attackRadius;
	[SerializeField] private Projectile projectile;
	private Enemy targetEnemy = null;
	private float attackCounter;
	// Use this for initialization
	private bool isAttacking = false;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		attackCounter -= Time.deltaTime;
		Enemy nearestEnemy = GetNearestEnemyInRange();
		targetEnemy = nearestEnemy;

		if (targetEnemy != null) {
			if (Vector2.Distance(transform.localPosition, targetEnemy.transform.localPosition) > attackRadius || targetEnemy.IsDead) {
				targetEnemy = null;
			} else if (attackCounter <= 0) {
				isAttacking = true;
				attackCounter = timeBetweenAttacks;
			} else {
				isAttacking = false;
			}
		}
	}

	void FixedUpdate() {
		if (isAttacking) {
			Attack();
		}
	}

	public void Attack() {
		isAttacking = false;
		Projectile newProjectile = Instantiate(projectile) as Projectile;
		newProjectile.transform.localPosition = transform.localPosition;

		if (targetEnemy == null) {
			Destroy(newProjectile);
		} else {
			StartCoroutine(MoveProjectile(newProjectile));
		}
	}

	IEnumerator MoveProjectile(Projectile projectile) {
		while (getTargetDistance(targetEnemy) > 0.20f && projectile != null && targetEnemy != null) {
			var dir = targetEnemy.transform.localPosition - transform.localPosition;
			var angleDirection = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			projectile.transform.rotation = Quaternion.AngleAxis(angleDirection, Vector3.forward);
			projectile.transform.localPosition = Vector2.MoveTowards(projectile.transform.localPosition, targetEnemy.transform.localPosition, 5f * Time.deltaTime);
			yield return null;
		}

		if (projectile != null || targetEnemy == null) {
			Destroy(projectile);
		}
	}

	private float getTargetDistance(Enemy enemy) {
		if (enemy == null) {
			enemy = GetNearestEnemyInRange();
			if (enemy == null) {
				return 0f;
			}
		}

		return Mathf.Abs(Vector2.Distance(transform.localPosition, enemy.transform.localPosition));
	}

	private List<Enemy> GetEnemiesInRange() {
		List<Enemy> enemiesInRange = new List<Enemy>();
		foreach(Enemy enemy in GameManager.Instance.EnemyList) {
			if (Vector2.Distance(transform.localPosition, enemy.transform.localPosition) <= attackRadius && !enemy.IsDead) {
				enemiesInRange.Add(enemy);
			}
		}

		return enemiesInRange;
	}

	private Enemy GetNearestEnemyInRange() {
		Enemy nearestEnemy = null;
		float smallestDistance = float.PositiveInfinity;
		foreach(Enemy enemy in GetEnemiesInRange()) {
			if (Vector2.Distance(transform.localPosition, enemy.transform.localPosition) < smallestDistance) {
				nearestEnemy = enemy;
				smallestDistance = Vector2.Distance(transform.localPosition, enemy.transform.localPosition);
			}
		}

		return nearestEnemy;
	}
}
