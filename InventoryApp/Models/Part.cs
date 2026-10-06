namespace InventoryApp.Models;

//abstract = you can never write "new Part()". Only subclasses can exist
public abstract class Part
{
    public int PartID { get; set; }            // declared first so it's the first grid column
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }         // decimal, not double: exact for money
    public int InStock { get; set; }
    public int Min { get; set; }
    public int Max { get; set; }
}