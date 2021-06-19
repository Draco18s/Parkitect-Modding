#if DLL_EXPORT
using HarmonyLib;
#endif
using System.Reflection;
using UnityEngine;

namespace Assets.draco18s.DeterministicCommands {
	public class DeterministicCommands
#if DLL_EXPORT
		: AbstractMod
	{
		public const string NAME = "Deterministic Commands";
		public const string VERSION = "1.0.0";

		public static DeterministicCommands instance;

		private Harmony harmony;

		public DeterministicCommands() {
			Debug.Log("Loading " + NAME);
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

		public override bool isMultiplayerModeCompatible() {
			return true;
		}

		public override void onEnabled() {
			base.onEnabled();
			Debug.Log("Patching command serializer!");
			if(harmony == null) {
				harmony = new Harmony(getIdentifier());
				MethodInfo deserOrig = typeof(CommandSerializerProtoBuf).GetMethod("deserialize", BindingFlags.Instance | BindingFlags.Public);
				MethodInfo deserPrefix = typeof(CommandSerializerProtoBufPatcher).GetMethod("deserializePrefix", BindingFlags.Static | BindingFlags.Public);
				MethodInfo serOrig = typeof(CommandSerializerProtoBuf).GetMethod("serialize", BindingFlags.Instance | BindingFlags.Public);
				MethodInfo serPrefix = typeof(CommandSerializerProtoBufPatcher).GetMethod("serializePrefix", BindingFlags.Static | BindingFlags.Public);
				MethodInfo addToOrig = typeof(CommandSerializerProtoBuf).GetMethod("addTypeToModel", BindingFlags.Instance | BindingFlags.NonPublic);
				MethodInfo addToPrefix = typeof(CommandSerializerProtoBufPatcher).GetMethod("addTypeToModelPrefix", BindingFlags.Static | BindingFlags.Public);
				
				harmony.Patch(deserOrig, new HarmonyMethod(deserPrefix));
				harmony.Patch(serOrig, new HarmonyMethod(serPrefix));
				harmony.Patch(addToOrig, new HarmonyMethod(addToPrefix));
			}
		}

		public override void onDisabled() {
			base.onDisabled();
			Debug.Log("Unpatching command serializer!");
			harmony.UnpatchAll(getIdentifier());
			harmony = null;
		}
	}
#else
	{ }
#endif
}