using System;
using System.Threading.Tasks;


using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TestProjectUnit")]


namespace Test;

internal class Inventory
{
    private readonly List<Item> _items = [];

    private readonly System.Threading.Lock _lock = new();
    private const int MaxWeight = 100;
    public IReadOnlyList<Item> Items
    {
        get
        {
            lock (_lock)
            {
                return _items.ToList();
            }
        }
    }

    public void AddItem(Item item)
    {
        ArgumentNullException.ThrowIfNull(item);
        
        lock (_lock)
        {
            int totalWeight = _items.Sum(i => i.Weight);

            if (totalWeight + item.Weight > MaxWeight)
                throw new InvalidOperationException(" превышен вес и не может быть > 100");


            var existingItem = _items.FirstOrDefault(i =>
            i.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase));
            
            if (_items.Contains(item))
                throw new ArgumentException("Попытка создания объекта, который уже имеется в списке");

            if (existingItem != null)
            {
                existingItem.Weight += item.Weight;
            }
            else
            {
                _items.Add(item);
            }
        }
    }

    public bool RemoveItem(Item item)
    {
        if (_items.Contains(item))
        {
            lock (_lock)
            {
                return _items.Remove(item);
            }
        }
            
        else throw new ArgumentException("Попытка удаления несуществующего объекта.");
    }
    public IEnumerable<Item> SearchItem(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return Enumerable.Empty<Item>();

        lock (_lock)
        {
            return _items
                .Where(item => item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}
