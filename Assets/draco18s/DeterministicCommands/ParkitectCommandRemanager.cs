using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace Assets.draco18s.DeterministicCommands {
	public class ParkitectCommandRemanager
#if DLL_EXPORT
		: AbstractMod
	{
		public const string NAME = "Deterministic Commands";
		public const string VERSION = "1.0.0";

		public static ParkitectCommandRemanager instance;

		private Harmony harmony;

		public ParkitectCommandRemanager():base() {
			instance = this;
		}

		public override string getName() {
			return NAME;
		}

		public override string getDescription() {
			return "Overrides Parkitect's command registration and network serialization protocols to be more robust.";
		}

		public override string getVersionNumber() {
			return VERSION;
		}

		public override string getIdentifier() {
			return NAME + "-" + VERSION;
		}

		public override void onEnabled() {
			base.onEnabled();
			if(harmony == null) {
				harmony = new Harmony(getIdentifier());
				MethodInfo mOriginal = typeof(CommandSerializerProtoBuf).GetMethod("deserialize", BindingFlags.Instance | BindingFlags.Public);
				MethodInfo mPrefix = typeof(CommandSerializerProtoBufPatcher).GetMethod("deserializePrefix", BindingFlags.Static | BindingFlags.Public);
				
				harmony.Patch(mOriginal, new HarmonyMethod(mPrefix));
			}
		}

		public override void onDisabled() {
			base.onDisabled();
			harmony.UnpatchAll(getIdentifier());
			harmony = null;
		}
	}
#else
	{ }
#endif
}