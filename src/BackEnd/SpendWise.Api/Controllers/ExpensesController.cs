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
        private readonly IExpenseManagementService _expenseManagementService;

        public ExpensesController(IExpenseManagementService expenseManagementService)
        {
            _expenseManagementService = expenseManagementService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateExpense([FromBody] NewExpenseDto newExpenseDto)
        {
            Result<Expense> inputResult = await _expenseManagementService.CreateExpense(newExpenseDto);
            return inputResult.Match<IActionResult>
            (

                newExpense =>
                {
                    return Ok(newExpense);
                }
                ,
                validationError =>
                {
                    return BadRequest(validationError.Errors);
                }

            );
        }

        [HttpGet]
        public async Task<IActionResult> GetAllExpenses([FromQuery] Guid ownerId)
        {
            List<Expense> expenses = await _expenseManagementService.GetAllExpenses(ownerId);
            return Ok(expenses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExpense([FromRoute] Guid id)
        {
            Expense? expense = await _expenseManagementService.GetExpense(id);
            return expense is not null ? Ok(expense) : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense([FromRoute] Guid id)
        {
            await _expenseManagementService.DeleteExpense(id);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateExpense([FromBody] ToUpdateExpenseDto toUpdateExpenseDTO)
        {
            Result<Expense> inputResult = await _expenseManagementService.UpdateExpense(toUpdateExpenseDTO);
            return inputResult.Match<IActionResult>
            (

                expense =>
                {
                    return Ok(expense);
                }
                ,
                validationError =>
                {
                    return BadRequest(validationError.Errors);
                }

            );
        }
    }
}