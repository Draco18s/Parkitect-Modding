using System;
using UnityEngine;

namespace TrainJetsMod {
	[Serializable]
	public struct DecoLink
#if DLL_EXPORT
	{
		[Serialized]
		[SerializeField]
		public string attachedCarID;
		[Serialized]
		[SerializeField]
		public string buildableID;
		
		public Vector3 localpos {
			get {
				return new Vector3(px, py, pz);
			}
			set {
				px = value.x;
				py = value.y;
				pz = value.z;
			}
		}
		
		public Quaternion localrot {
			get {
				return new Quaternion(rw, rx, ry, rz);
			}
			set {
				rw = value.w;
				rx = value.x;
				ry = value.y;
				rz = value.z;
			}
		}
		[Serialized]
		[SerializeField]
		public float px, py, pz;
		[Serialized]
		[SerializeField]
		public float rw, rx, ry, rz;

		public override bool Equals(object obj) {
			if(obj is DecoLink other) {
				return buildableID == other.buildableID && attachedCarID == other.attachedCarID;
			}
			return base.Equals(obj);
		}

		public override int GetHashCode() {
			return (buildableID + ":" + attachedCarID).GetHashCode();
		}

		public static DecoLink Parse(string json) {
			return JsonUtility.FromJson<DecoLink>(json);
		}

		public override string ToString() {
			return JsonUtility.ToJson(this);
		}
	}
#else
	{ }
#endif
}
