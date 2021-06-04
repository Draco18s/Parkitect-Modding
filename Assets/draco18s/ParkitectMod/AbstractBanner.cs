using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Assets.draco18s.ParkitectMod {
#if DLL_EXPORT
	public abstract class AbstractBanner : AbstractDarkRideBanner {

		public override void Initialize() {
			emptyTexture = ScriptableSingleton<AssetManager>.Instance.imageSignDefaultTextureTwoOne;
			CustomSize cs = lazyLoadedCustomSizeBehaviour;
			if(cs != null) {
				float v = cs.getValue();
				cs.setValue(0);
				cs.axisScale = new Vector3(1, 0, 1);
				cs.OnSizeChanged += SizeChange;
				cs.setValue(v);
			}
			if(!isPreview && !dontSerialize) {
				Transform screenTransform = transform.Find("Sign");
				if(screenTransform != null) {
					bannerMeshRenderer = screenTransform.GetComponent<MeshRenderer>();
					bannerMeshRenderer.sharedMaterial = ScriptableSingleton<AssetManager>.Instance.imageSignMaterial;
				}
			}
			base.Initialize();
		}

		protected virtual void SizeChange() {
			if(bannerMeshRenderer != null) {
				//block.SetVector("_MainTex_ST", new Vector4(transform.localScale.x, transform.localScale.y, 0, 0));
				//screenRenderer.SetPropertyBlock(block);
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
