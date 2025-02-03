using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpendWise.Domain.Models.ExpenseModels;
using SpendWise.Domain.Models.GlobalModels;
using SpendWise.Domain.Services.Contracts;

namespace SpendWise.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseManagementServices _expenseServices;

        public ExpensesController(IExpenseManagementServices expenseServices)
        {
            _expenseServices = expenseServices;
        }

        [HttpPost]
        public async Task<IActionResult> CreateExpense(NewExpenseDTO newExpenseDTO)
        {
            Result<Guid> inputResult = await _expenseServices.CreateExpense(newExpenseDTO);
            return inputResult.Match<IActionResult>
            (

                expenseId =>
                {
                    return Ok(expenseId);
                }
                ,
                validationError =>
                {
                    return BadRequest(validationError.Errors);
                }

            );
        }

        [HttpGet]
        public async Task<IActionResult> GetAllExpenses()
        {
            List<Expense> expenses = await _expenseServices.GetAllExpenses();
            return Ok(expenses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExpense(Guid id)
        {
            Expense? expense = await _expenseServices.GetExpense(id);
            return expense is not null ? Ok(expense) : NotFound();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteExpense(Guid id)
        {
            await _expenseServices.DeleteExpense(id);
            return Ok();
        }
    }
}