using System.Collections;
using UnityEngine;

namespace Assets.draco18s {
	public class MBLoader : MonoBehaviour {
		public string assemblyName = "";
		public string classToLoad = "";
		// Start is called before the first frame update
		void Start() {
			StartCoroutine(WaitAndLoad());
		}
#if UNITY_EDITOR
		private void OnValidate() {
			if(classToLoad == "FenceVoxelizeForcer" && transform.Find("VisBlocker") == null) {
				Debug.LogError("Need child named 'VisBlocker'!");
			}
		}
#endif
		private IEnumerator WaitAndLoad() {
			yield return new WaitForEndOfFrame();
#if DLL_EXPORT
			if(classToLoad == "FenceVoxelizeForcer")
				gameObject.AddComponent<ParkitectMod.FenceVoxelizeForcer>();
#endif
		}
	}
}