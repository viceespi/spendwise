using Popoupa.Core.DomainModels;

namespace Popoupa.Core.DataBaseManipulation.Interfaces
{
    public interface IBankStatementRepositoryInterface
    {
        Task<Guid> Add(BankStatement newBankStatement);
        Task Update(BankStatement bankStatement);
        Task Delete(Guid bankStatementId);
        Task<BankStatement?> Get(Guid bankStatementId);
        Task<List<BankStatement?>> GetAll(Guid userId);
        Task CreateUserBankStatementRelation(Guid userId, Guid bankStatementId);
        Task CreateBankStatementExpenseRelation(Guid bankStatementId, List<Guid> expensesIds);

    }
}