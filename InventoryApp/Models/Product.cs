using System.ComponentModel;

namespace InventoryApp.Models;

public class Product
{
    public int ProductID { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int InStock { get; set; }
    public int Min { get; set; }
    public int Max { get; set; }
    public BindingList<Part> AssociatedParts { get; set; } = new BindingList<Part>();

    public void addAssociatedPart(Part part)
    {
        AssociatedParts.Add(part);
    }

    public bool removeAssociatedPart(int partID)
    {
        Part? match = lookupAssociatedPart(partID);
        if (match == null) return false;
        return AssociatedParts.Remove(match);   // Remove returns true if something was removed
    }

    public Part? lookupAssociatedPart(int partID)
    {
        foreach (Part part in AssociatedParts)
        {
            if (part.PartID == partID) return part;
        }
        return null;   // "not found"; the ? in Part? says null is a possible result
    }
}