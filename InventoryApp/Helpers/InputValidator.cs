namespace InventoryApp.Helpers;

internal static class InputValidator
{
    public static List<string> CheckCommonFields(string name, string inventory, string price,
                                                 string min, string max)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name)) errors.Add("Name is required.");

        // TryParse returns false instead of crashing when the text isn't a number
        bool stockOk = int.TryParse(inventory, out int stock);
        bool priceOk = decimal.TryParse(price, out decimal priceValue);
        bool minOk   = int.TryParse(min, out int minValue);
        bool maxOk   = int.TryParse(max, out int maxValue);

        if (!stockOk) errors.Add("Inventory must be a whole number.");
        if (!priceOk) errors.Add("Price must be a number (for example, 12.50).");
        else if (priceValue < 0) errors.Add("Price cannot be negative.");
        if (!minOk) errors.Add("Min must be a whole number.");
        if (!maxOk) errors.Add("Max must be a whole number.");

        if (stockOk && stock < 0) errors.Add("Inventory cannot be negative.");
        if (minOk && minValue < 0) errors.Add("Min cannot be negative.");
        if (maxOk && maxValue < 0) errors.Add("Max cannot be negative.");

        // Only compare numbers once we know they parsed
        if (minOk && maxOk && minValue > maxValue)
            errors.Add("Min cannot be greater than Max.");
        else if (stockOk && minOk && maxOk && (stock < minValue || stock > maxValue))
            errors.Add("Inventory must be between Min and Max.");

        return errors;
    }
}