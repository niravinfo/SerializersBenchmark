using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Google.Protobuf;
using MessagePack;
using Newtonsoft.Json;
using ProtoBuf;
using SerializersBenchmark.Models;
using System.Text;
using STJ = System.Text.Json.JsonSerializer;
using GooglePayload = SerializersBenchmark.ProtobufModels.TestPayload;
using GoogleAddress = SerializersBenchmark.ProtobufModels.Address;
using GooglePayloadList = SerializersBenchmark.ProtobufModels.TestPayloadList;

namespace SerializersBenchmark;

[MemoryDiagnoser]
[ShortRunJob]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory, BenchmarkLogicalGroupRule.ByParams)]
public class SerializerBenchmarks
{
    [Params(10, 100, 1000)]
    public int Count { get; set; }

    private TestPayload[] _payloads = null!;
    private GooglePayloadList _protoList = null!;

    private byte[] _msgpackBytes = null!;
    private byte[] _msgpackLz4Bytes = null!;
    private byte[] _stjBytes = null!;
    private byte[] _newtonsoftBytes = null!;
    private byte[] _protobufNetBytes = null!;
    private byte[] _googleProtobufBytes = null!;

    private static MessagePackSerializerOptions _lz4Options = MessagePackSerializerOptions.Standard
            .WithCompression(MessagePackCompression.Lz4Block);

    private static MessagePackSerializerOptions _standardOptions = MessagePackSerializerOptions.Standard;

    [GlobalSetup]
    public void Setup()
    {
        _payloads = new TestPayload[Count];
        for (int i = 0; i < Count; i++)
        {
            _payloads[i] = CreatePayload(i);
        }

        _protoList = new GooglePayloadList();
        for (int i = 0; i < Count; i++)
        {
            _protoList.Items.Add(CreateProtoPayload(i));
        }

        _msgpackBytes = MessagePackSerializer.Serialize(_payloads, _standardOptions);
        _msgpackLz4Bytes = MessagePackSerializer.Serialize(_payloads, _lz4Options);

        _stjBytes = STJ.SerializeToUtf8Bytes(_payloads);
        _newtonsoftBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_payloads));

        using (MemoryStream ms = new MemoryStream())
        {
            Serializer.Serialize(ms, _payloads);
            _protobufNetBytes = ms.ToArray();
        }

        _googleProtobufBytes = _protoList.ToByteArray();
    }

    // Serialize

    [BenchmarkCategory("Serialize")]
    [Benchmark(Baseline = true)]
    public byte[] Serialize_MessagePack()
        => MessagePackSerializer.Serialize(_payloads, _standardOptions);

    [BenchmarkCategory("Serialize")]
    [Benchmark]
    public byte[] Serialize_MessagePack_LZ4()
    {
        return MessagePackSerializer.Serialize(_payloads, _lz4Options);
    }

    [BenchmarkCategory("Serialize")]
    [Benchmark]
    public byte[] Serialize_SystemTextJson()
        => STJ.SerializeToUtf8Bytes(_payloads);

    [BenchmarkCategory("Serialize")]
    [Benchmark]
    public byte[] Serialize_NewtonsoftJson()
        => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_payloads));

    [BenchmarkCategory("Serialize")]
    [Benchmark]
    public byte[] Serialize_ProtoBufNet()
    {
        using MemoryStream ms = new MemoryStream();
        Serializer.Serialize(ms, _payloads);
        return ms.ToArray();
    }

    [BenchmarkCategory("Serialize")]
    [Benchmark]
    public byte[] Serialize_GoogleProtobuf()
        => _protoList.ToByteArray();

    // ── Deserialize ───────────────────────────────────────────

    [BenchmarkCategory("Deserialize")]
    [Benchmark(Baseline = true)]
    public TestPayload[] Deserialize_MessagePack()
        => MessagePackSerializer.Deserialize<TestPayload[]>(_msgpackBytes)!;

    [BenchmarkCategory("Deserialize")]
    [Benchmark]
    public TestPayload[] Deserialize_MessagePack_LZ4()
        => MessagePackSerializer.Deserialize<TestPayload[]>(_msgpackLz4Bytes, _lz4Options)!;

    [BenchmarkCategory("Deserialize")]
    [Benchmark]
    public TestPayload[] Deserialize_SystemTextJson()
        => STJ.Deserialize<TestPayload[]>(_stjBytes)!;

    [BenchmarkCategory("Deserialize")]
    [Benchmark]
    public TestPayload[] Deserialize_NewtonsoftJson()
        => JsonConvert.DeserializeObject<TestPayload[]>(Encoding.UTF8.GetString(_newtonsoftBytes))!;

    [BenchmarkCategory("Deserialize")]
    [Benchmark]
    public TestPayload[] Deserialize_ProtoBufNet()
    {
        using var ms = new MemoryStream(_protobufNetBytes);
        return Serializer.Deserialize<TestPayload[]>(ms)!;
    }

    [BenchmarkCategory("Deserialize")]
    [Benchmark]
    public GooglePayloadList Deserialize_GoogleProtobuf()
        => GooglePayloadList.Parser.ParseFrom(_googleProtobufBytes);

    private static TestPayload CreatePayload(int index) => new()
    {
        Id = index,
        Name = $"Item {index}",
        IsActive = index % 2 == 0,
        Score = index * 1.1,
        CreatedAt = new DateTime(2024, 1, 1).AddDays(index),
        Description = $"Description for item {index}",
        Age = 20 + (index % 50),
        Rating = 1.0 + (index % 5),
        Tags = ["tag1", "tag2", "tag3"],
        Categories = ["cat1", "cat2"],
        Address = new Address
        {
            Street = $"{index} Main Street",
            City = "Springfield",
            ZipCode = "62704"
        }
    };

    private static GooglePayload CreateProtoPayload(int index) => new()
    {
        Id = index,
        Name = $"Item {index}",
        IsActive = index % 2 == 0,
        Score = index * 1.1,
        CreatedAt = new DateTime(2024, 1, 1).AddDays(index).ToString("O"),
        Description = $"Description for item {index}",
        Age = 20 + (index % 50),
        Rating = 1.0 + (index % 5),
        Tags = { "tag1", "tag2", "tag3" },
        Categories = { "cat1", "cat2" },
        Address = new GoogleAddress
        {
            Street = $"{index} Main Street",
            City = "Springfield",
            ZipCode = "62704"
        }
    };
}
