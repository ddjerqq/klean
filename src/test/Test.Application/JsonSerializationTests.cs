using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Cqrs.Users;
using Domain.Aggregates;
using Domain.ValueObjects;
using Generated;

namespace Test.Application;

public class JsonSerializationTests
{
    private JsonSerializerOptions _jsonOptions = default!;

    [SetUp]
    public void SetUp()
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            Converters = { new JsonStringEnumConverter() },
        };
        _jsonOptions.Converters.ConfigureGeneratedConverters();
    }

    [Test]
    public void Test_JsonSerializationOptionsForStrongIds()
    {
        var id = UserId.New();
        var userDto = new UserDto(id, "test", "test", null, Role.User, "stamp");

        var json = JsonSerializer.Serialize(userDto, _jsonOptions);
        Console.WriteLine(json);

        var deserialized = JsonSerializer.Deserialize<UserDto>(json, _jsonOptions);
        Console.WriteLine(deserialized);
    }
}