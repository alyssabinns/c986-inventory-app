using System.ComponentModel;
using InventoryApp.Helpers;
using InventoryApp.Models;

namespace InventoryApp.Forms;

public class ProductForm : Form
{
    private readonly Product _working = new Product();   // the editable copy
    private readonly bool _isModify;
    private readonly TextBox txtId = UI.MakeTextBox(120, 85);
    private readonly TextBox txtName = UI.MakeTextBox(120, 125);
    private readonly TextBox txtInventory = UI.MakeTextBox(120, 165);
    private readonly TextBox txtPrice = UI.MakeTextBox(120, 205);
    private readonly TextBox txtMax = UI.MakeTextBox(120, 245, 70);
    private readonly TextBox txtMin = UI.MakeTextBox(250, 245, 70);
    private readonly TextBox txtSearch = UI.MakeTextBox(815, 25, 155);
    private readonly DataGridView dgvAllParts = new DataGridView();
    private readonly DataGridView dgvAssociated = new DataGridView();
    private readonly Button btnSearch = UI.MakeButton("Search", 730, 23);
    private readonly Button btnAdd = UI.MakeButton("Add", 895, 270);
    private readonly Button btnDelete = UI.MakeButton("Delete", 895, 550);
    private readonly Button btnSave = UI.MakeButton("Save", 810, 590);
    private readonly Button btnCancel = UI.MakeButton("Cancel", 895, 590);

    public ProductForm(Product? existing = null)
    {
        _isModify = existing != null;
        Text = _isModify ? "Modify Product" : "Add Product";
        BuildLayout();
        txtId.ReadOnly = true;
        txtId.Enabled = false;

        if (existing != null)
        {
            _working.ProductID = existing.ProductID;
            foreach (Part part in existing.AssociatedParts)
                _working.addAssociatedPart(part);   // copy the list, not a reference to it

            txtName.Text = existing.Name;
            txtInventory.Text = existing.InStock.ToString();
            txtPrice.Text = existing.Price.ToString("0.00");
            txtMin.Text = existing.Min.ToString();
            txtMax.Text = existing.Max.ToString();
        }
        else
        {
            _working.ProductID = Inventory.NextProductID();
        }
        txtId.Text = _working.ProductID.ToString();

        UI.SetUpGrid(dgvAllParts, Inventory.AllParts);
        UI.SetUpGrid(dgvAssociated, _working.AssociatedParts);   // bound to the copy

        btnAdd.Click += BtnAdd_Click;
        btnDelete.Click += BtnDelete_Click;
        btnSave.Click += BtnSave_Click;
        btnSearch.Click += BtnSearch_Click;
        btnCancel.Click += (sender, e) => Close();   // edits live on _working, so nothing is saved
        CancelButton = btnCancel;
    }

    private void BuildLayout()
    {
        ClientSize = new Size(1000, 635);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        dgvAllParts.SetBounds(400, 60, 570, 200);
        dgvAssociated.SetBounds(400, 340, 570, 200);

        Controls.AddRange(new Control[] {
            UI.MakeLabel(Text, 20, 20, 11f),
            UI.MakeLabel("ID", 30, 88), txtId,
            UI.MakeLabel("Name", 30, 128), txtName,
            UI.MakeLabel("Inventory", 30, 168), txtInventory,
            UI.MakeLabel("Price", 30, 208), txtPrice,
            UI.MakeLabel("Max", 30, 248), txtMax,
            UI.MakeLabel("Min", 210, 248), txtMin,
            UI.MakeLabel("All Candidate Parts", 400, 30, 10f), btnSearch, txtSearch, dgvAllParts, btnAdd,
            UI.MakeLabel("Parts Associated with This Product", 400, 312, 10f), dgvAssociated, btnDelete,
            btnSave, btnCancel });
    }

    private void BtnAdd_Click(object? sender, EventArgs e)   // I: associate
    {
        if (dgvAllParts.SelectedRows.Count == 0)
        {
            MessageBox.Show("Select a part from All Candidate Parts first.", "No Selection");
            return;
        }
        Part part = (Part)dgvAllParts.SelectedRows[0].DataBoundItem;
        if (_working.lookupAssociatedPart(part.PartID) != null)
        {
            MessageBox.Show($"\"{part.Name}\" is already associated with this product.", "Duplicate Part");
            return;
        }
        _working.addAssociatedPart(part);   // stays in the upper grid too, per the spec
    }

    private void BtnDelete_Click(object? sender, EventArgs e)   // I: disassociate
    {
        if (dgvAssociated.SelectedRows.Count == 0)
        {
            MessageBox.Show("Select an associated part to remove.", "No Selection");
            return;
        }
        Part part = (Part)dgvAssociated.SelectedRows[0].DataBoundItem;
        if (MessageBox.Show($"Remove \"{part.Name}\" from this product?", "Confirm",
                            MessageBoxButtons.YesNo) == DialogResult.Yes)
        {
            _working.removeAssociatedPart(part.PartID);
        }
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        List<string> errors = InputValidator.CheckCommonFields(
            txtName.Text, txtInventory.Text, txtPrice.Text, txtMin.Text, txtMax.Text);
        if (errors.Count > 0)
        {
            MessageBox.Show(string.Join(Environment.NewLine, errors), "Please Fix the Following");
            return;
        }

        _working.Name = txtName.Text.Trim();
        _working.InStock = int.Parse(txtInventory.Text);
        _working.Price = decimal.Parse(txtPrice.Text);
        _working.Min = int.Parse(txtMin.Text);
        _working.Max = int.Parse(txtMax.Text);

        if (_isModify) Inventory.updateProduct(_working.ProductID, _working);
        else Inventory.addProduct(_working);
        Close();
    }

    private void BtnSearch_Click(object? sender, EventArgs e)
    {
        string term = txtSearch.Text.Trim();
        if (term.Length == 0) { dgvAllParts.DataSource = Inventory.AllParts; return; }

        var matches = new BindingList<Part>();
        if (int.TryParse(term, out int id))
        {
            Part? found = Inventory.lookupPart(id);
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
            txtSearch.Clear();
            dgvAllParts.DataSource = Inventory.AllParts;
            return;
        }
        dgvAllParts.DataSource = matches;
    }
}