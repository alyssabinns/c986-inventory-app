using InventoryApp.Helpers;
using InventoryApp.Models;

namespace InventoryApp.Forms;

public class PartForm : Form
{
    private readonly Part? _existing;   // null = Add mode
    private readonly RadioButton rdoInhouse = new RadioButton { Text = "In-House", Location = new Point(150, 22), AutoSize = true };
    private readonly RadioButton rdoOutsourced = new RadioButton { Text = "Outsourced", Location = new Point(265, 22), AutoSize = true };
    private readonly TextBox txtId = UI.MakeTextBox(140, 65);
    private readonly TextBox txtName = UI.MakeTextBox(140, 105);
    private readonly TextBox txtInventory = UI.MakeTextBox(140, 145);
    private readonly TextBox txtPrice = UI.MakeTextBox(140, 185);
    private readonly TextBox txtMax = UI.MakeTextBox(140, 225, 80);
    private readonly TextBox txtMin = UI.MakeTextBox(290, 225, 80);
    private readonly Label lblSource = UI.MakeLabel("Machine ID", 30, 268);
    private readonly TextBox txtSource = UI.MakeTextBox(140, 265);
    private readonly Button btnSave = UI.MakeButton("Save", 210, 320);
    private readonly Button btnCancel = UI.MakeButton("Cancel", 295, 320);

    public PartForm(Part? existing = null)
    {
        _existing = existing;
        Text = existing == null ? "Add Part" : "Modify Part";
        BuildLayout();

        txtId.ReadOnly = true;   // IDs are auto-generated, never user-edited
        txtId.Enabled = false;

        // H: switching the radio button swaps what the bottom field means
        rdoInhouse.CheckedChanged += (sender, e) =>
            lblSource.Text = rdoInhouse.Checked ? "Machine ID" : "Company Name";

        btnSave.Click += BtnSave_Click;
        btnCancel.Click += (sender, e) => Close();   // H: cancel, nothing saved
        CancelButton = btnCancel;                    // Esc key also cancels

        if (existing == null)
        {
            txtId.Text = Inventory.NextPartID().ToString();
            rdoInhouse.Checked = true;
        }
        else
        {
            LoadPart(existing);   // C: populate with the existing part's data
        }
        // CheckedChanged doesn't fire when an Outsourced part loads, so set the label directly
        lblSource.Text = rdoInhouse.Checked ? "Machine ID" : "Company Name";
    }

    private void LoadPart(Part part)
    {
        txtId.Text = part.PartID.ToString();
        txtName.Text = part.Name;
        txtInventory.Text = part.InStock.ToString();
        txtPrice.Text = part.Price.ToString("0.00");
        txtMin.Text = part.Min.ToString();
        txtMax.Text = part.Max.ToString();

        // Pattern matching: check the runtime type AND cast in one step
        if (part is Inhouse inhouse)
        {
            rdoInhouse.Checked = true;
            txtSource.Text = inhouse.MachineID.ToString();
        }
        else if (part is Outsourced outsourced)
        {
            rdoOutsourced.Checked = true;
            txtSource.Text = outsourced.CompanyName;
        }
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        List<string> errors = InputValidator.CheckCommonFields(
            txtName.Text, txtInventory.Text, txtPrice.Text, txtMin.Text, txtMax.Text);

        if (rdoInhouse.Checked && !int.TryParse(txtSource.Text, out _))
            errors.Add("Machine ID must be a whole number.");
        if (rdoOutsourced.Checked && string.IsNullOrWhiteSpace(txtSource.Text))
            errors.Add("Company Name is required.");

        if (errors.Count > 0)
        {
            MessageBox.Show(string.Join(Environment.NewLine, errors), "Please Fix the Following",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;   // stay on the form so the user can correct it
        }

        // Validation passed, so Parse can't fail now
        Part part = rdoInhouse.Checked
            ? new Inhouse { MachineID = int.Parse(txtSource.Text) }
            : new Outsourced { CompanyName = txtSource.Text.Trim() };

        part.PartID = int.Parse(txtId.Text);
        part.Name = txtName.Text.Trim();
        part.InStock = int.Parse(txtInventory.Text);
        part.Price = decimal.Parse(txtPrice.Text);
        part.Min = int.Parse(txtMin.Text);
        part.Max = int.Parse(txtMax.Text);

        if (_existing == null) Inventory.addPart(part);        // H1: save new
        else Inventory.updatePart(_existing.PartID, part);     // H2: save changes
        Close();                                               // back to main form
    }

    private void BuildLayout()
    {
        ClientSize = new Size(400, 370);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        Controls.AddRange(new Control[] {
            UI.MakeLabel(Text, 20, 20, 11f),
            rdoInhouse, rdoOutsourced,
            UI.MakeLabel("ID", 30, 68), txtId,
            UI.MakeLabel("Name", 30, 108), txtName,
            UI.MakeLabel("Inventory", 30, 148), txtInventory,
            UI.MakeLabel("Price / Cost", 30, 188), txtPrice,
            UI.MakeLabel("Max", 30, 228), txtMax,
            UI.MakeLabel("Min", 245, 228), txtMin,
            lblSource, txtSource,
            btnSave, btnCancel });
    }
}