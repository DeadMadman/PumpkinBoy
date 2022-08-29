using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenuHandler : MonoBehaviour
{
	[SerializeField] private GameObject pauseMenu;
	[SerializeField] private GameObject resumeButton;
	private SimpleControls inputs;
	[SerializeField] private ScriptableScore candy;
	

	private void Awake()
	{
		inputs = new SimpleControls();
	}
	private void OnEnable()
	{
		//remove this from build potentially
		inputs.Utilities.Reset.Enable();
		inputs.Utilities.Reset.started += OnRestartPressed;

		inputs.Gameplay.Menu.Enable();
		inputs.Gameplay.Menu.started += OnPauseGame;
		
		var selectables = pauseMenu.GetComponentsInChildren<Selectable>();
		foreach (var s in selectables)
		{
			if (s.enabled)
			{
				EventSystem.current.SetSelectedGameObject(s.gameObject);
				break;
			}
		}
	}

	private void OnDisable()
	{
		inputs.Utilities.Reset.Disable();
		inputs.Utilities.Reset.started -= OnRestartPressed;

		inputs.Gameplay.Menu.Disable();
		inputs.Gameplay.Menu.started -= OnPauseGame;
	}

	public void PauseGame()
	{
		if (pauseMenu.activeInHierarchy)
		{
			//turn off pause
			pauseMenu.SetActive(false);
			Time.timeScale = 1;
		}
		else
		{
			//turn on pause
			pauseMenu.SetActive(true);
			EventSystem.current.SetSelectedGameObject(resumeButton);
			Time.timeScale = 0;
		}
	}

	public void OnPauseGame(InputAction.CallbackContext context)
	{
		PauseGame();
	}

	public void RestartGame()
	{
		Time.timeScale = 1;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	void OnRestartPressed(InputAction.CallbackContext context)
	{
		RestartGame();
	}

	public void QuitGame()
	{
		candy.score = 0;
		Time.timeScale = 1;
		SceneManager.LoadScene(0);
		/*#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
				#else
					Application.Quit();
				#endif*/
	}

	private void OnApplicationQuit()
	{
		InputSystem.PauseHaptics();
	}
}
