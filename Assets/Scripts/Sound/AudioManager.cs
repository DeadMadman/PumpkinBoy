using UnityEngine;
using UnityEngine.Audio;
using System;
using MoreMountains.Feedbacks;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
	public Sound[] sounds;
	//[SerializeField] private MMFeedbacks mainMenuFeedback;
	//[SerializeField] private MMFeedbacks gameplayFeedback;
	public static AudioManager currentInstance;


	private void Awake()
	{
		if (currentInstance == null)
			currentInstance = this;
		else
		{
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);

		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;

			s.source.volume = s.volume;
			s.source.pitch = s.pitch;
			s.source.loop = s.loop;
			s.source.spatialBlend = s.spatialBlend;
		}
	}

	public void Play(string name)
	{
		Sound s = Array.Find(sounds, sound => sound.name == name);

		if (s == null)// change when we dont need debugging
		{
			Debug.LogWarning("Sound:" + name + "not found!!!!");
			return;
		}
		s.source.Play();
	}

	/*private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
	{
		if (scene.buildIndex != 0)
		{
			mainMenuFeedback.StopFeedbacks(true);
			gameplayFeedback.PlayFeedbacks();
		}
		else
		{
			gameplayFeedback.StopFeedbacks(true);
			mainMenuFeedback.PlayFeedbacks();
		}
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}*/
}
