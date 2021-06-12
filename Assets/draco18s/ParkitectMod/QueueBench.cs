using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.draco18s.ParkitectMod {
	public class QueueBench :
#if DLL_EXPORT
	Seating
#else
		MonoBehaviour
#endif
		{
#if DLL_EXPORT
		protected Queue queue;
		[Serialized]
		[DontSerializeIn(SerializationContext.Context.Blueprint)]
		[DontInstantiateNewWhenDeserializing]
		protected Seats<Seat> seats = new Seats<Seat>();

		public QueueBench() : base() {
			canBuildOnQueues = true;
		}

		protected override void Awake() {
			base.Awake();
			seats.initialize(transform);
		}

		protected virtual void Update() {
			if(isPreview) return;
			StationController station = queue.getStationController();
			foreach(Seat seat in seats) {
				if(!seat.isOccupied) continue;
				if(seat.occupiedByPerson.currentBehaviour is PrerideShowBehaviour) continue;
				seat.occupiedByPerson.instantlyChangeBehaviour<RoamingBehaviour>();
				seat.beFreed();
			}
			foreach(Person p in station.queueingPeople) {
				if(p.currentBlock != path) continue;
				if(recent.ContainsKey(p)) {
					if(recent[p] + 10 < Time.time)
						continue;
					recent.Remove(p);
				}
				if(p.currentBehaviour is QueueingBehaviour qb && !(qb is PrerideShowBehaviour)) {
					if(getFreeSeat() == null) return;
					if(!qb.isIdle) continue;
					p.currentBehaviour = null;
					PrerideShowBehaviour prsb = p.getBehaviour<PrerideShowBehaviour>();
					prsb.thisBlock = queue;
					prsb.StationController = station;
					prsb.original = qb;
					prsb.pos = p.transform.position;
					p.changeBehaviour(prsb, false);
					p.sitDownOn(getFreeSeat());
				}
			}
		}

		private Dictionary<Person,int> recent = new Dictionary<Person, int>();

		public void StoodUpFrom(Person p) {
			recent.Add(p, Mathf.CeilToInt(Time.time));
		}

		public override bool isVandalizable() {
			return false;
		}

		public override void onPositionedOnPath() {
			if(!(path is Queue)) Kill();
			else
				base.onPositionedOnPath();
			queue = (Queue)path;
			queue.OnPeopleOnBlockChanged.mainThreadEvent += PeopleChanged;
		}

		private void PeopleChanged(Block block) {

		}

		public override CanBuild canBuild() {
			if(path is Queue) return base.canBuild();
			Block b = GameController.Instance.park.blockData.getBlock(transform.position);
			return new CanBuild {
				result = b is Queue,
				message = "Must be on a queue"
			};
		}
#else
		public bool hasBackRest;
		public string categoryTag = "Path Attachments/Test";
#endif
	}
}
