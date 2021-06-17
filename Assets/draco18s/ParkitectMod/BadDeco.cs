using System;
using UnityEngine;

namespace Assets.draco18s {
	public class BadDeco :
#if DLL_EXPORT
	SerializedMonoBehaviour, IVisibilityGridEntry

#else
		MonoBehaviour
#endif
		{
#if DLL_EXPORT
		public LogicTransform logicTransform { get; private set; } = new LogicTransform();
		private BuildableObject parentObject;
#else
#endif
		public string objectName;
		public float overrideRating;
#if DLL_EXPORT

		public override void Initialize() {
			dontSerialize = true;
			base.Initialize();
			if(string.IsNullOrEmpty(objectName)) {
				objectName = "Electrical Connections";
				Debug.Log("You forgot to name me, dummy!");
			}
			parentObject = transform.parent.parent.GetComponent<BuildableObject>();
			gameObject.layer = parentObject.gameObject.layer;
			gameObject.tag = parentObject.gameObject.tag;
			
			if(ScriptableSingleton<AssetManager>.Instance.getPrefab(getReferenceName()) != null) {
				ScriptableSingleton<AssetManager>.Instance.registerObject(this);
			}
			GameController.Instance.park.visibilityGrid.add(this);
			transform.localScale = new Vector3(1 / transform.parent.localScale.x, 1 / transform.parent.localScale.y, 1 / transform.parent.localScale.z);
			Vector3 vector = transform.localScale;
			logicTransform.update(transform, vector);
		}

		public override string getReferenceName() {
			return objectName;
		}

		protected override void onKill() {
			base.onKill();
			GameController.Instance.park.visibilityGrid.remove(this);
			//ScriptableSingleton<AssetManager>.Instance.unregisterObject(this);
		}

		/*protected virtual void Update() {
			transform.localScale = new Vector3(1 / transform.parent.localScale.x, 1 / transform.parent.localScale.y, 1 / transform.parent.localScale.z);
			Vector3 vector = transform.localScale;
			logicTransform.update(transform, vector);
		}*/

		public VoxelizeInstruction getVoxelizeInstruction() {
			return new VoxelizeInstruction {
				flags = VisibilityGridObjectTypeFlag.OPAQUE,
				localToWorldMatrix = logicTransform.localToWorldMatrix,
				obj = getBuildableObject()
			};
		}

		public VisualScoreInfo getVisualScore() {
			return new VisualScoreInfo(overrideRating, objectName, 10f);
		}

		public uint getVisibilityGridOctreeID() {
			return getBuildableObject().objectID;
		}

		public BuildableObject getBuildableObject() {
			return parentObject;
		}

		public VisibilityGridEntryData visibilityGridData { get; set; }
#endif

	}
}
