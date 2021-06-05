using System.Collections.Generic;
#if DLL_EXPORT
using Parkitect.UI;
using Parkitect.Mods.AssetPacks;
#endif
using UnityEngine;

namespace Assets.draco18s.ParkitectMod {
#if DLL_EXPORT
	public class MainMod : AssetMod {
		public static string VERSION = "0.0.1a";
		public static MainMod instance;

		public MainMod() {
			instance = this;
		}

		public override void onEnabled() {
			base.onEnabled();
			EventManager.Instance.OnBuildableObjectBuilt += BuildTrigger;
		}

		public override void onDisabled() {
			base.onDisabled();
			EventManager.Instance.OnBuildableObjectBuilt -= BuildTrigger;
		}

		private void BuildTrigger(BuildableObject buildableObject) {
			if(buildableObject.transform.parent != null) return;
			if(buildableObject.getReferenceName().ToLower().Contains("nozzle")) {
				Debug.Log("Built a nozzle!");
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				BuilderMousePositionInfo builderMousePositionInfo = default(BuilderMousePositionInfo);
				builderMousePositionInfo.hitDistance = float.MaxValue;
				GameObject nozzleMeshObj = null;
				foreach(MouseCollider.HitInfo hitInfo in MouseCollisions.Instance.raycastAll(ray, builderMousePositionInfo.hitDistance)) {
					if(hitInfo.hitDistance < builderMousePositionInfo.hitDistance) {
						IMouseSelectable componentInParent = hitInfo.hitObject.GetComponentInParent<IMouseSelectable>();
						if((componentInParent == null || componentInParent.canBeSelected()) && hitInfo.hitObject != buildableObject.gameObject) {
							int num = 1 << hitInfo.hitObject.layer;
							builderMousePositionInfo.hitSomething = true;
							builderMousePositionInfo.hitObject = hitInfo.hitObject;
							builderMousePositionInfo.hitPosition = hitInfo.hitPosition;
							builderMousePositionInfo.hitDistance = hitInfo.hitDistance;
							builderMousePositionInfo.hitNormal = hitInfo.hitNormal;
							builderMousePositionInfo.hitLayerMask = num;
						}
						if((componentInParent == null || componentInParent.canBeSelected()) && hitInfo.hitObject == buildableObject.gameObject) {
							nozzleMeshObj = hitInfo.hitObject;
						}
					}
				}
				if(builderMousePositionInfo.hitSomething) {
					Car car = builderMousePositionInfo.hitObject.GetComponent<Car>();
					if(car != null) {
						buildableObject.gameObject.transform.SetParent(car.transform, true);
						buildableObject.GetComponent<Renderer>().enabled = false;
					}
				}
			}
		}
	}
#endif
}
