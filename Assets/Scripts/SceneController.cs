using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections;


public class SceneController : MonoBehaviour {

	public Image screenFade;
	public Animator animator;

	public static SceneController instance = null;

	void Awake () {
		if (instance != null) {
			Destroy (this);
			return;
		} else {
			instance = this;
			// DontDestroyOnLoad(gameObject);
		}
	}

	public void LoadScene(string sceneLabel) {
		if (sceneLabel == "Game") {
			Cursor.visible = false;
		} else {
			Cursor.visible = true;
		}
		StartCoroutine (FadeToScene (sceneLabel));
	}

	private IEnumerator FadeToScene(string sceneLabel) {
		animator.SetBool ("Fade", true);

		yield return new WaitUntil (() => screenFade.color.a == 1);

		SceneManager.LoadScene (sceneLabel);

		animator.SetBool ("Fade", false);
	}
}