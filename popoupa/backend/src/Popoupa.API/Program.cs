using Microsoft.AspNetCore.Mvc;
using Popoupa.API.APIClasses;
using Popoupa.Core.DataBaseManipulation;
using Popoupa.Core.DataBaseManipulation.Filters;
using Popoupa.Core.DataBaseManipulation.Repositories;
using Popoupa.Core.DomainModels;
using Popoupa.Core.Parsers;
using System.Text;


namespace Popoupa.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "Popo Enabler",
                                  policy =>
                                  {
                                      policy.AllowAnyOrigin();
                                      policy.AllowAnyMethod();
                                      policy.AllowAnyHeader();
                                  });
            });
            var app = builder.Build();
            app.UseCors("Popo Enabler");

            // Garantir a integridade da DB. Só um gato até eu aprender Pipelines.
            
            var categoryRepositoryDB = new CategoryRepository();
            var groupRepositoryDB = new GroupRepository();
            var userRepositoryDB = new UserRepository();
            await categoryRepositoryDB.CategoryPipeLine();
            await groupRepositoryDB.GroupPipeLine();
            await userRepositoryDB.UserPipeLine();

            // Adicionar o extrato bancário no banco de dados AKA Adicionar múltiplas expenses

            // questionar o thedeu sobre o uso de uma transação para garantir que todos os passos desse endpoint ou sejam todos efetuados ou nenhum
            _ = app.MapPost("/users/{userId}/bankstatements", async ([FromBody] BankStatementMessageObject bankStatementObject, [FromRoute] Guid userId) =>
            {
                switch (bankStatementObject.BSFormatType)
                {
                    case "nubank_csv":
                        {
                            try
                            {
                                var expenseRepository = new ExpenseRepository();
                                var bankStatementRepository = new BankStatementRepository();
                                var parser = new NubankCSVParser();
                                var newBankStatement = new BankStatement(DateTime.UtcNow, bankStatementObject.MonthOfReference, "nubank", userId);
                                var newExpensesList = parser.Parse(bankStatementObject.BSContent.ToArray(), Encoding.UTF8, userId, bankStatementObject.ExpensesCategory, bankStatementObject.ExpensesGroup);

                                var newBankStatementId = await bankStatementRepository.Add(newBankStatement);
                                var newExpensesIdList = await expenseRepository.AddMultiple(newExpensesList);
                                await expenseRepository.CreateMultipleOwnerExpenseRelation(userId, newExpensesIdList);
                                await bankStatementRepository.CreateBankStatementExpenseRelation(newBankStatementId, newExpensesIdList);

                                return Results.StatusCode(201);
                            }
                            catch (InvalidBankStatementException)
                            {
                                return Results.StatusCode(401);
                            }
                            catch (Exception)
                            {
                                return Results.StatusCode(500);
                            }
                        }
                    case "nubank_pdf":
                        {
                            try
                            {
                                var expenseRepository = new ExpenseRepository();
                                var bankStatementRepository = new BankStatementRepository();
                                var parser = new NubankPDFParser();
                                var newBankStatement = new BankStatement(DateTime.UtcNow, bankStatementObject.MonthOfReference, "nubank", userId);
                                var newExpensesList = parser.Parse(bankStatementObject.BSContent.ToArray(), userId, bankStatementObject.ExpensesCategory, bankStatementObject.ExpensesGroup);

                                var newBankStatementId = await bankStatementRepository.Add(newBankStatement);
                                var newExpensesIdList = await expenseRepository.AddMultiple(newExpensesList);
                                await expenseRepository.CreateMultipleOwnerExpenseRelation(userId, newExpensesIdList);
                                await bankStatementRepository.CreateBankStatementExpenseRelation(newBankStatementId, newExpensesIdList);

                                return Results.StatusCode(201);
                            }
                            catch (InvalidBankStatementException)
                            {
                                return Results.StatusCode(401);
                            }
                            catch (Exception)
                            {
                                return Results.StatusCode(500);
                            }
                        }
                    case "sicoob": return Results.StatusCode(501);
                    case "bradesco": return Results.StatusCode(501);
                    case "itau": return Results.StatusCode(501);

                    default: return Results.StatusCode(501);

                }
            }
            );

            // Deletar o extrato bancário indicado pelo ID do banco de dados
            app.MapDelete("users/bankstatements", async ([FromBody] Guid bankStatementId) =>
            {
                try
                {
                    var bankStatementRepository = new BankStatementRepository();
                    await bankStatementRepository.Delete(bankStatementId);
                    return Results.StatusCode(204);
                }
                catch
                {
                    return Results.StatusCode(500);
                }
            });

            // Pegar os extratos bancários com base no ID do usuário
            app.MapGet("users/{userId}/bankstatements", async ([FromRoute] Guid userId) =>
            {
                try
                {
                    var bankStatementRepository = new BankStatementRepository();
                    await bankStatementRepository.GetAll(userId);
                    return Results.StatusCode(204);
                }
                catch
                {
                    return Results.StatusCode(500);
                }
            });

            // Adicionar 1 expense
            app.MapPost("users/{userId}/expenses", async ([FromBody] Expense newExpense, [FromRoute] Guid userId) =>
            {
                var isValid = Expense.Validate(newExpense);
                if (isValid)
                {
                    try
                    {
                        var expenseRepository = new ExpenseRepository();
                        var expenseId = await expenseRepository.Add(newExpense);
                        await expenseRepository.CreateOwnerExpenseRelation(userId, expenseId);
                        return Results.Ok(expenseId);
                    }
                    catch
                    {
                        return Results.StatusCode(500);
                    }
                }
                return Results.StatusCode(400);
            });



            // Atualizar 1 expense
            app.MapPut("users/{userId}/expenses", async ([FromBody] Expense expense, [FromRoute] Guid userId) =>
            {
                var isValid = Expense.Validate(expense);
                if (isValid && userId == expense.OwnerId)
                {
                    try
                    {
                        var expenseRepository = new ExpenseRepository();
                        await expenseRepository.Update(expense);
                        return Results.StatusCode(200); 
                    }
                    catch
                    {
                        return Results.StatusCode(500);
                    }
                }
                return Results.StatusCode(400);
            });

            // Atualizar multiplas expenses
            app.MapPut("expenses", async ([FromBody] List<Expense> updatedExpenses) =>
            {
                var isExpenseListValid = true;
                foreach (Expense expense in updatedExpenses)
                {
                    var isExpenseValid = Expense.Validate(expense);
                    if (!isExpenseValid) isExpenseListValid = false;
                }
                if (isExpenseListValid)
                {
                    try
                    {
                        var expenseRepository = new ExpenseRepository();
                        await expenseRepository.UpdateMultiple(updatedExpenses);
                        return Results.StatusCode(200);
                    }
                    catch
                    {
                        return Results.StatusCode(500);
                    }
                }
                return Results.StatusCode(400);
            });

            // Deletar 1 expense
            app.MapDelete("users/expenses/{expenseId}", async ([FromRoute] Guid expenseId) =>
            {
                try
                {
                    var expenseRepository = new ExpenseRepository();
                    await expenseRepository.Delete(expenseId);
                    return Results.StatusCode(204);
                }
                catch
                {
                    return Results.StatusCode(500);
                }
            });

            // Deletar múltiplas expenses
            app.MapDelete("expenses", async ([FromBody] List<Guid> expensesIdList) =>
            {
                try
                {
                    var expenseRepository = new ExpenseRepository();
                    await expenseRepository.DeleteMultiple(expensesIdList);
                    return Results.StatusCode(204);
                }
                catch
                {
                    return Results.StatusCode(500);
                }
            });

            // Pegar 1 expense 
            app.MapGet("users/{userId}/expenses/{expenseId}", async ([FromRoute] Guid userId ,[FromRoute] Guid expenseId) =>
            {
                try
                {
                    var expenseRepository = new ExpenseRepository();
                    var expense = await expenseRepository.Get(expenseId, userId);
                    return Results.Ok(expense);
                }
                catch
                {
                    return Results.StatusCode(500);
                }
            });

            // Pegar expenses baseadas em critérios
            app.MapGet("users/{userId}/expenses", async (HttpContext httpContext, [FromRoute] Guid userId) =>
            {
                try
                {
                    var queryData = httpContext.Request.QueryString.Value;
                    var filter = new Filter();
                    filter = filter.CreateFilterFromQuery(queryData);
                    var expenseRepository = new ExpenseRepository();
                    var expensesList = await expenseRepository.GetAll(filter, userId);
                    return Results.Ok(expensesList);
                }
                catch
                {
                    return Results.StatusCode(500);
                }
            }
            );

            // Criar 1 novo grupo
            app.MapPost("users/{userId}/groups", async ([FromRoute] Guid userId, [FromBody] Group newGroup) =>
            {
                try
                {
                    var groupRepository = new GroupRepository();
                    var groupId = await groupRepository.Add(newGroup);
                    await groupRepository.CreateUserGroupRelation(userId, groupId);
                    return Results.StatusCode(201);
                }
                catch
                {
                    return Results.StatusCode(500);
                }
            });

            // Atualizar 1 grupo
            app.MapPut("users/groups", async ([FromBody] Group editedGroup) =>
            {
                try
                {
                    var groupRepository = new GroupRepository();
                    await groupRepository.Update(editedGroup);
                    return Results.StatusCode(200);
                }
                catch
                {
                    return Results.StatusCode(500);
                }
            });

            // Deletar 1 grupo
            app.MapDelete("users/groups", async ([FromBody] Guid toDeleteGroupId) =>
            {
                try
                {
                    var groupRepository = new GroupRepository();
                    await groupRepository.Delete(toDeleteGroupId);
                    return Results.StatusCode(200);
                }
                catch
                {
                    return Results.StatusCode(500);
                }
            });

            // Pegar todos os grupos
            app.MapGet("users/{userId}/groups", async ([FromRoute] Guid userId) =>
            {
                try
                {
                    var groupRepository = new GroupRepository();
                    var groupList = await groupRepository.GetAll(userId);
                    return Results.Ok(groupList);
                }
                catch
                {
                    return Results.StatusCode(500);
                }
            });

            // Criar uma nova categoria
            app.MapPost("users/{userId}/categories", async ([FromBody] Category newCategory, [FromRoute] Guid userId) =>
            {
                try
                {
                    var categoryRepository = new CategoryRepository();
                    var categoryId = await categoryRepository.Add(newCategory);
                    await categoryRepository.CreateUserCategoryRelation(userId, categoryId);
                    return Results.StatusCode(201);
                }
                catch
                {
                    return Results.StatusCode(500);
                }
            });

            // Atualizar 1 categoria
            app.MapPut("users/categories", async ([FromBody] Category toUpdateCategory) =>
            {
                try
                {
                    var categoryRepository = new CategoryRepository();
                    await categoryRepository.Update(toUpdateCategory);
                    return Results.StatusCode(200);
                }
                catch
                {
                    return Results.StatusCode(500);
                }
            });

            // Deletar 1 categoria
            app.MapDelete("users/categories", async ([FromBody] Guid toDeleteCategoryId) =>
            {
                try
                {
                    var categoryRepository = new CategoryRepository();
                    await categoryRepository.Delete(toDeleteCategoryId);
                    return Results.StatusCode(204);
                }
                catch
                {
                    return Results.StatusCode(500);
                }
            });

            // Pegar 1 categoria
            app.MapGet("users/categories/{categoryId}", async ([FromRoute] Guid categoryId) =>
            {
                try
                {
                    var categoryRepository = new CategoryRepository();
                    var category = await categoryRepository.Get(categoryId);
                    return Results.Ok(category);
                }
                catch
                {
                    return Results.StatusCode(500);
                }
            });

            // Pegar categorias
            app.MapGet("users/{userId}/categories", async ([FromRoute] Guid userId) =>
            {
                try
                {
                    var categoriesRepository = new CategoryRepository();
                    var categoriesList = await categoriesRepository.GetAll(userId);
                    return Results.Ok(categoriesList);
                }
                catch
                {
                    return Results.StatusCode(500);
                }
            }
            );

            // Criar um novo usuário
            app.MapPost("/users", ([FromBody] User newUser) =>
            {
                try
                {
                    var userRepository = new UserRepository();
                    var newUserId = userRepository.Add(newUser);
                    return Results.Ok(newUserId);
                }
                catch
                {
                    return Results.StatusCode(500);
                }
            });

            // Atualizar dados do usu�rio
            app.MapPut("/users", async ([FromBody] User user) =>
            {
                try
                {
                    var userRepository = new UserRepository();
                    await userRepository.Update(user);
                    return Results.StatusCode(201);
                }
                catch
                {
                    return Results.StatusCode(500);
                }
            });

            //Deletar 1 usu�rio
            app.MapDelete("users", async ([FromBody] Guid userId) =>
            {
                try
                {
                    var userRepository = new UserRepository();
                    await userRepository.Delete(userId);
                    return Results.StatusCode(204);
                }
                catch
                {
                    return Results.StatusCode(500);
                }
            });

            // Pegar 1 usu�rio 
            app.MapGet("users/{userId}", async ([FromRoute] Guid userId) =>
            {
                try
                {
                    var userRepository = new UserRepository();
                    var user = await userRepository.Get(userId);
                    return Results.Ok(user);
                }
                catch
                {
                    return Results.StatusCode(500);
                }
            });

            // Pegar a lista de amigos do usu�rio
            app.MapGet("/users/{userId}/friends", async ([FromRoute] Guid userId) =>
            {
                try
                {
                    var userRepository = new UserRepository();
                    var usersList = await userRepository.GetAll(userId);
                    return Results.Ok(usersList);
                }
                catch
                {
                    return Results.StatusCode(500);
                }
            });

            app.Run();
        }
    }
}

