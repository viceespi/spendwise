using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Popoupa.Core.DomainModels;

namespace Popoupa.Core.DataBaseManipulation.DBClasses
{
    public class ExpenseEntity
    {
        public ExpenseEntity(Guid id, string description, decimal amount, DateTime date, Guid category, Guid group, Guid ownerId, bool isShared)
        {
            Id = id;
            Description = description;
            Amount = amount;
            Date = date;
            ExpenseCategoryId = category;
            ExpenseGroupId = group;
            OwnerId = ownerId;
            IsShared = isShared;

        }
        public Guid Id { get; }
        public string Description { get; } = string.Empty;
        public decimal Amount { get; }
        public DateTime Date { get; }
        public Guid ExpenseCategoryId { get; }
        public Guid ExpenseGroupId { get; }
        public bool IsShared {get; }
        public Guid OwnerId { get; }
    }
}