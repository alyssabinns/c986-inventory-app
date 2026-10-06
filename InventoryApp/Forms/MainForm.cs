using System.ComponentModel;
using InventoryApp.Helpers;
using InventoryApp.Models;

namespace InventoryApp.Forms;

public class MainForm : Form
{
    private readonly DataGridView dgvParts = new DataGridView();
    private readonly DataGridView dgvProducts = new DataGridView();
    private readonly TextBox txtPartSearch = UI.MakeTextBox(440, 70, 155);
    private readonly TextBox txtProductSearch = UI.MakeTextBox(1040, 70, 155);

    public MainForm()
    {
        Text = "Main Screen";
        ClientSize = new Size(1220, 510);
        StartPosition = FormStartPosition.CenterScreen;

        // Title labels (A)
        Controls.Add(UI.MakeLabel("Inventory Management System", 15, 15, 14f));
        Controls.Add(UI.MakeLabel("Parts", 25, 72, 12f));
        Controls.Add(UI.MakeLabel("Products", 625, 72, 12f));

        // ---- Parts side ----
        Button btnPartSearch = UI.MakeButton("Search", 355, 68);
        Button btnAddPart    = UI.MakeButton("Add", 360, 400);
        Button btnModifyPart = UI.MakeButton("Modify", 440, 400);
        Button btnDeletePart = UI.MakeButton("Delete", 520, 400);
        dgvParts.SetBounds(25, 105, 570, 280);
        UI.SetUpGrid(dgvParts, Inventory.AllParts);

        btnPartSearch.Click += BtnPartSearch_Click;
        btnAddPart.Click    += BtnAddPart_Click;
        btnModifyPart.Click += BtnModifyPart_Click;
        btnDeletePart.Click += BtnDeletePart_Click;

        // ---- Products side: same pattern ----
        Button btnProductSearch = UI.MakeButton("Search", 955, 68);
        Button btnAddProduct    = UI.MakeButton("Add", 960, 400);
        Button btnModifyProduct = UI.MakeButton("Modify", 1040, 400);
        Button btnDeleteProduct = UI.MakeButton("Delete", 1120, 400);
        dgvProducts.SetBounds(625, 105, 570, 280);
        UI.SetUpGrid(dgvProducts, Inventory.Products);

        btnProductSearch.Click += BtnProductSearch_Click;
        btnAddProduct.Click    += BtnAddProduct_Click;
        btnModifyProduct.Click += BtnModifyProduct_Click;
        btnDeleteProduct.Click += BtnDeleteProduct_Click;

        Button btnExit = UI.MakeButton("Exit", 1120, 455);
        btnExit.Click += (sender, e) => Application.Exit();   // G: exit

        Controls.AddRange(new Control[] {
            btnPartSearch, txtPartSearch, dgvParts, btnAddPart, btnModifyPart, btnDeletePart,
            btnProductSearch, txtProductSearch, dgvProducts, btnAddProduct, btnModifyProduct, btnDeleteProduct,
            btnExit });
    }

    // Returns the highlighted row's object, or null if nothing is selected
    private Part? SelectedPart() =>
        dgvParts.SelectedRows.Count > 0 ? dgvParts.SelectedRows[0].DataBoundItem as Part : null;

    private void ShowAllParts()
    {
        txtPartSearch.Clear();
        dgvParts.DataSource = Inventory.AllParts;   // undo any search filter
    }

    // ---------- G: redirect to forms ----------
    private void BtnAddPart_Click(object? sender, EventArgs e)
    {
        using var form = new PartForm();   // no argument = "add" mode
        form.ShowDialog(this);             // modal: main form waits until it closes
        ShowAllParts();
    }

    private void BtnModifyPart_Click(object? sender, EventArgs e)
    {
        Part? part = SelectedPart();
        if (part == null)
        {
            MessageBox.Show("Please select a part to modify.", "No Selection");
            return;
        }
        using var form = new PartForm(part);   // passing a part = "modify" mode
        form.ShowDialog(this);
        ShowAllParts();
    }

    // ---------- G: delete, with J validations ----------
    private void BtnDeletePart_Click(object? sender, EventArgs e)
    {
        Part? part = SelectedPart();
        if (part == null)
        {
            MessageBox.Show("Please select a part to delete.", "No Selection");
            return;
        }

        // J: can't delete a part that a product uses
        bool inUse = Inventory.Products.Any(p => p.lookupAssociatedPart(part.PartID) != null);
        if (inUse)
        {
            MessageBox.Show($"\"{part.Name}\" is associated with a product and cannot be deleted.",
                            "Part In Use", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // J: confirm before deleting
        DialogResult answer = MessageBox.Show($"Delete \"{part.Name}\"?", "Confirm Delete",
                                              MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (answer == DialogResult.Yes)
        {
            Inventory.deletePart(part);
            ShowAllParts();
        }
    }

    // ---------- G: search ----------
    private void BtnPartSearch_Click(object? sender, EventArgs e)
    {
        string term = txtPartSearch.Text.Trim();
        if (term.Length == 0) { ShowAllParts(); return; }   // empty search = show everything

        var matches = new BindingList<Part>();
        if (int.TryParse(term, out int id))
        {
            Part? found = Inventory.lookupPart(id);   // uses the UML method
            if (found != null) matches.Add(found);
        }
        else
        {
            foreach (Part part in Inventory.AllParts)
                if (part.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
                    matches.Add(part);
        }

        if (matches.Count == 0)
        {
            MessageBox.Show("No parts matched your search.", "Search");
            ShowAllParts();
            return;
        }
        dgvParts.DataSource = matches;   // show only the results
    }

    private Product? SelectedProduct() =>
        dgvProducts.SelectedRows.Count > 0 ? dgvProducts.SelectedRows[0].DataBoundItem as Product : null;

    private void ShowAllProducts()
    {
        txtProductSearch.Clear();
        dgvProducts.DataSource = Inventory.Products;
    }

    private void BtnAddProduct_Click(object? sender, EventArgs e)
    {
        using var form = new ProductForm();
        form.ShowDialog(this);
        ShowAllProducts();
    }

    private void BtnModifyProduct_Click(object? sender, EventArgs e)
    {
        Product? product = SelectedProduct();
        if (product == null)
        {
            MessageBox.Show("Please select a product to modify.", "No Selection");
            return;
        }
        using var form = new ProductForm(product);
        form.ShowDialog(this);
        ShowAllProducts();
    }

    private void BtnDeleteProduct_Click(object? sender, EventArgs e)
    {
        Product? product = SelectedProduct();
        if (product == null)
        {
            MessageBox.Show("Please select a product to delete.", "No Selection");
            return;
        }

        DialogResult answer = MessageBox.Show($"Delete \"{product.Name}\"?", "Confirm Delete",
                                              MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (answer == DialogResult.Yes)
        {
            Inventory.removeProduct(product.ProductID);
            ShowAllProducts();
        }
    }

    private void BtnProductSearch_Click(object? sender, EventArgs e)
    {
        string term = txtProductSearch.Text.Trim();
        if (term.Length == 0) { ShowAllProducts(); return; }

        var matches = new BindingList<Product>();
        if (int.TryParse(term, out int id))
        {
            Product? found = Inventory.lookupProduct(id);
            if (found != null) matches.Add(found);
        }
        else
        {
            foreach (Product product in Inventory.Products)
                if (product.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
                    matches.Add(product);
        }

        if (matches.Count == 0)
        {
            MessageBox.Show("No products matched your search.", "Search");
            ShowAllProducts();
            return;
        }
        dgvProducts.DataSource = matches;
    }
}
