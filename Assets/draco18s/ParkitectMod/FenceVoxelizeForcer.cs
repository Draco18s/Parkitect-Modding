using System.Collections;
using UnityEngine;

namespace Assets.draco18s.ParkitectMod {
#if DLL_EXPORT
	public class FenceVoxelizeForcer : MonoBehaviour {
		void Start() {
			StartCoroutine(UpdateVoxels());
		}
		IEnumerator UpdateVoxels() {
			Debug.Log("Updating Fence Voxels!");
			yield return new WaitForEndOfFrame();
			Fence fence = GetComponent<Fence>();
			if(fence) {
				fence.canSeeThrough = false;
				fence.flatGO = fence.transform.Find("VisBlocker").gameObject;
				GameController.Instance.park.visibilityGrid.update(fence);
			}
		}
	}
#endif
}
