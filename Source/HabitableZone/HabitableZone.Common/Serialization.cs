using System;
using System.IO;
using System.Reflection;
using Pathfinding.Serialization.JsonFx;

namespace HabitableZone.Common
{
	public class Serialization
	{
		/// <summary>
		///    Deserializes some type from json using project's json settings.
		/// </summary>
		public static TData DeserializeDataFromJson<TData>(Stream stream)
		{
			var jsonReaderSettings = GetJsonReaderSettings(typeof(TData).Assembly);
			var jsonReader = new JsonReader(stream, jsonReaderSettings);
			var data = (TData) jsonReader.Deserialize(typeof(TData));

			return data;
		}

		/// <summary>
		///    Serializes some type to json using project's json settings.
		/// </summary>
		public static void SerializeDataToJson(Object data, Stream stream)
		{
			var jsonWriterSettings = GetJsonWriterSettings(data.GetType().Assembly);
			var jsonWriter = new JsonWriter(stream, jsonWriterSettings);

			jsonWriter.Write(data);
			jsonWriter.TextWriter.Flush();
		}

		/// <summary>
		///    Returns JsonWriterSettings used across the project for serialization.
		/// </summary>
		private static JsonWriterSettings GetJsonWriterSettings(Assembly assembly)
		{
			return new JsonWriterSettings
			{
				PrettyPrint = true,
				TypeHintName = "__type",
				TypeHintsOnlyWhenNeeded = true,
				DefaultAssembly = assembly
			};
		}

		/// <summary>
		///    Returns JsonReaderSettings used across the project for deserialization.
		/// </summary>
		private static JsonReaderSettings GetJsonReaderSettings(Assembly assembly)
		{
			return new JsonReaderSettings
			{
				TypeHintName = "__type",
				DefaultAssembly = assembly
			};
		}
	}
}