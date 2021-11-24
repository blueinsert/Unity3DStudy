using UnityEngine;
using System.Collections.Generic;
using Pathfinding.RVO;
using Pathfinding.RVO.Sampled;

namespace Pathfinding.Examples {
	/// <summary>
	/// RVO Example Scene Unit Controller.
	/// Controls AIs and camera in the RVO example scene.
	/// </summary>
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_examples_1_1_group_controller.php")]
	public class GroupController : MonoBehaviour {
		public GUIStyle selectionBox;
		public bool adjustCamera = true;

		Vector2 start, end;
		bool wasDown = false;

		Simulator sim;

		Camera cam;

		public void Start () {
			cam = Camera.main;
			var simu = FindObjectOfType(typeof(RVOSimulator)) as RVOSimulator;
			if (simu == null) {
				this.enabled = false;
				throw new System.Exception("No RVOSimulator in the scene. Please add one");
			}

			sim = simu.GetSimulator();
		}

		public void Update () {
			if (Screen.fullScreen && Screen.width != Screen.resolutions[Screen.resolutions.Length-1].width) {
				Screen.SetResolution(Screen.resolutions[Screen.resolutions.Length-1].width, Screen.resolutions[Screen.resolutions.Length-1].height, true);
			}

			if (adjustCamera) {
				//Adjust camera
				List<Agent> agents = sim.GetAgents();

				float max = 0;
				for (int i = 0; i < agents.Count; i++) {
					float d = Mathf.Max(Mathf.Abs(agents[i].Position.x), Mathf.Abs(agents[i].Position.y));
					if (d > max) {
						max = d;
					}
				}

				float hh = max / Mathf.Tan((cam.fieldOfView*Mathf.Deg2Rad/2.0f));
				float hv = max / Mathf.Tan(Mathf.Atan(Mathf.Tan(cam.fieldOfView*Mathf.Deg2Rad/2.0f)*cam.aspect));

				var yCoord = Mathf.Max(hh, hv)*1.1f;
				yCoord = Mathf.Max(yCoord, 20);
				cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(0, yCoord, 0), Time.smoothDeltaTime*2);
			}
		}

		// Update is called once per frame
		void OnGUI () {
			if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && !Input.GetKey(KeyCode.A)) {
				Select(start, end);
				wasDown = false;
			}

			if (Event.current.type == EventType.MouseDrag && Event.current.button == 0) {
				end = Event.current.mousePosition;

				if (!wasDown) { start = end; wasDown = true; }
			}

			if (Input.GetKey(KeyCode.A)) wasDown = false;
			if (wasDown) {
				Rect r = Rect.MinMaxRect(Mathf.Min(start.x, end.x), Mathf.Min(start.y, end.y), Mathf.Max(start.x, end.x), Mathf.Max(start.y, end.y));
				if (r.width > 4 && r.height > 4)
					GUI.Box(r, "", selectionBox);
			}
		}

	

		public void Select (Vector2 _start, Vector2 _end) {
			_start.y = Screen.height - _start.y;
			_end.y = Screen.height - _end.y;

			Vector2 start = Vector2.Min(_start, _end);
			Vector2 end = Vector2.Max(_start, _end);

			if ((end-start).sqrMagnitude < 4*4) return;

		}

		/// <summary>Radians to degrees constant</summary>
		const float rad2Deg = 360.0f/ ((float)System.Math.PI*2);

		/// <summary>Color from an angle</summary>
		public Color GetColor (float angle) {
			return Math.HSVToRGB(angle * rad2Deg, 0.8f, 0.6f);
		}
	}
}
