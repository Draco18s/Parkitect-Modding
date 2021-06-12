using System;
using System.Text;
using UnityEngine;

namespace TrainJetsMod {
	[Serializable]
	public struct DecoLink
#if DLL_EXPORT
	{
		[Serialized]
		public string attachedCarID;
		[Serialized]
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
		public float px, py, pz;
		[Serialized]
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

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			sb.Append("{").Append("\"attachedCarID\":\"").Append(attachedCarID).Append("\",")
				.Append("\"buildableID\":\"").Append(buildableID).Append("\",")
				.Append("\"px\":").Append(px)
				.Append(",\"py\":").Append(py)
				.Append(",\"pz\":").Append(pz)
				.Append(",\"rw\":").Append(rw)
				.Append(",\"rx\":").Append(rx)
				.Append(",\"ry\":").Append(ry)
				.Append(",\"rz\":").Append(rz)
				.Append("}");
			return sb.ToString();
		}
	}
#else
	{ }
#endif
}
