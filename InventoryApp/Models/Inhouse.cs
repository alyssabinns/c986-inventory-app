namespace InventoryApp.Models;

public class Inhouse : Part   // ":" means "inherits from": gets all of Part's properties
{
    public int MachineID { get; set; }
}