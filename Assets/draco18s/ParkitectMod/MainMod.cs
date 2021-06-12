using System.Collections.Generic;
using System.Reflection;
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
		private GameObject hider;
		private List<UnityEngine.Object> assetObjects = new List<UnityEngine.Object>();

		public MainMod() {
			instance = this;
		}

		public override void onEnabled() {
			base.onEnabled();
			EventManager.Instance.OnBuildableObjectBuilt += BuildTrigger;

			AbstractMod m = ModManager.Instance.getMod("com.themeparkitect.Chainlink Fence").mod;
			FieldInfo packField = m.GetType().GetField("assetPack", BindingFlags.NonPublic | BindingFlags.Instance);
			AssetPack pack = (AssetPack)packField.GetValue(m);
			FieldInfo hiderField = m.GetType().GetField("hider", BindingFlags.NonPublic | BindingFlags.Instance);
			hider = (GameObject)hiderField.GetValue(m);
			foreach(Asset asset in pack.Assets) {
				GameObject go = GameObject.Find(asset.Guid + "(Clone)");
				if(go == null) continue;
				go.name = asset.Guid;
				if(go.transform.parent != null) continue;
				new MaterialDecorator().Decorate(go, asset, null);
				new CustomColorDecorator().Decorate(go, asset, null);
				new LightEffectsDecorator().Decorate(go, asset, null);
				new BuildModeDecorator().Decorate(go, asset, null);
				new BoundingBoxDecorator().Decorate(go, asset, null);
				registerGameObject(asset, go);
			}
		}

		public override void onDisabled() {
			base.onDisabled();
			EventManager.Instance.OnBuildableObjectBuilt -= BuildTrigger;
			foreach(UnityEngine.Object obj in assetObjects) {
				ScriptableSingleton<AssetManager>.Instance.unregisterObject(obj);
			}
		}

		private void BuildTrigger(BuildableObject buildableObject) {
			
		}

		private void registerGameObject(Asset asset, GameObject gameObject) {
			GameObject.DontDestroyOnLoad(gameObject);
			SerializedMonoBehaviour component = gameObject.GetComponent<SerializedMonoBehaviour>();
			component.dontSerialize = true;
			component.isPreview = true;
			ScriptableSingleton<AssetManager>.Instance.registerObject(component);
			assetObjects.Add(component);
			BuildableObject buildableObject = component as BuildableObject;
			if(buildableObject != null) {
				buildableObject.setDisplayName(asset.Name);
				buildableObject.price = asset.Price;
				buildableObject.canBeRefunded = false;
				buildableObject.isStatic = true;
				if(string.IsNullOrEmpty(asset.SubCategory)) {
					buildableObject.categoryTag = asset.Category;
				}
				else {
					buildableObject.categoryTag = asset.Category + "/" + asset.SubCategory;
				}
			}
			Object.DontDestroyOnLoad(component.gameObject);
			gameObject.transform.SetParent(hider.transform);
			return;
		}
	}
#endif
}
