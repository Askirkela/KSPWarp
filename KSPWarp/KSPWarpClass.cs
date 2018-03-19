using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSPWarp
{
	[KSPAddon (KSPAddon.Startup.FlightAndKSC, true)]
	public class KSPWarpClass : MonoBehaviour
	{
		private static String warpDebug = "[KSPWarp] : ";
		private static float[] baseRates = new float[] {
			1.0f,
			5.0f,
			10.0f,
			50.0f,
			100.0f,
			1000.0f,
			10000.0f,
			100000.0f
		},
			superRates = new float[] {
			1.0f,
			10.0f,
			50.0f,
			100.0f,
			1000.0f,
			10000.0f,
			100000.0f,
			200000.0f
		},
			ultraRates = new float[] {
			1.0f,
			50.0f,
			100.0f,
			1000.0f,
			10000.0f,
			100000.0f,
			200000.0f,
			500000.0f
		};
		private static float[] baseAltitude = new float[] {
			0.0f,
			70000.0f,
			70000.0f,
			70000.0f,
			120000.0f,
			240000.0f,
			480000.0f,
			600000.0f
		},
			superAltitude = new float[] {
			0.0f,
			70000.0f,
			70000.0f,
			120000.0f,
			240000.0f,
			480000.0f,
			600000.0f,
			1000000.0f
		},
			ultraAltitude = new float[] {
			0.0f,
			70000.0f,
			120000.0f,
			240000.0f,
			480000.0f,
			600000.0f,
			1000000.0f,
			1000000.0f
		};
		private float[][] rates = new float[][] { baseRates, superRates, ultraRates };
		private float[][] altitudes = new float[][] {
			baseAltitude,
			superAltitude,
			ultraAltitude
		};

		void Start ()
		{
			print (warpDebug + "Launching KSPWarp");
			UnityEngine.Object.DontDestroyOnLoad (this);
		}

		void FixedUpdate ()
		{
			//				/** anti Kaboom thingy from Planet Factory **/
			bool speedMax = (TimeWarp.fetch.warpRates.Equals (superRates) && (TimeWarp.fetch.current_rate_index == 7));
			bool ultraMax = (TimeWarp.fetch.warpRates.Equals (ultraRates) && (TimeWarp.fetch.current_rate_index >= 6));
			//
			if (!speedMax || !ultraMax)
				ToggleCollisions (true);
			else
				ToggleCollisions (false);
		
		}

		private void SetRates (int i)
		{
			if (i >= 0 || i < rates.Length) { 
				TimeWarp.fetch.warpRates = rates [i];
				TimeWarp.fetch.altitudeLimits = altitudes [i];
				print (warpDebug + "Warp rates set to " + i);
			} else {
				SetRates (0);
				print (warpDebug + "Warp rates set to default");
			}
		}

		private void OnDestroy ()
		{
			SetRates (0);	
		}

		/*******************************************
        	*			GUI
        	* ******************************************/
		protected Rect windowPos = new Rect (Screen.width / 6, 0, 75, 50);
		bool basicBool, superBool, ultraBool;

		private void OnGUI ()
		{
			DrawGUI ();
		}

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
				ScreenMessages.PostScreenMessage (warpDebug + "Entering Basic rates", 4, ScreenMessageStyle.UPPER_CENTER);
				SetRates (0);
			}
			if (superBool) { // If user clicks on Super, set warp rates to custom "super"
				ScreenMessages.PostScreenMessage (warpDebug + "Entering Super rates", 4, ScreenMessageStyle.UPPER_CENTER);
				SetRates (1);
			}
			if (ultraBool) { // If user clicks on Ultra, set warp rates to custom "ultra"
				ScreenMessages.PostScreenMessage (warpDebug + "Entering Ultra rates", 4, ScreenMessageStyle.UPPER_CENTER);
				SetRates (2);
			}
			GUILayout.EndHorizontal ();

			GUI.DragWindow (new Rect (0, 0, 1000, 20));
		}

		private void DrawGUI ()
		{
			GUI.skin = HighLogic.Skin;
			windowPos = GUILayout.Window (1, windowPos, WindowGUI, "Time warp rates", GUILayout.MinWidth (10));
		}


		/*******************************************
		 * 		Collision handling
		 * *****************************************/
		bool isCollisionActive = true;

		private void ToggleCollisions (bool enabled)
		{
			if (enabled != isCollisionActive) { // If collisions aren't already enabled/disabled
				isCollisionActive = enabled;

				//Enable/Disable collisions for all parts that makes the vessel
				List<Vessel> vessels = FlightGlobals.Vessels;
				Collider[] p;
				for (int i = 0; i < vessels.Count; i++) {
					print (warpDebug + "Collisions on " + vessels [i].GetName () + " are now " + enabled + "**");
					ScreenMessages.PostScreenMessage (warpDebug + "Collisions on " + vessels [i].GetName () + " are now " + enabled + "**", 1, ScreenMessageStyle.UPPER_CENTER);
					p = vessels [i].GetComponentsInChildren<Collider> ();
					for (int j = 0; j < p.Length; j++)
						p [j].enabled = enabled;
				}

				// Enable/Disable collisions for all Celestial bodies, just to be sure...
				List<CelestialBody> bodies = FlightGlobals.Bodies;
				for (int i = 0; i < bodies.Count; i++) {
					print (warpDebug + "Collisions on " + bodies [i].GetName () + " are now " + enabled + "**");
					ScreenMessages.PostScreenMessage (warpDebug + "Collisions on " + bodies [i].GetName () + " are now " + enabled + "**", 1, ScreenMessageStyle.UPPER_CENTER);
					bodies [i].GetComponentInChildren<Collider> ().enabled = enabled;
				}
			}
		}
	}
}