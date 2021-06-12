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
			UnityEngine.Debug.Log("Instanced: " + person.currentBehaviour);
			if(person.currentBehaviour != null) return;
			base.onInstantiate(person);
			
			time = 0;
			RemoveBehaviour(original);
		}

		/*protected override Node setupTree() {
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

		private void onQueueBlocked() {
			//this.queueIsBlocked = true;
			//this.wokeUpNextGuest = false;
		}
		
		private void onQueueUnblocked() {
			//if(!this.wokeUpNextGuest) {
			//	this.wakeupNextQueueingGuest();
			//}
			//this.stopIdle();
		}*/

		public override void tick(float deltaTime) {
			base.tick(deltaTime);
			person.Immersion += 0.01f * deltaTime;
			time += deltaTime;
			if(person.currentBlock != thisBlock) {
				stopIdle();
			}
		}

		public new void stopIdle() {
			base.stopIdle();
			foreach(PathAttachment a in ((Queue)thisBlock).getPathAttachments()) {
				if(a is QueueBench) {
					((QueueBench)a).StoodUpFrom(person);
				}
			}
			RemoveBehaviour(this);
			//person.SittingOn.beFreed();
			person.standUp(true);
			person.transform.position = pos;
			person.currentBehaviour = original;
			//original.GetType().GetField("previousDistanceToEntrance", BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.FlattenHierarchy).SetValue(original, -1);
			//original.stopIdle();
			time = 0;
		}

		private void RemoveBehaviour(PersonBehaviour behav) {
			FieldInfo field = typeof(Person).GetField("behaviours", BindingFlags.NonPublic | BindingFlags.Instance);
			List<PersonBehaviour> behavs = (List<PersonBehaviour>)field.GetValue(person);
			behavs.Remove(behav);
		}

		public override string getDescription() {
			return I18N.GetString("Watching {0}'s preshow for {1}", new object[] {
				StationController.getAttraction().getCustomizedColorizedName(),
				TextUtility.formatTime((int)time)
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
