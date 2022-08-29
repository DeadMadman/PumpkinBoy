using System;
using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[SerializeField] private Image button;
	[SerializeField] private int StartSceneIndex;
	[SerializeField] private GameObject OptionsMenu;
	[SerializeField] private GameObject Menu;
	private Fade fade;
	private Color color;
	[SerializeField] private ScriptableScore candy;

	private void Awake()
	{
		fade = GetComponent<Fade>();
		fade.FadeIn();
		color = button.color;
	}

	private void Start()
	{
		var selectables = Menu.GetComponentsInChildren<Selectable>();
		foreach (var s in selectables)
		{
			if (s.enabled)
			{
				EventSystem.current.SetSelectedGameObject(s.gameObject);
				break;
			}
		}
	}
	
	public void LoadStartScene()
	{
		fade.FadeOut();
		candy.score = 0;
		StartCoroutine(WaitAndLoad(StartSceneIndex));
	}

	public void LoadOptionsScene()
	{
		button.color = color;
		OptionsMenu.SetActive(true);
		Menu.SetActive(false);
		Selectable selectable = OptionsMenu.GetComponentInChildren<Selectable>();
		EventSystem.current.SetSelectedGameObject(selectable.gameObject);
	}
	
	public void UnloadOptionsScene()
	{
		OptionsMenu.SetActive(false);
	}

	public void LoadScene(int sceneIndex)
	{
		fade.FadeOut();
		StartCoroutine(WaitAndLoad(sceneIndex));
	}

	public void Quit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
					Application.Quit();
#endif
	}

	private IEnumerator WaitAndLoad(int nextSceneIndex)
	{
		yield return new WaitUntil(() => fade.faded);
		SceneManager.LoadScene(nextSceneIndex);
	}

	private void Update()
	{
		if (Keyboard.current != null)
		{
			if (Keyboard.current.anyKey.isPressed || Mouse.current.leftButton.isPressed ||
			    Mouse.current.rightButton.isPressed)
			{
				PlayerPrefs.SetInt("Controller", 0);
			}
		}
		else if (Gamepad.current != null) {
			if (Gamepad.current.xButton.isPressed || Gamepad.current.yButton.isPressed || 
			    Gamepad.current.aButton.isPressed || Gamepad.current.bButton.isPressed)
			{
				PlayerPrefs.SetInt("Controller", 1);
			}
		}
		
		//Debug.LogError(PlayerPrefs.GetInt("Controller"));
	}
}
