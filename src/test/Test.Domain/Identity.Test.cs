using Domain.Aggregates;
using FluentAssertions;

namespace Test.Domain;

public sealed class IdentityTests
{
    [Test]
    public void TestCreation()
    {
        var id = UserId.New();
        Console.WriteLine(id);
        var idString = id.ToString();
        Console.WriteLine(idString);
        var idParsed = UserId.Parse(idString);
        id.Should().Be(idParsed);
    }
}