using System;

namespace riusco_mvc.Models
{
    public class Transaction
    {
        public int TransactionID { get; set; }
        public ProductDTO Product { get; set; }
        public UserDTO Seller { get; set; }
        public UserDTO Buyer { get; set; }
        public DateTime LastUpdate { get; set; }
        public string State { get; set; }

        public Transaction(int transactionId, ProductDTO product, UserDTO seller, UserDTO buyer, DateTime lastUpdate, string state)
        {
            TransactionID = transactionId;
            Product = product;
            Seller = seller;
            Buyer = buyer;
            LastUpdate = lastUpdate;
            State = state;
        }
    }
}