#if DLL_EXPORT
using BehaviourTree;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Assets.draco18s.ParkitectMod {
	public class PrerideShowBehaviour
#if DLL_EXPORT
		: QueueingBehaviour {

		[Serialized]
		public Block thisBlock;

		[Serialized]
		public float time;

		[Serialized]
		public QueueingBehaviour original;

		[Serialized]
		public Vector3 pos;

		protected override void Initialize(bool isDeserialized, SerializationContext serializationContext) {
			base.Initialize(isDeserialized, serializationContext);
			time = 0;
		}

		public override void onInstantiate(Person person) {
			if(person.currentBehaviour != null) return;
			base.onInstantiate(person);
			
			time = 0;
			RemoveBehaviour(original);
		}

		protected override Node setupTree() {
			return new Loop(new Node[]
			{
			new Sequence(new Node[]
			{
				new DecideNextWalkToBlockAction("block").setFailIfNoBlockFound(true),
				new TurnBlockIntoWalkToPositionAction("block", "position"),
				new Selector(new Node[]
				{
					new Parallel(Parallel.Fail.IfOneNodeFails, Parallel.Succeed.IfAllNodesSucceed, new Node[]
					{
						new Inverter(new IsQueueBlockedCondition("stationController", this).withSuccessHandler(new Node.OnSuccessHandler(this.onQueueBlocked)).withFailedHandler(new Node.OnFailedHandler(this.onQueueUnblocked))),
						new WalkToPositionAction("position", 0.33333334f, false).setAvoidPeople(false)
					}),
					new IdleAction(this)
				})
			})
			});
		}

		[Serialized]
		[OnlySerializeIn(SerializationContext.Context.Multiplayer)]
		private bool wokeUpNextGuest;

		private void onQueueBlocked() {
			this.wokeUpNextGuest = false;
		}
		
		private void onQueueUnblocked() {
			this.stopIdle();
			if(!this.wokeUpNextGuest) {
				this.wakeupNextQueueingGuest();
			}
		}

		private void wakeupNextQueueingGuest() {
			if(this.StationController == null) {
				return;
			}
			this.wokeUpNextGuest = true;
			int positionInQueue = this.StationController.getPositionInQueue(this.person);
			QueueingBehaviour queueingBehaviour;
			if(positionInQueue + 1 < this.StationController.queueingPeople.Count && (queueingBehaviour = (this.StationController.queueingPeople[positionInQueue + 1].currentBehaviour as QueueingBehaviour)) != null) {
				queueingBehaviour.stopIdle();
			}
		}

		public override void tick(float deltaTime) {
			base.tick(deltaTime);
			person.Immersion += 0.01f * deltaTime;
			time += deltaTime;
			if(doMoveForward) {
				delay -= deltaTime;
				Node.Result result = this.treeRootNode.triggerRun(deltaTime, this.dataContext);
				if(delay <= 0) {
					RemoveBehaviour(this);
					//person.transform.position = pos;
					person.currentBehaviour = original;
					//original.GetType().GetField("previousDistanceToEntrance", BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.FlattenHierarchy).SetValue(original, -1);
					time = 0;
				}
			}
			else if(person.currentBlock != thisBlock) {
				stopIdle();
				delay = 0;
			}
		}

		public bool doMoveForward = false;
		public float delay = 0;

		public new void stopIdle() {
			base.stopIdle();
			/*foreach(PathAttachment a in ((Queue)thisBlock).getPathAttachments()) {
				if(a is QueueBench) {
					((QueueBench)a).StoodUpFrom(person);
				}
			}*/
			doMoveForward = true;
			//person.SittingOn.beFreed();
			original.stopIdle();
			delay = 0.5f;
		}

		private void RemoveBehaviour(PersonBehaviour behav) {
			FieldInfo field = typeof(Person).GetField("behaviours", BindingFlags.NonPublic | BindingFlags.Instance);
			List<PersonBehaviour> behavs = (List<PersonBehaviour>)field.GetValue(person);
			behavs.Remove(behav);
		}

		public override string getDescription() {
			return I18N.GetString("Watching {0}'s preshow for {1} ({2})", new object[] {
				StationController.getAttraction().getCustomizedColorizedName(),
				TextUtility.formatTime((int)time),
				doMoveForward
			});
		}

		public override string getGeneralBehaviourDescription() {
			return I18N.GetString("Watching {0}'s preshow", new object[] {
				this.StationController.getAttraction().getCustomizedColorizedName()
			});
		}
	}
#else
	{ }
#endif
}
