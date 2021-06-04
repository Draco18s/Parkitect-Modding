using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Assets.draco18s.ParkitectMod {
#if DLL_EXPORT
	public abstract class AbstractScreen : AbstractDarkRideProp {
		protected MeshRenderer screenRenderer;

		public override void Initialize() {
			base.Initialize();
			CustomSize cs = lazyLoadedCustomSizeBehaviour;
			if(cs != null) {
				float v = cs.getValue();
				cs.setValue(0);
				cs.axisScale = new Vector3(1, 0, 1);
				cs.OnSizeChanged += SizeChange;
				cs.setValue(v);
			}
			if(!isPreview && !dontSerialize) {
				Transform screenTransform = transform.Find("Screen");
				if(screenTransform != null) {
					screenRenderer = screenTransform.GetComponent<MeshRenderer>();
					screenRenderer.sharedMaterial = ScriptableSingleton<AssetManager>.Instance.tvImageMaterial;
				}
				TVImageController.Instance.setup();
				SetMaterial(TVImageController.Instance.tvImageMaterialInstance);
			}
		}

		private void SetMaterial(Material material) {
			foreach(Renderer renderer in base.GetComponentsInChildren<Renderer>()) {
				Material[] sharedMaterials = renderer.sharedMaterials;
				for(int j = 0; j < sharedMaterials.Length; j++) {
					if(sharedMaterials[j] == ScriptableSingleton<AssetManager>.Instance.tvImageMaterial || sharedMaterials[j] == TVImageController.Instance.tvImageMaterialInstance || sharedMaterials[j] == TVImageController.Instance.tvImageBrokenMaterialInstance) {
						sharedMaterials[j] = material;
					}
				}
				renderer.sharedMaterials = sharedMaterials;
			}
		}

		protected virtual void SizeChange() {
			if(screenRenderer != null) {
				block.SetVector("_MainTex_ST", new Vector4(transform.localScale.x, transform.localScale.y, 0, 0));
				screenRenderer.SetPropertyBlock(block);
			}
			MarkDirty(transform.position);
		}

		private void MarkDirty(Vector3 position) {
			/*CrossedTiles tiles = getCrossedTiles();
			if(tiles.crossedTilesInfo == null) {
				throw new Exception("tiles.crossedTilesInfo was null");
			}
			int minx = tiles.crossedTilesInfo.Min(x => x.getLocalX());
			int minz = tiles.crossedTilesInfo.Min(x => x.getLocalZ());
			int maxx = tiles.crossedTilesInfo.Max(x => x.getLocalX());
			int maxz = tiles.crossedTilesInfo.Max(x => x.getLocalZ());

			FieldInfo fi = typeof(VisibilityGrid).GetField("dirtyCellPositionMainThreadRequests", BindingFlags.NonPublic | BindingFlags.Instance);
			if(fi == null) {
				throw new Exception("dirtyCellPositionMainThreadRequests was null");
			}
			IntVector3 from = new IntVector3(position + new Vector3(minx, minz));
			IntVector3 to = new IntVector3(position + new Vector3(maxx, maxz));
			HashSet<IntVector3> set = (HashSet<IntVector3>)fi.GetValue(gameController.park.visibilityGrid);
			MethodInfo mi = typeof(VisibilityGrid).GetMethod("markAreaDirtyCube", BindingFlags.NonPublic | BindingFlags.Instance);
			if(mi == null) {
				throw new Exception("markAreaDirtyCube was null");
			}
			mi.Invoke(gameController.park.visibilityGrid, new object[] { from, to, set });
			fi.SetValue(gameController.park.visibilityGrid, set);*/
		}

		protected override CrossedTiles getCrossedTilesInstance() {
			return GetCrossedTilesInstance();
		}

		protected abstract CrossedTiles GetCrossedTilesInstance();
	}
#endif
}
