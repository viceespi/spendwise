using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpendWise.Domain.Models.ExpenseModels
{
    public class NewExpenseDTO
    {
        public NewExpenseDTO(string description, DateTime date, decimal amount)
        {
            Description = description;
            Date = date;
            Amount = amount;
        }
        public string Description { get; } = string.Empty;
        public DateTime Date { get; }
        public decimal Amount { get; }
    }
}