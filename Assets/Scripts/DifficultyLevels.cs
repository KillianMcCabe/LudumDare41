using System.Collections;
using System.Collections.Generic;

public class DifficultyLevels {

	public struct Wave {

		public int noOfLevel1Enemies;
		public int noOfLevel2Enemies;
		public int noOfLevel3Enemies;

		public Wave(int _noOfLevel1Enemies, int _noOfLevel2Enemies, int _noOfLevel3Enemies) {
			noOfLevel1Enemies = _noOfLevel1Enemies;
			noOfLevel2Enemies = _noOfLevel2Enemies;
			noOfLevel3Enemies = _noOfLevel3Enemies;
		}
	}

	public static Wave[] waves = {
		new Wave(8, 0, 0),
		new Wave(12, 2, 0),
		new Wave(16, 4, 0),
		new Wave(20, 6, 0),
		new Wave(20, 6, 1)
	};

}
