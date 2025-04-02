using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpendWise.Domain.Models.ExpenseModels
{
    public class Expense
    {
        public Expense(string description, DateTime date, decimal amount, Guid id, Guid ownerId)
        {
            Description = description;
            Date = date;
            Amount = amount;
            Id = id;
            OwnerId = ownerId;
        }

        
        public string Description { get; } = string.Empty;
        public DateTime Date { get; }
        public decimal Amount { get; }
        public Guid Id { get; }
        public Guid OwnerId { get; }
    }
}