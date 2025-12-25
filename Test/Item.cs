namespace Test;

internal class Item
{
    public required string Name { get; set; }
    private int _weight;
    public int Weight
    {
        get => _weight;
        internal set 
        {
            _weight = value;
        }
    }
}
