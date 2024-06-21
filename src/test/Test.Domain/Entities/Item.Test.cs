using Domain.Entities;
using Klean.Generated;

namespace Test.Domain.Entities;

public sealed class ItemTest
{
    [Test]
    [Parallelizable]
    public void TestItemCreation()
    {
        var itemId = ItemId.NewItemId();
        var strItemId = itemId.ToString();
        var parsed = ItemId.Parse(strItemId);

        Console.WriteLine(itemId);
        Console.WriteLine(parsed);

        Assert.That(parsed, Is.EqualTo(itemId));
    }
}