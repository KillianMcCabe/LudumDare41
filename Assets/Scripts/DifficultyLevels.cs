using System.Collections;
using System.Collections.Generic;

public class DifficultyLevels {

	public struct Wave {

		public float duration;
		public int noOfLevel1Enemies;
		public int noOfLevel2Enemies;

		public Wave(float _duration, int _noOfLevel1Enemies, int _noOfLevel2Enemies) {
			duration = _duration;
			noOfLevel1Enemies = _noOfLevel1Enemies;
			noOfLevel2Enemies = _noOfLevel2Enemies;
		}
	}

	public static Wave[] waves = {
		new Wave(10, 5, 0),
		new Wave(12, 10, 0),
		new Wave(14, 16, 0),
		new Wave(16, 20, 0)
	};

}
