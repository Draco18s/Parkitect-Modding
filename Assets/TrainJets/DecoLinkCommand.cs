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

		public DecoLinkCommand(DecoLink l) {
			theLink = l;
		}

		public override void run() {
			if(!isOwnCommand)
				TrainJetsMod.DoLinkFrom(theLink);
		}

		public override bool isOnlineOnlyCommand() {
			return true;
		}
	}
#else
	{ }
#endif
}
