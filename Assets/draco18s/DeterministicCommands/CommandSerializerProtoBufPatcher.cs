using Alba.Framework.Collections;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Assets.draco18s.DeterministicCommands {
	public class CommandSerializerProtoBufPatcher {
		private static BiDictionary<Type, string> typeRegistry = new BiDictionary<Type, string>();

		public static bool deserializePrefix(byte[] message, ref AbstractCommand __result, RuntimeTypeModel ___typeModel) {
			AbstractCommand result = null;
			using(MemoryStream memoryStream = new MemoryStream(message)) {
				memoryStream.Seek(0L, SeekOrigin.Begin);
				StringBuilder sb = new StringBuilder();
				int len = memoryStream.ReadByte();
				for(int i = 0; i < len; i++) {
					sb.Append((char)memoryStream.ReadByte());
				}
				string messageID = sb.ToString();
				if(typeRegistry.Reverse.TryGetValue(messageID, out Type t)) {
					result = (AbstractCommand)___typeModel.Deserialize(memoryStream, null, t);
				}
			}
			__result = result;
			return false;
		}

		public bool serializePrefix(AbstractCommand command, ref byte[] __result, RuntimeTypeModel ___typeModel) {
			byte[] result;
			using(MemoryStream memoryStream = new MemoryStream()) {
				if(typeRegistry.TryGetValue(command.GetType(), out string messageID)) {
					byte b = (byte)Math.Min(messageID.Length, 128);
					memoryStream.WriteByte(b);
					for(int i = 0; i < b; i++) {
						memoryStream.WriteByte((byte)messageID[i]);
					}
					___typeModel.Serialize(memoryStream, command);
				}
				result = memoryStream.ToArray();
			}
			__result = result;
			return false;
		}

		public static void addTypeToModelPrefix(Type type) {
			string asmName = type.Assembly.GetName().Name;
			string typeName = type.Name;
			if(asmName.Length + typeName.Length > 128) {
				asmName = asmName.Truncate(48);
				typeName = typeName.Truncate(96);
			}
			typeRegistry.Add(type, asmName + typeName);
		}
	}
}