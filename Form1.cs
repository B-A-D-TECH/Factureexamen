using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Facture
{
    public partial class Form1 : Form
    {
        private BindingList<Invoice> invoices = new BindingList<Invoice>();
        private int nextId = 1;

        public Form1()
        {
            InitializeComponent();
            dataGridViewInvoices.DataSource = invoices;
            dataGridViewInvoices.Columns["Id"].ReadOnly = true;
            dateTimePickerDate.Value = DateTime.Today;
            // remplir quelques devises courantes
            comboBoxCurrency.Items.AddRange(new object[] { "EUR", "USD", "GBP", "CHF" });
            comboBoxCurrency.SelectedIndex = 0;
            numericUpDownPaymentTerm.Value = 30;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var inv = new Invoice
            {
                Id = nextId++,
                InvoiceNumber = textBoxInvoiceNumber.Text.Trim(),
                Client = textBoxClient.Text.Trim(),
                Date = dateTimePickerDate.Value.Date,
                Amount = numericUpDownAmount.Value,
                Currency = comboBoxCurrency.SelectedItem?.ToString() ?? string.Empty,
                PaymentTermDays = (int)numericUpDownPaymentTerm.Value,
                PaymentMethod = textBoxPaymentMethod.Text.Trim(),
                Observation = textBoxObservation.Text
            };
            invoices.Add(inv);
            ClearInputs();
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridViewInvoices.SelectedRows.Count == 0) return;
            var row = dataGridViewInvoices.SelectedRows[0];
            if (row.DataBoundItem is Invoice inv)
            {
                inv.InvoiceNumber = textBoxInvoiceNumber.Text.Trim();
                inv.Client = textBoxClient.Text.Trim();
                inv.Date = dateTimePickerDate.Value.Date;
                inv.Amount = numericUpDownAmount.Value;
                inv.Currency = comboBoxCurrency.SelectedItem?.ToString() ?? string.Empty;
                inv.PaymentTermDays = (int)numericUpDownPaymentTerm.Value;
                inv.PaymentMethod = textBoxPaymentMethod.Text.Trim();
                inv.Observation = textBoxObservation.Text;
                dataGridViewInvoices.Refresh();
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewInvoices.SelectedRows.Count == 0) return;
            var row = dataGridViewInvoices.SelectedRows[0];
            if (row.DataBoundItem is Invoice inv)
            {
                invoices.Remove(inv);
                ClearInputs();
            }
        }

        private void dataGridViewInvoices_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewInvoices.SelectedRows.Count == 0)
            {
                ClearInputs();
                return;
            }
            var row = dataGridViewInvoices.SelectedRows[0];
            if (row.DataBoundItem is Invoice inv)
            {
                textBoxInvoiceNumber.Text = inv.InvoiceNumber;
                textBoxClient.Text = inv.Client;
                dateTimePickerDate.Value = inv.Date;
                numericUpDownAmount.Value = inv.Amount;
                if (!string.IsNullOrEmpty(inv.Currency) && comboBoxCurrency.Items.Contains(inv.Currency))
                    comboBoxCurrency.SelectedItem = inv.Currency;
                else if (!string.IsNullOrEmpty(inv.Currency))
                {
                    comboBoxCurrency.Items.Add(inv.Currency);
                    comboBoxCurrency.SelectedItem = inv.Currency;
                }
                numericUpDownPaymentTerm.Value = Math.Min(Math.Max(inv.PaymentTermDays, (int)numericUpDownPaymentTerm.Minimum), (int)numericUpDownPaymentTerm.Maximum);
                textBoxPaymentMethod.Text = inv.PaymentMethod;
                textBoxObservation.Text = inv.Observation;
            }
        }

        private void ClearInputs()
        {
            textBoxInvoiceNumber.Text = string.Empty;
            textBoxClient.Text = string.Empty;
            dateTimePickerDate.Value = DateTime.Today;
            numericUpDownAmount.Value = 0;
            comboBoxCurrency.SelectedIndex = 0;
            numericUpDownPaymentTerm.Value = 30;
            textBoxPaymentMethod.Text = string.Empty;
            textBoxObservation.Text = string.Empty;
            dataGridViewInvoices.ClearSelection();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                    sfd.FileName = "invoices.xml";
                    if (sfd.ShowDialog() != DialogResult.OK) return;
                    using (var fs = System.IO.File.Create(sfd.FileName))
                    {
                        var ser = new System.Xml.Serialization.XmlSerializer(typeof(List<Invoice>));
                        ser.Serialize(fs, invoices.ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'enregistrement : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            try
            {
                using (var ofd = new OpenFileDialog())
                {
                    ofd.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                    if (ofd.ShowDialog() != DialogResult.OK) return;
                    using (var fs = System.IO.File.OpenRead(ofd.FileName))
                    {
                        var ser = new System.Xml.Serialization.XmlSerializer(typeof(List<Invoice>));
                        var list = (List<Invoice>)ser.Deserialize(fs);
                        invoices.Clear();
                        foreach (var i in list)
                        {
                            invoices.Add(i);
                        }
                        nextId = invoices.Any() ? invoices.Max(x => x.Id) + 1 : 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
