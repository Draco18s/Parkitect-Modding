using System;
using UnityEngine;

namespace Assets.draco18s {
	public class VoxelVisualizer :
#if DLL_EXPORT
		Deco
#else
		MonoBehaviour
#endif
		{
#if DLL_EXPORT
		public override void Initialize() {
			base.Initialize();
			if(!isPreview) {
				Debug.Log("Ahoy hoy: 20x20!"); //change this!
				for(int x = 0; x < 20; x++) {
					for(int z = 0; z < 20; z++) {
						for(int y = 0; y < 8; y++) { //map height
							VisibilityGridUnitCell cell = GameController.Instance.park.visibilityGrid.getCell(x, y, z);
							if(cell == null) continue;
							byte b = cell.opaqueCells;
							if(b == 0) continue;
							//Debug.Log(x + "," + y + "," + z);
							PlaceCubeAt(x, y, z, b);
						}
					}
				}
			}
		}
		private const byte LEFT = 85;
		private void PlaceCubeAt(int x, int y, int z, byte b) {
			Vector3 pos = new Vector3(x, y, z);
			for(int bi = 1; bi <= 128; bi = bi << 1) {
				if((b & bi) == 0) continue;
				Vector3 halfPos = VisibilityGridCell.getOffset((byte)bi) / 2;//new Vector3((VisibilityGridCell.RIGHT & bi) == 0 ? 0 : 1, (VisibilityGridCell.TOP & bi) == 0 ? 0 : 1, (VisibilityGridCell.FRONT & bi) == 0 ? 0 : 1) / 2;
				GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
				go.layer = gameObject.layer;
				go.tag = gameObject.tag;
				go.transform.position = pos + halfPos + Vector3.one/4;
				go.transform.localScale = Vector3.one / 2;
				go.transform.SetParent(transform, true);
				Destroy(go.GetComponent<Collider>());
			}
		}
#endif
	}
}
