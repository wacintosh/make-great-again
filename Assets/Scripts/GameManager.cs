using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum gameStatus {
	next, play, gameover, win
}

public class GameManager : Singleton<GameManager> {
	
	[SerializeField] private int totalWaves = 10;
	[SerializeField] private Text totalMoneyLabel;
	[SerializeField] private int totalEscaped;
	[SerializeField] private Text totalEscapedLabel;
	[SerializeField] private GameObject spawnPoint;
	[SerializeField] private GameObject[] enemies;
	[SerializeField] private int totalEnemies = 3;
	[SerializeField] private int enemiesPerSpawn;
	[SerializeField] private Button playBtn;
	[SerializeField] private Text playBtnLabel;

	private gameStatus currentState = gameStatus.play;
	private int whichEnemiesToSpawn = 0;
	private int waveNumber = 0;
	private int totalMoney = 10;

	private int roundEscaped = 0;
	private int totalKilled = 0;
	public List<Enemy> EnemyList = new List<Enemy>();

	const float spawnDelay = 0.5f;

	public int TotalEscaped {
		get {
			return totalEscaped;
		}
		set {
			totalEscaped = value;
			totalEscapedLabel.text = "Escaped " + totalEscaped + "/10";
		}
	}

	public int RoundEscaped {
		get {
			return roundEscaped;
		}
		set {
			roundEscaped = value;
		}
	}

	public int TotalKilled {
		get {
			return totalKilled;
		}
		set {
			totalKilled = value;
		}
	}

	public int TotalMoney {
		get {
			return totalMoney;
		}
		set {
			totalMoney = value;
			totalMoneyLabel.text = totalMoney.ToString();
		}
	}

	// Use this for initialization
	void Start () {
		playBtn.gameObject.SetActive(false);
		showMenu();
	}

	IEnumerator spawn() {
		if (enemiesPerSpawn > 0 && EnemyList.Count < totalEnemies) {
			for (int i = 0; i < enemiesPerSpawn; i++) {
				if (EnemyList.Count < totalEnemies) {
					GameObject newEnemy = Instantiate(enemies[0]) as GameObject;
					newEnemy.transform.position = spawnPoint.transform.position;
				}
			}
			yield return new WaitForSeconds(spawnDelay);
			StartCoroutine(spawn());
		}
	}

	public void RegisterEnemy(Enemy enemy) {
		EnemyList.Add(enemy);
	}

	public void UnregisterEnemy(Enemy enemy) {
		EnemyList.Remove(enemy);
		Destroy(enemy.gameObject);
	}

	public void DestroyAllEnemies() {
		foreach(Enemy enemy in EnemyList) {
			Destroy(enemy.gameObject);
		}

		EnemyList.Clear();
	}

	public void addMoney(int money) {
		TotalMoney += money;
	}

	public void subtractMoney(int money) {
		TotalMoney -= money;
	}

	public void showMenu() {
		switch (currentState) {
			case gameStatus.gameover:
				playBtnLabel.text = "Play again";
				break;
			case gameStatus.next:
				playBtnLabel.text = "Next wave";
				break;
			case gameStatus.play:
				playBtnLabel.text = "Play";
				break;
			case gameStatus.win:
				playBtnLabel.text = "Play again";
				break;
		}
		playBtn.gameObject.SetActive(true);
	}

	public void isWaveOver() {
		totalEscapedLabel.text = "Escaped " + TotalEscaped + "/10";
		if ((RoundEscaped + TotalKilled) == totalEnemies) {
			setCurrentGameState();
			showMenu();
		}
	}

	public void setCurrentGameState() {
		if (TotalEscaped >= 10) {
			currentState = gameStatus.gameover;
		} else if (waveNumber == 0 && (TotalKilled + RoundEscaped) == 0) {
			currentState = gameStatus.play;
		} else if (waveNumber >= totalWaves) {
			currentState = gameStatus.win;
		} else {
			currentState = gameStatus.next;
		}
	}

	public void playBtnPressed() {
		switch(currentState) {
			case gameStatus.next:
				waveNumber++;
				totalEnemies += waveNumber;
				break;
			default:
				totalEnemies = 3;
				TotalEscaped = 0;
				TotalMoney = 10;
				break;
		}

		DestroyAllEnemies();
		TotalKilled = 0;
		RoundEscaped = 0;
		StartCoroutine(spawn());
		playBtn.gameObject.SetActive(false);
	}
}
