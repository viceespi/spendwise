using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpendWise.Domain.Models.ExpenseModels;
using SpendWise.Domain.Models.GlobalModels;

namespace SpendWise.Domain.Factories.Contracts
{
    public interface IExpenseFactory
    {
        Result<Expense> CreateExpenseFromNewExpenseDTO(NewExpenseDTO newExpenseDTO);

        Result<Expense> CreateExpenseFromToUpdateExpenseDTO(ToUpdateExpenseDTO toUpdateExpenseDTO);
        
    }
}