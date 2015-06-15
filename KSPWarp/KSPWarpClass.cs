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

		private void Start ()
		{
			print ("====== Loading KSPWarp ======");

			timewarp = new TimeWarp ();
			baseRates = new float[] {
				1.0f,
				5.0f,
				10.0f,
				50.0f,
				100.0f,
				1000.0f,
				10000.0f,
				100000.0f
			};
			superRates = new float[] {
				1.0f,
				10.0f,
				50.0f,
				100.0f,
				1000.0f,
				10000.0f,
				100000.0f,
				1000000.0f
			};
			ultraRates = new float[] {
				1.0f,
				50.0f,
				100.0f,
				1000.0f,
				10000.0f,
				100000.0f,
				1000000.0f,
				10000000.0f
			};
			rates = new float[][] { baseRates, superRates, ultraRates };
			setRates (0);
			UnityEngine.Object.DontDestroyOnLoad (this);
		}

		public void Update ()
		{
			if (HighLogic.LoadedScene.Equals (GameScenes.TRACKSTATION) || HighLogic.LoadedScene.Equals (GameScenes.FLIGHT)) {

				RenderingManager.AddToPostDrawQueue (3, new Callback (drawGUI));
				if (FindObjectOfType (typeof(TimeWarp)) != null)
					this.timewarp = (TimeWarp)FindObjectOfType (typeof(TimeWarp));


				/** anti Kaboom thingy from Planet Factory **/
				bool speedMax = (this.timewarp.warpRates.Equals (superRates) && (this.timewarp.current_rate_index == 7));
				bool ultraMax = (this.timewarp.warpRates.Equals (ultraRates) && (this.timewarp.current_rate_index >= 6));

				if (!speedMax || ultraMax) 
					ToggleCollisions (true);
				else
					ToggleCollisions (false);
				
			}
		}

		private void setRates (int i)
		{
			if (i >= 0 || i <= 2) 
				this.timewarp.warpRates = rates [i];
		}

		int getCurrentRate ()
		{
			int currentRate;
			if (this.timewarp.warpRates.Equals (baseRates))
				currentRate = 0;
			else if (this.timewarp.warpRates.Equals (superRates))
				currentRate = 1;
			else
				currentRate = 2;
			return currentRate;
		}


		/*******************************************
        * 					GUI
        * ******************************************/
		protected Rect windowPos = new Rect (Screen.width / 6, 0, 75, 50);
		bool basicBool, superBool, ultraBool;

		private void WindowGUI (int windowID)
		{
			/** Style definition **/
			GUIStyle style = new GUIStyle (GUI.skin.button);
			style.padding = new RectOffset (2, 2, 2, 2);
			style.active.textColor = Color.red;

			/** Gui window definition **/
			GUILayout.BeginHorizontal ();
			basicBool = GUILayout.Button ("Basic", style, GUILayout.ExpandWidth (true));
			superBool = GUILayout.Button ("Super", style, GUILayout.ExpandWidth (true));
			ultraBool = GUILayout.Button ("Ultra", style, GUILayout.ExpandWidth (true));
				
			if (basicBool) { // If user clicks on Basic, set warp rates to stock
				print ("==== Set rates 0 ====");
				ScreenMessages.PostScreenMessage ("Entering Basic rates", 4, ScreenMessageStyle.UPPER_CENTER);
				setRates (0);
			}
			if (superBool) { // If user clicks on Super, set warp rates to custom "super"
				print ("==== Set rates 1 ====");
				ScreenMessages.PostScreenMessage ("Entering Super rates", 4, ScreenMessageStyle.UPPER_CENTER);
				setRates (1);
			}
			if (ultraBool) { // If user clicks on Ultra, set warp rates to custom "ultra"
				print ("==== Set rates 2 ====");
				ScreenMessages.PostScreenMessage ("Entering Ultra rates", 4, ScreenMessageStyle.UPPER_CENTER);
				setRates (2);
			}
			GUILayout.EndHorizontal ();

			GUI.DragWindow (new Rect (0, 0, 1000, 20));
		}

		private void drawGUI ()
		{
			GUI.skin = HighLogic.Skin;
			windowPos = GUILayout.Window (1, windowPos, WindowGUI, "Time warp rates", GUILayout.MinWidth (10));

		}


		/*******************************************
		 * 			Collision handling
		 * *****************************************/
		bool isCollisionActive = true;

		private void ToggleCollisions (bool enabled)
		{
			if (enabled != isCollisionActive) { // If collisions aren't already enabled/disabled

				isCollisionActive = enabled;

				//Enable/Disable collisions for all parts that makes the vessel
				foreach (var vessel in FlightGlobals.Vessels) {
					print ("**Debug : Collisions on " + vessel.GetName() + " are now " + enabled + "**");
					foreach (var part in vessel.GetComponentsInChildren<Collider>())
						part.enabled = enabled;
				}

				// Enable/Disable collisions for all Celestial bodies, just to be sure...
				foreach (var body in FlightGlobals.Bodies) {
					print ("**Debug : Collisions on " + body.GetName() + " are now " + enabled + "**");
					body.GetComponentInChildren<Collider> ().enabled = enabled;
				}
			}
		}
	}
}