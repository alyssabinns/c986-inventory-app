namespace InventoryApp.Helpers;

internal static class UI
{
    public static Label MakeLabel(string text, int x, int y, float fontSize = 9f) =>
        new Label { Text = text, Location = new Point(x, y), AutoSize = true,
                    Font = new Font("Segoe UI", fontSize) };

    public static Button MakeButton(string text, int x, int y, int width = 75) =>
        new Button { Text = text, Location = new Point(x, y), Size = new Size(width, 30) };

    public static TextBox MakeTextBox(int x, int y, int width = 150) =>
        new TextBox { Location = new Point(x, y), Width = width };

    public static void SetUpGrid(DataGridView grid, object dataSource)
    {
        grid.ReadOnly = true;                  // edits happen in forms, not the grid
        grid.AllowUserToAddRows = false;       // hides the blank "new row"
        grid.AllowUserToDeleteRows = false;
        grid.MultiSelect = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.RowHeadersVisible = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        // Columns are generated from the class's properties once binding finishes.
        // Subscribe BEFORE setting DataSource so this runs every time.
        grid.DataBindingComplete += (sender, e) =>
        {
            if (grid.Columns["PartID"] is DataGridViewColumn partId) partId.HeaderText = "Part ID";
            if (grid.Columns["ProductID"] is DataGridViewColumn productId) productId.HeaderText = "Product ID";
            if (grid.Columns["InStock"] is DataGridViewColumn stock) stock.HeaderText = "Inventory";
            if (grid.Columns["Price"] is DataGridViewColumn price) price.DefaultCellStyle.Format = "C2";
            if (grid.Columns["AssociatedParts"] is DataGridViewColumn assoc) assoc.Visible = false;
        };

        grid.DataSource = dataSource;
    }
}