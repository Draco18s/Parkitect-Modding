﻿#if DLL_EXPORT
using MiniJSON;
using System;
#endif
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace TrainJetsMod {
	public class TrainJetsMod
#if DLL_EXPORT
		: AbstractMod
	{
		public static string NAME = "Train Jets";
		public static string DESCRIPT = "Allows jet deco objects to be placed on ride cars.\nCode by Draco18s";
		public static string VERSION = "1.0.0";
		private List<DecoLink> linksDict;
		public static TrainJetsMod instance;

		public string path {
			get {
				return Application.persistentDataPath + "/Parkitect/Mods/TrainJetsData";
			}
		}

		public TrainJetsMod() {
			try {
				Directory.CreateDirectory(path);
			}
			catch {
				Debug.LogError("Creating path failed: " + path);
				return;
			}
			linksDict = new List<DecoLink>();
			instance = this;
		}

		public override string getName() {
			return NAME;
		}

		public override string getDescription() {
			return DESCRIPT;
		}

		public override string getVersionNumber() {
			return VERSION;
		}

		public override string getIdentifier() {
			return NAME + "-" + VERSION;
		}
		public override void onEnabled() {
			base.onEnabled();
			EventManager.Instance.OnBuildableObjectBuilt += BuildTrigger;
			EventManager.Instance.OnStartPlayingPark += LoadData;
			EventManager.Instance.OnGameSaved += SaveData;
			EventManager.Instance.OnWeekChanged += WeekChanged;
			Deserializer.Instance.addAfterDeserializationHandler(delegate
			{
				Debug.Log("After deserialize, we link stuff" + linksDict.Count);
			});
		}

		private void WeekChanged(int newWeek) {
			foreach(DecoLink link in linksDict) {
				Debug.Log("Relinking " + link.buildableID);
				DoLinkFrom(link);
			}
		}

		public override void onDisabled() {
			base.onDisabled();
			EventManager.Instance.OnBuildableObjectBuilt -= BuildTrigger;
			EventManager.Instance.OnStartPlayingPark -= LoadData;
			EventManager.Instance.OnGameSaved -= SaveData;
			EventManager.Instance.OnWeekChanged -= WeekChanged;
		}

		private void BuildTrigger(BuildableObject buildableObject) {
			if(buildableObject.getReferenceName().ToLower().Contains("nozzle")) {
				if(buildableObject.transform.parent != null) return;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				BuilderMousePositionInfo builderMousePositionInfo = default(BuilderMousePositionInfo);
				builderMousePositionInfo.hitDistance = float.MaxValue;
				GameObject nozzleMeshObj = null;
				foreach(MouseCollider.HitInfo hitInfo in MouseCollisions.Instance.raycastAll(ray, builderMousePositionInfo.hitDistance)) {
					if(hitInfo.hitDistance < builderMousePositionInfo.hitDistance) {
						IMouseSelectable componentInParent = hitInfo.hitObject.GetComponentInParent<IMouseSelectable>();
						if((componentInParent == null || componentInParent.canBeSelected()) && hitInfo.hitObject != buildableObject.gameObject) {
							int num = 1 << hitInfo.hitObject.layer;
							builderMousePositionInfo.hitSomething = true;
							builderMousePositionInfo.hitObject = hitInfo.hitObject;
							builderMousePositionInfo.hitPosition = hitInfo.hitPosition;
							builderMousePositionInfo.hitDistance = hitInfo.hitDistance;
							builderMousePositionInfo.hitNormal = hitInfo.hitNormal;
							builderMousePositionInfo.hitLayerMask = num;
						}
						if((componentInParent == null || componentInParent.canBeSelected()) && hitInfo.hitObject == buildableObject.gameObject) {
							nozzleMeshObj = hitInfo.hitObject;
						}
					}
				}
				if(builderMousePositionInfo.hitSomething) {
					Car car = builderMousePositionInfo.hitObject.GetComponent<Car>();
					if(car != null) {
						Link(buildableObject, car);
					}
				}
			}
		}

		private void Link(BuildableObject buildableObject, Car car) {
			buildableObject.gameObject.transform.SetParent(car.transform, true);
			ChunkedMesh[] componentsInChildren = buildableObject.GetComponentsInChildren<ChunkedMesh>();
			foreach(ChunkedMesh cm in componentsInChildren) {
				GameObject.Destroy(cm);
			}
			ParticleSystem[] systems = buildableObject.GetComponentsInChildren<ParticleSystem>();
			foreach(ParticleSystem sys in systems) {
				ParticleSystem.MainModule m = sys.main;
				m.simulationSpace = ParticleSystemSimulationSpace.World;
			}
			//linksDict.Add(car.getId(),buildableObject.getId());
			DecoLink link = new DecoLink {
				buildableID = buildableObject.getId(),
				attachedCarID = car.getId(),
				localpos = buildableObject.transform.localPosition,
				localrot = buildableObject.transform.localRotation
			};
			linksDict.Add(link);
			if(CommandController.Instance.isInMultiplayerMode()) {
				CommandController.Instance.addCommand(new DecoLinkCommand(link), null, true);
			}
		}

		private void SaveData() {
			string parkName = GameController.Instance.park.parkName;
			string saveDir = System.IO.Path.Combine(path, parkName);
			try {
				Directory.CreateDirectory(saveDir);
			}
			catch {
				Debug.LogError("Creating path failed: " + saveDir);
				return;
			}
			try {
				File.WriteAllText(saveDir + "/links.json", Json.Serialize(linksDict));
			}
			catch {
				Debug.LogError("Writing failed: " + saveDir + "/links.json");
				return;
			}
		}

		private void LoadData() {
			string parkName = GameController.Instance.park.parkName;
			string saveDir = System.IO.Path.Combine(path, parkName);
			if(File.Exists(saveDir + "/links.json")) {
				object q = Json.Deserialize(File.ReadAllText(saveDir + "/links.json"));
				Debug.Log(q is List<object>);
				List<object> l = (List<object>)q;
				foreach(object o in l) {
					if(o is string) {
						object j = Json.Deserialize((string)o);
						Debug.Log(j is Dictionary<string, object>);
						if(j is Dictionary<string, object> d) {
							double px = (double)d["px"];
							double py = (double)d["py"];
							double pz = (double)d["pz"];
							double rw = (double)d["rw"];
							double rx = (double)d["rx"];
							double ry = (double)d["ry"];
							double rz = (double)d["rz"];
							Debug.Log("Adding saved link");
							linksDict.Add(new DecoLink {
								attachedCarID = (string)d["attachedCarID"],
								buildableID = (string)d["buildableID"],
								localpos = new Vector3((float)px, (float)py, (float)pz),
								localrot = new Quaternion((float)rw, (float)rx, (float)ry, (float)rz)
							});
						}
					}
				}
			}
			else {
				Debug.Log(saveDir + "/links.json not found");
			}
		}

		public static void DoLinkFrom(DecoLink link) {
			BuildableObject buildableObject = Deserializer.Instance.resolveReference<BuildableObject>(link.buildableID);
			Car car = Deserializer.Instance.resolveReference<Car>(link.attachedCarID);
			if(!(car && buildableObject)) {
				Debug.Log("Invalid " + link.buildableID + " or " + link.attachedCarID);
				return;
			}
			else {
				Debug.Log("Relinking " + link.buildableID + " to " + link.attachedCarID);
			}
			buildableObject.transform.SetParent(car.transform);
			buildableObject.transform.localPosition = link.localpos;
			buildableObject.transform.localRotation = link.localrot;
			ChunkedMesh[] componentsInChildren = buildableObject.GetComponentsInChildren<ChunkedMesh>();
			foreach(ChunkedMesh cm in componentsInChildren) {
				GameObject.Destroy(cm);
			}
			ParticleSystem[] systems = buildableObject.GetComponentsInChildren<ParticleSystem>();
			foreach(ParticleSystem sys in systems) {
				ParticleSystem.MainModule m = sys.main;
				m.simulationSpace = ParticleSystemSimulationSpace.World;
			}
			if(!instance.linksDict.Contains(link)) {
				instance.linksDict.Add(link);
			}
		}
	}
#else
	{ }
#endif
}
