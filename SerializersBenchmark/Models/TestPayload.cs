using MessagePack;
using ProtoBuf;

namespace SerializersBenchmark.Models;

[MessagePackObject]
[ProtoContract]
public class TestPayload
{
    [Key(0)]
    [ProtoMember(1)]
    public int Id { get; set; }

    [Key(1)]
    [ProtoMember(2)]
    public string Name { get; set; } = string.Empty;

    [Key(2)]
    [ProtoMember(3)]
    public bool IsActive { get; set; }

    [Key(3)]
    [ProtoMember(4)]
    public double Score { get; set; }

    [Key(4)]
    [ProtoMember(5)]
    public DateTime CreatedAt { get; set; }

    [Key(5)]
    [ProtoMember(6)]
    public string? Description { get; set; }

    [Key(6)]
    [ProtoMember(7)]
    public int? Age { get; set; }

    [Key(7)]
    [ProtoMember(8)]
    public double? Rating { get; set; }

    [Key(8)]
    [ProtoMember(9)]
    public string[] Tags { get; set; } = [];

    [Key(9)]
    [ProtoMember(10)]
    public List<string> Categories { get; set; } = [];

    [Key(10)]
    [ProtoMember(11)]
    public Address Address { get; set; } = new();
}

[MessagePackObject]
[ProtoContract]
public class Address
{
    [Key(0)]
    [ProtoMember(1)]
    public string Street { get; set; } = string.Empty;

    [Key(1)]
    [ProtoMember(2)]
    public string City { get; set; } = string.Empty;

    [Key(2)]
    [ProtoMember(3)]
    public string ZipCode { get; set; } = string.Empty;
}
