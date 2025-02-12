using Popoupa.Core.DataBaseManipulation.Filters;
using Popoupa.Core.DomainModels;

namespace Popoupa.Core.DataBaseManipulation.Interfaces
{
    public interface IExpenseRepositoryInterface
    {
        Task<Guid> Add(Expense newExpense);
        Task<List<Guid>> AddMultiple (List<Expense> newExpenses);
        Task Update(Expense expense);
        Task UpdateMultiple(List<Expense> expenses);
        Task Delete(Guid expenseId);
        Task DeleteMultiple(List<Guid> expensesIds);
        Task<Expense?> Get(Guid expenseId, Guid userId);
        Task<Expense[]> GetAll(Filter filters, Guid userId);
        Task CreateOwnerExpenseRelation(Guid userId, Guid expenseId);
        Task CreateMultipleOwnerExpenseRelation(Guid userId, List<Guid> expensesIds);
        Task ShareExpense(List<(Guid, decimal)> toShareUsersIds, Guid expenseId);
        Task DeleteShareFromExpense(List<Guid> usersIds, Guid expenseId);
        Task<List<Expense?>> GetFromBankStatement(Guid bankStatementId, Guid userId);
    }
}