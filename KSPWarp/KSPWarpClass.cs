using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSPWarp
{
	[KSPAddon (KSPAddon.Startup.FlightAndKSC, false)]
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

		}

		int previousRate = 0;

		void FixedUpdate ()
		{
			if (toolbarInt != previousRate) {
				SetRates (toolbarInt);
				previousRate = toolbarInt;
			}

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
				ScreenMessages.PostScreenMessage (warpDebug + "Entering " + toolbarStrings [i] + " rates", 4, ScreenMessageStyle.UPPER_CENTER);
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
		private Rect windowPos = new Rect (300, 0, 150, 60);
		private int toolbarInt = 0;
		private String[] toolbarStrings = { ">", ">>", ">>>" };

		private void OnGUI ()
		{
			DrawGUI ();
//			EventVoid.OnEvent hide = new EventVoid.OnEvent (HideWindow);
//			EventVoid.OnEvent show = new EventVoid.OnEvent (DrawGUI);
//			GameEvents.onGUIMissionControlSpawn.Add (hide);
//			GameEvents.onGUIRnDComplexSpawn.Add (hide);
//			GameEvents.onGUIMissionControlDespawn.Add (show);
//			GameEvents.onGUIRnDComplexDespawn.Add (show);
		}

		private void WindowGUI (int windowID)
		{
			/** Style definition **/
			GUIStyle style = new GUIStyle (GUI.skin.button);
			style.padding = new RectOffset (2, 2, 2, 2);
			style.active.textColor = Color.red;

			/** Gui window definition **/
			toolbarInt = GUI.Toolbar (new Rect (0, 30, 150, 30), toolbarInt, toolbarStrings, style);

			GUI.DragWindow ();

		}

		private void DrawGUI ()
		{
			GUI.skin = HighLogic.Skin;
			windowPos = GUI.Window (1, windowPos, WindowGUI, "Time warp rates");
		}

		private void HideWindow ()
		{
			windowPos = new Rect (-100, -100, 150, 30);
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