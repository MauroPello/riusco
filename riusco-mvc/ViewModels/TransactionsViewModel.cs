using System.Collections.Generic;
using riusco_mvc.Models;

namespace riusco_mvc.ViewModels
{
    public class TransactionsViewModel
    {
        public IEnumerable<Transaction> UserTransactions { get; set; }

        public TransactionsViewModel(IEnumerable<Transaction> userTransactions)
        {
            UserTransactions = userTransactions;
        }
    }
}