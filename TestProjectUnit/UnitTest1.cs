using FluentAssertions;
using Test;
using Xunit;

namespace TestProjectUnit;
public class InventoryTests
{
    [Fact]
    public void AddItem_ShouldAddNewItem_WhenInventoryIsEmpty()
    {
        var inventory = new Inventory();
        var item = new Item { Name = "Велосипед", Weight = 10 };
        inventory.AddItem(item);

        inventory.Items.Should().ContainSingle()
            .Which.Name.Should().Be("Велосипед");
    }

    [Fact]
    public void AddItem_ShouldSumWeight_WhenItemAlreadyExists()
    {
        var inventory = new Inventory();
        inventory.AddItem(new Item { Name = "Синий", Weight = 50 });
        var duplicate = new Item { Name = "Синий", Weight = 30 };

        inventory.AddItem(duplicate);

        inventory.Items.Should().HaveCount(1, "потому что предмет должен был суммироваться");
        inventory.Items[0].Weight.Should().Be(80); 
    }

    [Fact]
    public void AddItem_ShouldThrowException_WhenWeightExceedsLimit()
    {
        var inventory = new Inventory();
        inventory.AddItem(new Item { Name = "Первый велик", Weight = 90 });
        var heavyItem = new Item { Name = "Второй велик", Weight = 11 };

        Action act = () => inventory.AddItem(heavyItem);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*превышен*вес*");
    }

    [Fact]
    public void SearchItem_ShouldReturnFilteredResults()
    {
        var inventory = new Inventory();
        inventory.AddItem(new Item { Name = "Ручной насос", Weight = 5 });
        inventory.AddItem(new Item { Name = "Ножной насос", Weight = 10 });
        inventory.AddItem(new Item { Name = "Аккумуляторный", Weight = 8 });

        var results = inventory.SearchItem("насос");

        results.Should().HaveCount(2)
            .And.OnlyContain(i => i.Name.Contains("насос"));
    }

    [Fact]
    public void AddItem_ShouldBeThreadSafe()
    {
        var inventory = new Inventory();
        int threadsCount = 10;
        int itemsPerThread = 100;

        Parallel.For(0, threadsCount, i =>
        {
            for (int j = 0; j < itemsPerThread; j++)
            {
                inventory.AddItem(new Item { Name = $"Item_{i}_{j}", Weight = 0 });
            }
        });

        inventory.Items.Should().HaveCount(threadsCount * itemsPerThread);
    }
}
