using BepInEx;
using Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wizgun;
using System.Collections;
using BepInEx.Configuration;
namespace WWAG_RainbowEyes
{
	// Plugin Info
    [BepInPlugin("org.NikoTheFox.RainbowEyes", "WWAG Rainbow Eyes", "0.0.2")]
    public class Plugin : BaseUnityPlugin
    {
		public static ConfigEntry<float> configSpeed;

		private void Awake()
        {
			configSpeed = Config.Bind("General",      // The section under which the option is shown
										"ColorSpeed",  // The key of the configuration option in the configuration file
										 0.04f, // The default value
										 "How fast the eye color changes"); // Description of the option to show in the config file

			Debug.Log($"Plugin RainbowEyes is loaded!");
		}
		void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (scene.name == "scene-tower" || scene.name == "scene-overworld-demo")
			{
				CoroutineRunner.Instance.RunCoroutine(DelayedInitialization());

			}
		}

		IEnumerator DelayedInitialization()
		{
			yield return new WaitForSeconds(10);  // Adjust the wait time if necessary

			Debug.Log($"Found tower scene, loading animation");
			GameObject player = GameObject.Find("player(Clone)");

			if (player == null)
			{
				Debug.Log("player Object not found");
				yield break;
			}

			Transform playerVisualTransform = player.transform.Find("player-visual/Main Visuals/spine-object");

			if (playerVisualTransform == null)
			{
				Debug.Log("playerVis Object not found");
				yield break;
			}

			Debug.Log("Found spine object, attaching");
			GameObject PlayerVis = playerVisualTransform.gameObject;

			PlayerVis.AddComponent<RainbowColors>();
		}
		void OnEnable()
		{
			Debug.Log($"Plugin enabled");
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

	}
	public class RainbowColors : MonoBehaviour
	{
		public float hue = 0.0f; // Hue value between 0 and 1
		public float changeSpeed = 0.04f; // Speed of hue change
		private void Awake()
		{
			Debug.Log("In MonoBehavior for Rainbow");
			changeSpeed = Plugin.configSpeed.Value;
			Debug.Log("Speed: " + changeSpeed);
		}

		

		void Update()
		{
			changeSpeed = Plugin.configSpeed.Value;
			hue += changeSpeed * Time.deltaTime; // Update the hue value
			if (hue > 1.0f)
				hue -= 1.0f;

			Color rainbowColor = Color.HSVToRGB(hue, 1, 1);
			GameObject PlayerVis = this.gameObject;
			Skins.PlayerSkins playerSkins = PlayerVis.GetComponent<Skins.PlayerSkins>();
			playerSkins.SetSlotColor(Skins.ColorSlot.Eyes, rainbowColor);
			playerSkins.SetAllColors();
		}

	}

	public class CoroutineRunner : MonoBehaviour
	{
		// Singleton instance
		private static CoroutineRunner _instance;

		public static CoroutineRunner Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GameObject("CoroutineRunner").AddComponent<CoroutineRunner>();
					DontDestroyOnLoad(_instance.gameObject);
				}
				return _instance;
			}
		}

		public void RunCoroutine(IEnumerator routine)
		{
			StartCoroutine(routine);
		}
	}

}
