using System;
using System.Reflection;
#if DLL_EXPORT
using Parkitect.UI;
#endif
using UnityEngine;

namespace Assets.draco18s.ParkitectMod {
#if DLL_EXPORT
	public class MainMod : AssetMod {
		public static string VERSION = "0.0.1a";
		public static MainMod instance;

		public MainMod() {
			Debug.Log("I EXIST");
			instance = this;
		}

		/*public override string getName() {
			return "Test Mod";
		}

		public override string getDescription() {
			return "Flappy Flails";
		}

		public override string getVersionNumber() {
			return VERSION;
		}

		public override string getIdentifier() {
			return "draco18s_" + getName().ToLower().Replace(' ','_') + "_" + VERSION;
		}*/

		public override void onEnabled() {
			base.onEnabled();
			Debug.Log("Hello to my little fren");
			EventManager.Instance.OnBuildableObjectBuilt += BuildTrigger;
			//FindObjectOfType<DecoBuilderTab>();
			//Builder.OnBuildTriggered += BuildTrigger;
		}

		public override void onDisabled() {
			base.onDisabled();
			Debug.Log("Goodbye my little fren");
			EventManager.Instance.OnBuildableObjectBuilt -= BuildTrigger;
		}

		private void BuildTrigger(BuildableObject buildableObject) {
			Debug.Log("Built: " + buildableObject.getName());
			Debug.Log("     : " + buildableObject.getReferenceName());
			//if(buildableObject.getName().Contains("Nozzle")) {
				Debug.Log("Built a nozzle!");
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				BuilderMousePositionInfo builderMousePositionInfo = default(BuilderMousePositionInfo);
				builderMousePositionInfo.hitDistance = float.MaxValue;
				foreach(MouseCollider.HitInfo hitInfo in MouseCollisions.Instance.raycastAll(ray, builderMousePositionInfo.hitDistance)) {
					if(hitInfo.hitDistance < builderMousePositionInfo.hitDistance) {
						IMouseSelectable componentInParent = hitInfo.hitObject.GetComponentInParent<IMouseSelectable>();
						if((componentInParent == null || componentInParent.canBeSelected())) {
							int num = 1 << hitInfo.hitObject.layer;
							builderMousePositionInfo.hitSomething = true;
							builderMousePositionInfo.hitObject = hitInfo.hitObject;
							builderMousePositionInfo.hitPosition = hitInfo.hitPosition;
							builderMousePositionInfo.hitDistance = hitInfo.hitDistance;
							builderMousePositionInfo.hitNormal = hitInfo.hitNormal;
							builderMousePositionInfo.hitLayerMask = num;
						}
					}
				}
				if(builderMousePositionInfo.hitSomething) {
					Debug.Log(builderMousePositionInfo.hitObject.name);
				}
			//}
		}
	}
#endif
}
