﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace telegram_bot
{
    static class Serializer
    {
        static JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects };
        
        /// <summary>
        /// Writes the given object instance to a binary file.
        /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the XML file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the XML file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            if (append) throw new NotImplementedException("Didn't need append when I implemented this");
            Directory.GetParent(filePath).Create();
            File.WriteAllText(filePath, JsonConvert.SerializeObject(objectToWrite, jsonSettings));
        }

        /// <summary>
        /// Reads an object instance from a binary file.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the XML.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the binary file.</returns>
        public static T ReadFromBinaryFile<T>(string filePath)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath), jsonSettings);
        }
    }
}
