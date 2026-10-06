using InventoryApp.Forms;
using InventoryApp.Models;

namespace InventoryApp;

internal static class Program
{
    [STAThread]   // required by WinForms
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Inventory.SeedSampleData();
        Application.Run(new MainForm());
    }
}