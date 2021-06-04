using UnityEngine;

namespace Assets.draco18s.ParkitectMod {
	public class FlatBannerDeco :
#if DLL_EXPORT
		AbstractBanner
#else
		MonoBehaviour
#endif
		{
#if DLL_EXPORT
		protected override CrossedTiles GetCrossedTilesInstance() {
			if(_crossedTiles != null && Mathf.Abs(transform.eulerAngles.y - _crossedTiles.rotationY) >= 1f) {
				_crossedTiles = null;
			}
			if(_crossedTiles != null && Vector3.Distance(transform.position, new Vector3(_crossedTiles.offsetX, _crossedTiles.offsetY, _crossedTiles.offsetZ)) >= 0.25f) {
				_crossedTiles = null;
			}
			if(_crossedTiles == null) {
				_crossedTiles = new CrossedTiles();
				_crossedTiles.addCrossedTile(new CrossedTileInfo(0, 0));
				_crossedTiles.rotationY = transform.eulerAngles.y;

				float size = lazyLoadedCustomSizeBehaviour.getValue();
				for(int x = 0; x < Mathf.CeilToInt(size); x++) {
					if(!_crossedTiles.isCrossedLocal(x, 0))
						_crossedTiles.addCrossedTile(new CrossedTileInfo(x, 0));
				}
			}
			return _crossedTiles;
		}
	}
#else
		public float height;
	}
#endif
}
