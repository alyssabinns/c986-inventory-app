using System.ComponentModel;

namespace InventoryApp.Models;

public static class Inventory
{
    public static BindingList<Product> Products { get; set; } = new BindingList<Product>();
    public static BindingList<Part> AllParts { get; set; } = new BindingList<Part>();

    // ---------- Products ----------
    public static void addProduct(Product product)
    {
        Products.Add(product);
        _highestProductID = Math.Max(_highestProductID, product.ProductID);
    }

    public static bool removeProduct(int productID)
    {
        Product? product = lookupProduct(productID);
        return product != null && Products.Remove(product);
    }

    public static Product? lookupProduct(int productID)
    {
        foreach (Product product in Products)
            if (product.ProductID == productID) return product;
        return null;
    }

    public static void updateProduct(int productID, Product updated)
    {
        for (int i = 0; i < Products.Count; i++)
        {
            if (Products[i].ProductID == productID)
            {
                Products[i] = updated;   // replacing an item refreshes the grid
                return;
            }
        }
    }

    // ---------- Parts ----------
    public static void addPart(Part part)
    {
        AllParts.Add(part);
        _highestPartID = Math.Max(_highestPartID, part.PartID);
    }

    public static bool deletePart(Part part) => AllParts.Remove(part);

    public static Part? lookupPart(int partID)
    {
        foreach (Part part in AllParts)
            if (part.PartID == partID) return part;
        return null;
    }

    public static void updatePart(int partID, Part updated)
    {
        for (int i = 0; i < AllParts.Count; i++)
        {
            if (AllParts[i].PartID == partID)
            {
                AllParts[i] = updated;
                break;
            }
        }
        // Helper addition: keep products pointing at the current version of this part
        foreach (Product product in Products)
        {
            for (int i = 0; i < product.AssociatedParts.Count; i++)
                if (product.AssociatedParts[i].PartID == partID)
                    product.AssociatedParts[i] = updated;
        }
    }

    // ---------- Helpers (allowed by UML note #2) ----------
    // Highest ID ever added, so a deleted item's ID is never handed out again
    private static int _highestPartID;
    private static int _highestProductID;

    public static int NextPartID() => _highestPartID + 1;
    public static int NextProductID() => _highestProductID + 1;

    public static void SeedSampleData()
    {
        addPart(new Inhouse    { PartID = 1, Name = "Wheel",      Price = 12.11m, InStock = 15, Min = 5, Max = 25, MachineID = 101 });
        addPart(new Inhouse    { PartID = 2, Name = "Pedal",      Price = 8.22m,  InStock = 11, Min = 5, Max = 25, MachineID = 102 });
        addPart(new Outsourced { PartID = 3, Name = "Chain",      Price = 8.33m,  InStock = 12, Min = 5, Max = 25, CompanyName = "ChainCo" });
        addPart(new Outsourced { PartID = 4, Name = "Seat",       Price = 4.44m,  InStock = 8,  Min = 2, Max = 15, CompanyName = "Comfort Seats" });
        addPart(new Inhouse    { PartID = 5, Name = "Handlebars", Price = 15.50m, InStock = 6,  Min = 2, Max = 12, MachineID = 105 });

        var redBike = new Product { ProductID = 1, Name = "Red Bicycle", Price = 15.67m, InStock = 15, Min = 1, Max = 25 };
        redBike.addAssociatedPart(lookupPart(1)!);   // ! = "I know this isn't null"
        redBike.addAssociatedPart(lookupPart(2)!);
        redBike.addAssociatedPart(lookupPart(3)!);
        addProduct(redBike);

        var tricycle = new Product { ProductID = 2, Name = "Yellow Tricycle", Price = 12.77m, InStock = 19, Min = 1, Max = 20 };
        tricycle.addAssociatedPart(lookupPart(1)!);
        tricycle.addAssociatedPart(lookupPart(4)!);
        addProduct(tricycle);

        var blueBike = new Product { ProductID = 3, Name = "Blue Bicycle", Price = 17.15m, InStock = 5, Min = 1, Max = 10 };
        blueBike.addAssociatedPart(lookupPart(1)!);
        blueBike.addAssociatedPart(lookupPart(3)!);
        blueBike.addAssociatedPart(lookupPart(5)!);
        addProduct(blueBike);

        addProduct(new Product { ProductID = 4, Name = "Training Wheels Kit", Price = 9.99m, InStock = 7, Min = 2, Max = 15 });
    }
}

