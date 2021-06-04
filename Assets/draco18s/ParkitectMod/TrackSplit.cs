using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.draco18s.ParkitectMod {
#if DLL_EXPORT
	public class TrackSplit : SBend {
		public override int getMaximumSize() {
			return base.getMinimumSize();
		}

		public override bool canBeBanked() {
			return false;
		}

		public override Vector3 getEndpoint() {
			return base.getEndpoint();
		}

		/*private void OnMouseDown() {

		}

		public override bool onOpenInfoWindow() {
			return base.onOpenInfoWindow();
		}

		//this one!?
		public override void onContextClick(bool isInBuildingTool) {
			base.onContextClick(isInBuildingTool);
		}*/
	}
#endif
}
