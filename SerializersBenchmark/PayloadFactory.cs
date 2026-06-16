using SerializersBenchmark.Models;
using GooglePayload = SerializersBenchmark.ProtobufModels.TestPayload;
using GoogleAddress = SerializersBenchmark.ProtobufModels.Address;
using GooglePayloadList = SerializersBenchmark.ProtobufModels.TestPayloadList;

namespace SerializersBenchmark;

public static class PayloadFactory
{
    public static TestPayload CreatePayload(int index) => new()
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

    public static GooglePayload CreateProtoPayload(int index) => new()
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

    public static TestPayload[] CreatePayloads(int count)
    {
        var payloads = new TestPayload[count];
        for (int i = 0; i < count; i++)
            payloads[i] = CreatePayload(i);
        return payloads;
    }

    public static GooglePayloadList CreateProtoList(int count)
    {
        var list = new GooglePayloadList();
        for (int i = 0; i < count; i++)
            list.Items.Add(CreateProtoPayload(i));
        return list;
    }
}
