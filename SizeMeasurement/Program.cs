using Google.Protobuf;
using MessagePack;
using Newtonsoft.Json;
using ProtoBuf;
using SerializersBenchmark;
using SerializersBenchmark.Models;
using System.Text;
using STJ = System.Text.Json.JsonSerializer;
using GooglePayloadList = SerializersBenchmark.ProtobufModels.TestPayloadList;

var counts = new[] { 10, 100, 1000 };

Console.WriteLine("Serializer Size Comparison");
Console.WriteLine("==========================");
Console.WriteLine();

var lz4Options = MessagePackSerializerOptions.Standard
    .WithCompression(MessagePackCompression.Lz4Block);

var results = new List<(int Count, string Serializer, long Bytes)>();

foreach (var count in counts)
{
    var payloads = PayloadFactory.CreatePayloads(count);
    var protoList = PayloadFactory.CreateProtoList(count);

    var msgpackSize = MessagePackSerializer.Serialize(payloads).Length;
    var msgpackLz4Size = MessagePackSerializer.Serialize(payloads, lz4Options).Length;
    var stjSize = STJ.SerializeToUtf8Bytes(payloads).Length;
    var newtonsoftSize = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payloads)).Length;

    long protobufNetSize;
    using (var ms = new MemoryStream())
    {
        Serializer.Serialize(ms, payloads);
        protobufNetSize = ms.Length;
    }

    var googleProtobufSize = protoList.ToByteArray().Length;

    results.Add((count, "MessagePack", msgpackSize));
    results.Add((count, "MessagePack LZ4", msgpackLz4Size));
    results.Add((count, "System.Text.Json", stjSize));
    results.Add((count, "Newtonsoft.Json", newtonsoftSize));
    results.Add((count, "protobuf-net", protobufNetSize));
    results.Add((count, "Google.Protobuf", googleProtobufSize));
}

Console.WriteLine($"{"Serializer",-20} {"Count",8} {"Size (bytes)",14} {"Size (KB)",12}");
Console.WriteLine(new string('-', 56));

foreach (var count in counts)
{
    var group = results.Where(r => r.Count == count).ToList();
    foreach (var (_, serializer, bytes) in group)
    {
        Console.WriteLine($"{serializer,-20} {count,8} {bytes,14:N0} {bytes / 1024.0,12:F2}");
    }

    Console.WriteLine(new string('-', 56));
}
