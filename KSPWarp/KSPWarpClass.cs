using System;
using System.Collections;
using UnityEngine;

namespace KSPWarp
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class KSPWarpClass : MonoBehaviour
	{
		private TimeWarp timewarp;
		private static float[] baseRates,
		superRates,
		ultraRates;
		private float[][] rates;

		private void Awake()
		{
			print ("====== Welcome to Askirkela's awesome mod! ======");
			print ("Expect some crashes, explosions, headaches and nausea");
			print ("====== Enjoy! ======");
			timewarp = new TimeWarp();
			baseRates = new float[] { 1, 5, 10, 50, 100, 1000, 10000, 100000 };
			superRates = new float[] { 1, 10, 50, 100, 1000, 10000, 100000, 1000000 };
			ultraRates = new float[] { 1, 50, 100, 1000, 10000, 100000, 1000000, 10000000 };
			rates = new float[][] { baseRates, superRates, ultraRates };
			setRates (0);
			UnityEngine.Object.DontDestroyOnLoad(this);
		}

		public void Update()
		{
			if (HighLogic.LoadedScene.Equals(GameScenes.TRACKSTATION) || HighLogic.LoadedScene.Equals(GameScenes.FLIGHT)) {

				RenderingManager.AddToPostDrawQueue (3, new Callback (drawGUI));
				if(FindObjectOfType (typeof(TimeWarp)) != null)
					this.timewarp = (TimeWarp)FindObjectOfType (typeof(TimeWarp));


				/** anti Kaboom thingy from Planet Factory **/
				bool speedMax = (this.timewarp.warpRates.Equals (superRates) && (this.timewarp.current_rate_index == 7));
				bool ultraMax = (this.timewarp.warpRates.Equals (ultraRates) && (this.timewarp.current_rate_index >= 6));
				if (speedMax || ultraMax) 
					ToggleCollisions (true);
				else
					ToggleCollisions (false);
				
			}
		}


		private void setRates(int i)
		{
			if(i >= 0 || i <= 2) 
				this.timewarp.warpRates = rates[i];
		}

		int getCurrentRate()
		{
			int currentRate;
			if (this.timewarp.warpRates.Equals(baseRates))
				currentRate = 0;
			else if (this.timewarp.warpRates.Equals(superRates))
				currentRate = 1;
			else
				currentRate = 2;
			return currentRate;
		}


		/*******************************************
        * 					GUI
        * ******************************************/
		protected Rect windowPos = new Rect(Screen.width/6, 0, 50, 50);
		bool basicBool, superBool, ultraBool;

		private void WindowGUI(int windowID)
		{
			GUIStyle style = new GUIStyle(GUI.skin.button);
			style.padding = new RectOffset(2, 2, 2, 2);
			style.active.textColor = Color.red;

			int currentIndex = this.timewarp.current_rate_index;
			int currentRate = getCurrentRate();

			GUILayout.BeginHorizontal();
			basicBool = GUILayout.Button ("Basic", style, GUILayout.ExpandWidth(true));
			superBool = GUILayout.Button ("Super", style, GUILayout.ExpandWidth(true));
			ultraBool = GUILayout.Button ("Ultra", style, GUILayout.ExpandWidth(true));
			if (basicBool)
			{
				print("==== Set rates 0 ====");
				ScreenMessages.PostScreenMessage("Entering Basic rates", 4, ScreenMessageStyle.UPPER_CENTER);
				setRates(0);
			}
			if (superBool)
			{
				print("==== Set rates 1 ====");
				ScreenMessages.PostScreenMessage("Entering Super rates", 4, ScreenMessageStyle.UPPER_CENTER);
				setRates(1);
			}
			if (ultraBool)
			{
				print("==== Set rates 2 ====");
				ScreenMessages.PostScreenMessage("Entering Ultra rates", 4, ScreenMessageStyle.UPPER_CENTER);
				setRates(2);
			}
			GUILayout.EndHorizontal();

			GUI.DragWindow(new Rect(0, 0, 1000, 20));
		}

		private void drawGUI()
		{
			GUI.skin = HighLogic.Skin;
			windowPos = GUILayout.Window(1, windowPos, WindowGUI, "Time warp rates", GUILayout.MinWidth(10));
		}


		/*******************************************
		 * 			Collision handling
		 * *****************************************/
		bool isCollisionActive = true;
		private void ToggleCollisions(bool enabled)
		{
			if (enabled != isCollisionActive) {
				isCollisionActive = enabled;
				foreach (var part in FlightGlobals.ActiveVessel.GetComponentsInChildren<Collider>()) {
					part.enabled = enabled;
				}
				foreach (var body in FlightGlobals.Bodies) {
					body.GetComponentInChildren<Collider> ().enabled = enabled;
				}
			}
		}
	}
}