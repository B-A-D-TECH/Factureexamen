using System;

namespace Facture
{
    [Serializable]
    public class Invoice
    {
        public int Id { get; set; }
        // Numéro de facture (ex: INV-2026-001)
        public string InvoiceNumber { get; set; }
        public string Client { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        // Devise (EUR, USD, ...)
        public string Currency { get; set; }
        // Délai de paiement en jours
        public int PaymentTermDays { get; set; }
        // Moyen de paiement (Virement, Carte, Espèces, ...)
        public string PaymentMethod { get; set; }
        // Observations ou notes
        public string Observation { get; set; }
    }
}
