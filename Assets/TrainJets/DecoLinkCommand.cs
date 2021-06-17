using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TrainJetsMod {
	public class DecoLinkCommand
#if DLL_EXPORT
		: OrderedCommand {
		[SerializeField]
		public DecoLink theLink;

		[SerializeField]
		public string theLinkJson;

		public DecoLinkCommand(DecoLink l) {
			theLink = l;
			theLinkJson = l.ToString();
		}

		public override void run() {
			if(!isOwnCommand) {
				if(!string.IsNullOrEmpty(theLink.attachedCarID)) {
					Debug.Log("Got link object");
					TrainJetsMod.DoLinkFrom(theLink);
				}
				else if(!string.IsNullOrEmpty(theLinkJson)) {
					Debug.Log("Got string");
					DecoLink theLink = JsonUtility.FromJson<DecoLink>(theLinkJson); //TrainJetsMod.JsonToDecoLink(MiniJSON.Json.Deserialize(theLinkJson));
					TrainJetsMod.DoLinkFrom(theLink);
				}
				else {
					Debug.Log("Something is broken");
				}
			}
		}

		public override bool isOnlineOnlyCommand() {
			return false;
		}
	}
#else
	{ }
#endif
}
