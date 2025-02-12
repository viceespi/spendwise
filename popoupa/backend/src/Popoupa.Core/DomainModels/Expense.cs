using System.Text.Json.Serialization;
using Popoupa.Core.DomainModels;
using SurrealDb.Net.Models;


namespace Popoupa.Core.DomainModels
{
    public class Expense
    {
        [JsonConstructor]
        public Expense(Guid id, string description, decimal amount, DateTime date, Category category, Group group, bool isShared, Guid ownerId)
        {
            Id = id;
            Description = description;
            Amount = amount;
            Date = date;
            Category = category;
            Group = group;
            IsShared = isShared;
            OwnerId = ownerId;
        }

        public Guid Id { get; }
        public string Description { get; }
        public DateTime Date { get; }
        public decimal Amount { get; }
        public Category Category { get; }
        public Group Group { get; }
        public bool IsShared { get; }
        public Guid OwnerId { get; }

        public static bool Validate(Expense expense)
        {
            return expense.Date < DateTime.UtcNow;
        }
    }
}
