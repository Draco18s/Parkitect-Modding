using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Assets.draco18s.ParkitectMod {
#if DLL_EXPORT
	public abstract class AbstractDarkRideBanner : ImageBanner {
		protected static bool forcePlacement = false;
		protected static MaterialPropertyBlock block = new MaterialPropertyBlock();
		private static Thread mainThread;

		public float height;

		protected bool canEnhanceRide;
		protected bool customSizeBehaviourLoaded;
		protected CustomSize _lazyLoadedCustomSizeBehaviour;
		protected CustomSize lazyLoadedCustomSizeBehaviour {
			get {
				if(!customSizeBehaviourLoaded) {
					_lazyLoadedCustomSizeBehaviour = GetComponent<CustomSize>();
					customSizeBehaviourLoaded = true;
				}
				return _lazyLoadedCustomSizeBehaviour;
			}
		}

		private List<TrackSegment4> nearbyTrackSegments;

		public override void Initialize() {
			base.Initialize();
			mainThread = Thread.CurrentThread;
			if(!isPreview && registerTileListeners)
				gameController.park.visibilityGrid.solidCoverHeightChanged += RainUpdate;
			canEnhanceRide = true;
			nearbyTrackSegments = GetNearbyTrack();
			if(isPreview)
				forcePlacement = false;
		}

		public override void updateLogicTransform() {
			Vector3 vector = Vector3.one;
			CustomSize lazyLoadedCustomSizeBehaviour;
			if((lazyLoadedCustomSizeBehaviour = this.lazyLoadedCustomSizeBehaviour) != null) {
				vector = lazyLoadedCustomSizeBehaviour.axisScale * lazyLoadedCustomSizeBehaviour.getValue();
			}
			logicTransform.update(transform, vector);
		}

		public override string getDescription() {
			return getName() + "\nGains a decoration bonus near slow moving track sections.";
		}

		private void RainUpdate(int tileX, int tileZ) {
			/*if(Mathf.Abs(tileX - getPositionForSerialization().x) > 1 ||
				Mathf.Abs(tileZ - getPositionForSerialization().z) > 1) {
				return;
			}*/
			bool canBuild = GetRainProtection();
			canEnhanceRide = canBuild;
		}

		private bool GetRainProtection() {
			bool canBuild = true;
			int maxX = gameController.park.visibilityGrid.solidCoverHeights.xSize;
			int maxZ = gameController.park.visibilityGrid.solidCoverHeights.ySize;
			foreach(CrossedTileInfo crossedTileInfo in getCrossedTiles().crossedTilesInfo) {
				Vector3 pos = new Vector3(crossedTileInfo.getWorldX(), crossedTileInfo.crossedTiles.offsetY, crossedTileInfo.getWorldZ());
				if(pos.x < 0 || pos.x >= maxX || pos.z < 0 || pos.z >= maxZ) return false;
				canBuild &= gameController.park.hasSolidCover(pos, 15, (height * lazyLoadedCustomSizeBehaviour.getValue() * (lazyLoadedCustomSizeBehaviour.axisScale.y+1)) + 0.1f);
			}
			return canBuild;
		}

		protected override void onKill() {
			gameController.park.visibilityGrid.solidCoverHeightChanged -= RainUpdate;
			base.onKill();
		}

		public override CanBuild canBuild() {
			CanBuild b = new CanBuild();
			bool canBuild = GetRainProtection();
			b.result = canBuild || forcePlacement;
			if(!b.result) {
				b.message = "Must be rain protected.";
			}
			if(!canBuild && !forcePlacement) {
				if(toggleCoroutine == null)
					toggleCoroutine = StartCoroutine(ToggleForcePlacement());
				return b;
			}
			CanBuild b2 = base.canBuild();
			b2.merge(b);
			return b2;
		}

		private Coroutine toggleCoroutine;
		private IEnumerator ToggleForcePlacement() {
			yield return new WaitForSecondsRealtime(0.2f);
			yield return new WaitUntil(() => Input.GetMouseButton(0));
			yield return new WaitForSecondsRealtime(0.3f);
			forcePlacement = true;
			toggleCoroutine = null;
		}

		protected virtual List<TrackSegment4> GetNearbyTrack() {
			if(Thread.CurrentThread != mainThread) return nearbyTrackSegments;
			List<TrackSegment4> segments = new List<TrackSegment4>();
			CrossedTiles ct = getCrossedTilesInstance();
			List<CrossedTileInfo> crossedTileInfoList = ct.crossedTilesInfo;
			if(crossedTileInfoList.Count == 0) return segments;
			int currentBlockX = crossedTileInfoList[0].getWorldX();
			int currentBlockZ = crossedTileInfoList[0].getWorldZ();
			ICollection<TrackSegment4> objectsOutlineIC = gameController.park.trackSegmentRegistry.getObjects(currentBlockX, currentBlockZ, Mathf.RoundToInt(lazyLoadedCustomSizeBehaviour.getValue()));
			TrackSegment4[] objectsOutline = objectsOutlineIC.ToArray();
			Vector3 p = transform.position;
			foreach(TrackSegment4 trackSegment in objectsOutline) {
				if(trackSegment != null && trackSegment.undergroundLength <= 0f && trackSegment.track.TrackedRide != null && (trackSegment.track.TrackedRide.canOpen(out string _) || GameController.Instance.park.lockManager.isLocked(trackSegment.track.TrackedRide, out AbstractNetworkClient.INetworkPlayer networkPlayer)) && trackSegment.statsDataPoints.Count > 0) {
					foreach(Vector3 dp in trackSegment.getTrackerPositions()) {
						if(Mathf.Abs(p.y - (dp.y+.5f)) <= 1f) {
							segments.Add(trackSegment);
							goto outer;
						}
					}
				outer:
					;
				}
			}
			return segments;
		}

		public override VisualScoreInfo getVisualScore() {
			if(isPreview) return base.getVisualScore();
			if(!canEnhanceRide) {
				//utility buildings are -0.75
				return new VisualScoreInfo(-1.5f, getReferenceName(), 10);
			}

			VisualScoreInfo bv = base.getVisualScore();

			if(nearbyTrackSegments == null || nearbyTrackSegments.Count == 0) {
				bv.score *= 0.5f;
				return bv;
			}

			float minSpeed = 3.6f*nearbyTrackSegments.Min(t => t.statsDataPoints[t.statsDataPoints.Count - 1].velocity)+2;
			float maxSpeed = 3.6f*nearbyTrackSegments.Max(t => t.statsDataPoints[t.statsDataPoints.Count - 1].velocity)+2;
			float scoreMulti = minSpeed <= 14 ? Mathf.Clamp((14 - minSpeed) / 10f * 6f, 0.2f, 5.5f) : 0.1f;
			
			bv.score *= scoreMulti;

			return bv;
		}

		public override void serialize(SerializationContext context, Dictionary<string, object> values) {
			base.serialize(context, values);
			values.Add("canEnhanceRide", canEnhanceRide);
		}

		public override void deserialize(SerializationContext context, Dictionary<string, object> values) {
			base.deserialize(context, values);
			if(values.TryGetValue("canEnhanceRide", out object obj)) {
				canEnhanceRide = (bool)obj;
			}
		}
	}
#endif
}
