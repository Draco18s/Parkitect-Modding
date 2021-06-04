using UnityEngine;

namespace Assets.draco18s.ParkitectMod {
	public class CurvedScreenDeco :
#if DLL_EXPORT
		AbstractScreen
#else
		MonoBehaviour
#endif
		{
#if DLL_EXPORT
		protected override CrossedTiles GetCrossedTilesInstance() {
			if(_crossedTiles != null && Mathf.Abs(base.transform.eulerAngles.y - _crossedTiles.rotationY) >= 1f) {
				_crossedTiles = null;
			}
			if(_crossedTiles != null && Vector3.Distance(transform.position, new Vector3(_crossedTiles.offsetX, _crossedTiles.offsetY, _crossedTiles.offsetZ)) >= 0.1f) {
				_crossedTiles = null;
			}
			if(_crossedTiles == null) {
				_crossedTiles = new CrossedTiles();
				_crossedTiles.rotationY = transform.eulerAngles.y;
				float size = lazyLoadedCustomSizeBehaviour.getValue();
				float rot = (transform.eulerAngles.y) * Mathf.Deg2Rad;
				float cos = Mathf.Cos(rot);
				float sin = Mathf.Sin(rot);
				float ox = transform.position.x - Mathf.FloorToInt(transform.position.x);
				float oz = transform.position.z - Mathf.FloorToInt(transform.position.z);
				Vector2 vn = new Vector2(sin + cos, cos - sin);
				vn = new Vector2(0.5f - vn.x / 2, 0.5f - vn.y / 2) * Mathf.CeilToInt(size - 0.99f);
				//debugString = "";
				float oth = (size * 1f) - 0.95f;
				float ith = Mathf.Max(oth - 1, 0);
				for(float x = 0; x <= Mathf.CeilToInt(size); x += 0.25f) {
					for(float z = 0; z <= Mathf.CeilToInt(size); z += 0.25f) {
						Vector2 vc = new Vector2(x, z);
						if(vc.sqrMagnitude <= oth * oth && vc.sqrMagnitude >= ith * ith) {
							Vector2 v2 = new Vector2(vc.x * sin + vc.y * cos, vc.x * cos - vc.y * sin) - vn;
							Vector2Int vvv = new Vector2Int(Mathf.RoundToInt(v2.x + ox), Mathf.RoundToInt(v2.y + oz));
							if(!_crossedTiles.isCrossedLocal(vvv.x, vvv.y)) {
								//debugString += "\n" + vvv.ToString();
								_crossedTiles.addCrossedTile(new CrossedTileInfo(vvv.x, vvv.y));
							}
						}
					}
				}
			}
			return _crossedTiles;
		}

		protected override void SizeChange() {
			Vector3 p = Vector3.one + lazyLoadedCustomSizeBehaviour.axisScale * (lazyLoadedCustomSizeBehaviour.getValue() - 1);
			transform.localScale = new Vector3(p.x - 0.05f, p.y, p.z - 0.05f);
			base.SizeChange();
		}
	}
#else
		public float height;
	}
#endif
}
