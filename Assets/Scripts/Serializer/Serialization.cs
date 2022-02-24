using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using FullSerializer;

public static class Serialization
{
    //public static void Serialize(object value, string filePath)
    //{
    //    byte[] bytes = SerializationUtility.SerializeValueWeak(value, DataFormat.Binary);

    //    File.WriteAllBytes(filePath, bytes);
    //}

    //public static object Deserialize(string filePath)
    //{
    //    if (!File.Exists(filePath)) return null;

    //    byte[] bytes = File.ReadAllBytes(filePath);
    //    return SerializationUtility.DeserializeValue<object>(bytes, DataFormat.Binary);
    //}

    private static readonly fsSerializer _serializer = new fsSerializer();

    public static string Serialize(Type type, object value)
    {
        // serialize the data
        fsData data;
        _serializer.TrySerialize(type, value, out data).AssertSuccess();

        // emit the data via JSON
        return fsJsonPrinter.CompressedJson(data);
    }

    public static object Deserialize(Type type, string serializedState)
    {
        // step 1: parse the JSON data
        fsData data = fsJsonParser.Parse(serializedState);

        // step 2: deserialize the data
        object deserialized = null;
        _serializer.TryDeserialize(data, type, ref deserialized).AssertSuccess();

        return deserialized;
    }
}
