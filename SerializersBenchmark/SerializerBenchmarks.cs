using BenchmarkDotNet.Attributes;
using Google.Protobuf;
using MessagePack;
using ProtoBuf;
using SerializersBenchmark.Models;
using System.Text;
using STJ = System.Text.Json.JsonSerializer;
using GooglePayload = SerializersBenchmark.ProtobufModels.TestPayload;
using GoogleAddress = SerializersBenchmark.ProtobufModels.Address;

namespace SerializersBenchmark;

[MemoryDiagnoser]
[SimpleJob]
public class SerializerBenchmarks
{
    private TestPayload _payload = null!;
    private GooglePayload _protoPayload = null!;

    private byte[] _msgpackBytes = null!;
    private byte[] _stjBytes = null!;
    private byte[] _newtonsoftBytes = null!;
    private byte[] _protobufNetBytes = null!;
    private byte[] _googleProtobufBytes = null!;

    [GlobalSetup]
    public void Setup()
    {
        _payload = new TestPayload
        {
            Id = 42,
            Name = "Benchmark Test Payload",
            IsActive = true,
            Score = 99.7,
            CreatedAt = new DateTime(2024, 6, 15, 14, 30, 0, DateTimeKind.Utc),
            Description = "A comprehensive test payload for benchmarking serialization libraries",
            Age = 30,
            Rating = 4.5,
            Tags = ["benchmark", "serialization", "performance", "dotnet", "test"],
            Categories = ["testing", "benchmarks", "evaluation"],
            Address = new Address
            {
                Street = "123 Main Street",
                City = "Springfield",
                ZipCode = "62704"
            }
        };

        _protoPayload = new GooglePayload
        {
            Id = 42,
            Name = "Benchmark Test Payload",
            IsActive = true,
            Score = 99.7,
            CreatedAt = "2024-06-15T14:30:00Z",
            Description = "A comprehensive test payload for benchmarking serialization libraries",
            Age = 30,
            Rating = 4.5,
            Tags = { "benchmark", "serialization", "performance", "dotnet", "test" },
            Categories = { "testing", "benchmarks", "evaluation" },
            Address = new GoogleAddress
            {
                Street = "123 Main Street",
                City = "Springfield",
                ZipCode = "62704"
            }
        };

        _msgpackBytes = MessagePackSerializer.Serialize(_payload);
        _stjBytes = STJ.SerializeToUtf8Bytes(_payload);
        _newtonsoftBytes = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_payload));

        using (var ms = new MemoryStream())
        {
            Serializer.Serialize(ms, _payload);
            _protobufNetBytes = ms.ToArray();
        }

        _googleProtobufBytes = _protoPayload.ToByteArray();
    }

    // MessagePack

    [Benchmark(Baseline = true)]
    public byte[] Serialize_MessagePack()
        => MessagePackSerializer.Serialize(_payload);

    [Benchmark]
    public TestPayload Deserialize_MessagePack()
        => MessagePackSerializer.Deserialize<TestPayload>(_msgpackBytes)!;

    // System.Text.Json

    [Benchmark]
    public byte[] Serialize_SystemTextJson()
        => STJ.SerializeToUtf8Bytes(_payload);

    [Benchmark]
    public TestPayload Deserialize_SystemTextJson()
        => STJ.Deserialize<TestPayload>(_stjBytes)!;

    // Newtonsoft.Json

    [Benchmark]
    public byte[] Serialize_NewtonsoftJson()
        => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_payload));

    [Benchmark]
    public TestPayload Deserialize_NewtonsoftJson()
        => Newtonsoft.Json.JsonConvert.DeserializeObject<TestPayload>(Encoding.UTF8.GetString(_newtonsoftBytes))!;

    // protobuf-net

    [Benchmark]
    public byte[] Serialize_ProtoBufNet()
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, _payload);
        return ms.ToArray();
    }

    [Benchmark]
    public TestPayload Deserialize_ProtoBufNet()
    {
        using var ms = new MemoryStream(_protobufNetBytes);
        return Serializer.Deserialize<TestPayload>(ms)!;
    }

    // Google.Protobuf

    [Benchmark]
    public byte[] Serialize_GoogleProtobuf()
        => _protoPayload.ToByteArray();

    [Benchmark]
    public GooglePayload Deserialize_GoogleProtobuf()
        => GooglePayload.Parser.ParseFrom(_googleProtobufBytes);
}
