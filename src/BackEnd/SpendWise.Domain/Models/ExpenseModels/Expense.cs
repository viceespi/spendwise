using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpendWise.Domain.Models.ExpenseModels
{
    public class Expense
    {
        public Expense(string description, DateTime date, decimal amount, Guid id)
        {
            Description = description;
            Date = date;
            Amount = amount;
            Id = id;
        }
        public string Description { get; } = string.Empty;
        public DateTime Date { get; }
        public decimal Amount { get; }
        public Guid Id { get; }
    }
}