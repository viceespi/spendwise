import type { Filter } from "../models/Filter";
import { type Expense, type Group, type Category } from "../models/expenses";

export class ExpenseService {
  readonly unassignedGroup: Group = {
    id: "00000000-0000-0000-0000-000000000000",
    name: "Unassigned",
  };

  readonly unassignedCategory: Category = {
    id: "00000000-0000-0000-0000-000000000000",
    name: "Unassigned",
  };

  async createSingleExpense(
    formData: FormData,
    categoriesHashMap: Map<string, Category>,
    groupsHashMap: Map<string, Group>,
    currentUserId: string
  ) {
    const DateAsString = formData.get("ExpenseDate") as string;
    const ExpenseAmount = formData.get("ExpenseAmount") as string;
    const CategoryId = formData.get("ExpenseCategoryId") as string;
    const GroupId = formData.get("ExpenseGroupId") as string;

    const description = formData.get("ExpenseDescription") as string;

    const date = new Date(DateAsString);

    const isShared = false;
    const ownerId = currentUserId;

    const amount = parseInt(ExpenseAmount);
    let category = categoriesHashMap.get(CategoryId);
    let group = groupsHashMap.get(GroupId);
    let id = "00000000-0000-0000-0000-000000000000";

    if (category === undefined || group === undefined) {
      throw new Error("Invalid Category or Group!");
    }

    const response = await fetch(
      `http://localhost:5010/users/${currentUserId}/expenses`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          id,
          description,
          amount,
          date,
          category,
          group,
          ownerId,
          isShared,
        }),
      }
    );
    if (!response.ok) {
      if (response.status === 400) {
        throw new Error("Error, the expense is invalid!");
      } else {
        throw new Error("Error in the DataBase during the expense creation!");
      }
    }
  }

  async createBankStatement(
    formData: FormData,
    categoriesHashMap: Map<string, Category>,
    groupsHashMap: Map<string, Group>,
    currentUserId: string
  ) {
    const CategoryId = formData.get("BankStatementCategory") as string;
    const GroupId = formData.get("BankStatementGroup") as string;
    const bankStatementFile = formData.get("BankStatementFile") as File;
    const bankStatementArrayBuffer = await bankStatementFile.arrayBuffer();

    const BSContent = Array.from(new Uint8Array(bankStatementArrayBuffer));
    const BSFormatType = formData.get("BankStatementType") as string;
    let Category = categoriesHashMap.get(CategoryId);
    let Group = groupsHashMap.get(GroupId);
    let MonthOfReference = formData.get("BankStatementMonthOfReference") as string;
    if (Group === undefined || Category === undefined) {
      throw new Error("Invalid Category or Group!");
    }

    const response = await fetch(
      `http://localhost:5010/users/${currentUserId}/bankstatements`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          BSFormatType: BSFormatType,
          BSContent: BSContent,
          ExpensesCategory: Category,
          ExpensesGroup: Group,
          MonthOfReference: MonthOfReference,
        }),
      }
    );
    if (!response.ok) {
      if (response.status === 401) {
        throw new Error("Error, the Bank Statement is invalid!");
      } else if (response.status === 501) {
        throw new Error(
          "Error, the selected Bank Statement is not yet implemented!"
        );
      } else {
        throw new Error("Error in the DataBase during expenses creation!");
      }
    }
  }

  async editSingleExpense(
    formData: FormData,
    categoriesHashMap: Map<string, Category>,
    groupsHashMap: Map<string, Group>,
    currentcurrentUserId: string
  ) {
    const DateAsString = formData.get("ExpenseDate") as string;
    const ExpenseAmount = formData.get("ExpenseAmount") as string;
    const CategoryId = formData.get("ExpenseCategoryId") as string;
    const GroupId = formData.get("ExpenseGroupId") as string;

    const Description = formData.get("ExpenseDescription") as string;
    const Id = formData.get("ExpenseId") as string;
    const OwnerId = formData.get("OwnerId") as string;
    const date = new Date(DateAsString);
    const Amount = parseInt(ExpenseAmount);
    const Category = categoriesHashMap.get(CategoryId);
    const Group = groupsHashMap.get(GroupId);
    if (Category === undefined || Group === undefined) {
      throw new Error("Invalid Category or Group!");
    }

    const response = await fetch(`http://localhost:5010/users/${currentcurrentUserId}/expenses`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        Id,
        Description,
        date,
        Amount,
        Category,
        Group,
        OwnerId,
      }),
    });
    if (!response.ok) {
      if (response.status === 400) {
        throw new Error("Error, the expense is invalid!");
      } else {
        throw new Error("Error in the DataBase during the expense update!");
      }
    }
  }

  async editMultipleExpenses(
    formData: FormData,
    expenseHashMap: Map<string, Expense>,
    categoriesHashMap: Map<string, Category>,
    groupsHashMap: Map<string, Group>
  ) {
    let selectedExpensesIds: string[] = [];
    for (let [checkboxName, checkboxValue] of formData.entries()) {
      if (checkboxName === "SelectedExpenseId") {
        selectedExpensesIds.push(checkboxValue as string);
      }
    }
    let expensesCategoryId = formData.get("ToEditExpensesCategoryId") as string;
    let expensesGroupId = formData.get("ToEditExpensesGroupId") as string;

    let toEditExpenses: Expense[] = [];
    let toEditExpensesCategory = categoriesHashMap.get(expensesCategoryId);
    let toEditExpensesGroup = groupsHashMap.get(expensesGroupId);

    if (
      toEditExpensesCategory !== undefined &&
      toEditExpensesGroup !== undefined
    ) {
      selectedExpensesIds.forEach((expenseId) => {
        let selectedExpense = expenseHashMap.get(expenseId);
        if (selectedExpense !== undefined) {
          selectedExpense.category = toEditExpensesCategory;
          selectedExpense.group = toEditExpensesGroup;
          toEditExpenses.push(selectedExpense);
        }
      });
    } else if (
      toEditExpensesCategory !== undefined &&
      toEditExpensesGroup === undefined
    ) {
      selectedExpensesIds.forEach((expenseId) => {
        let selectedExpense = expenseHashMap.get(expenseId);
        if (selectedExpense !== undefined) {
          selectedExpense.category = toEditExpensesCategory;
          toEditExpenses.push(selectedExpense);
        }
      });
    } else if (
      toEditExpensesCategory === undefined &&
      toEditExpensesGroup !== undefined
    ) {
      selectedExpensesIds.forEach((expenseId) => {
        let selectedExpense = expenseHashMap.get(expenseId);
        if (selectedExpense !== undefined) {
          selectedExpense.group = toEditExpensesGroup;
          toEditExpenses.push(selectedExpense);
        }
      });
    } else {
      throw new Error("Invalid categorization for expenses!");
    }

    const response = await fetch("http://localhost:5010/expenses", {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(toEditExpenses),
    });
    if (!response.ok) {
      if (response.status === 400) {
        throw new Error("Error, one of the expenses is invalid!");
      } else {
        throw new Error("Error in the DataBase during the expense update!");
      }
    }
  }

  async deleteSingleExpense(formData: FormData) {
    const expenseId = formData.get("ExpenseId") as string;
    const response = await fetch(
      `http://localhost:5010/users/expenses/${expenseId}`,
      {
        method: "DELETE",
        headers: {
          "Content-Type": "application/json",
        },
      }
    );
    if (!response.ok) {
      if (response.status === 400) {
        throw new Error("Error, the expense is invalid!");
      } else {
        throw new Error("Error in the DataBase during the expense deletion!");
      }
    }
  }

  async deleteMultipleExpensesFromCheckBox(formData: FormData) {
    let selectedExpensesIds: string[] = [];
    for (let [checkboxName, checkboxValue] of formData.entries()) {
      if (checkboxName === "SelectedExpenseId") {
        selectedExpensesIds.push(checkboxValue as string);
      }
    }
    const response = await fetch("http://localhost:5010/expenses/", {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(selectedExpensesIds),
    });
    if (!response.ok) {
      if (response.status === 400) {
        throw new Error("Error, one of the expenses is invalid!");
      } else {
        throw new Error("Error in the DataBase during the expense deletion!");
      }
    }
  }

  private async getExpenses(
    filters: Filter,
    currentUserId: string
  ): Promise<Expense[]> {
    let queryParams = new URLSearchParams();

    if (filters.CategoryId != undefined)
      queryParams.append("CategoryId", filters.CategoryId);
    if (filters.GroupId != undefined)
      queryParams.append("GroupId", filters.GroupId);
    if (filters.ExpenseDescription != undefined)
      queryParams.append("ExpenseDescription", filters.ExpenseDescription);
    if (filters.StartingDate != undefined)
      queryParams.append("StartingDate", filters.StartingDate.toISOString());
    if (filters.EndingDate != undefined)
      queryParams.append("EndingDate", filters.EndingDate.toISOString());

    const response = await fetch(
      `http://localhost:5010/users/${currentUserId}/expenses?${queryParams.toString()}`
    );
    if (!response.ok) {
      throw new Error("Error during the fetch");
    }
    const responseBodyData = await response.text();
    const expenseList = JSON.parse(responseBodyData, (key, value) => {
      if (key === "date") {
        return new Date(value);
      } else {
        return value;
      }
    }) as Expense[];
    return expenseList;
  }

  async getExpenseHashTable(
    filters: Filter,
    currentUserId: string
  ): Promise<Map<string, Expense>> {
    let expenseList = await this.getExpenses(filters, currentUserId);

    const expenseHashMap: Map<string, Expense> = new Map();
    expenseList.forEach((expense) => {
      if (expense.id !== null) {
        expenseHashMap.set(expense.id, expense);
      }
    });
    return expenseHashMap;
  }

  async resetExpensesGroup(formData: FormData, currentUserId: string) {
    let groupId = formData.get("GroupId") as string;
    let filter: Filter = {
      GroupId: groupId,
    };
    let expenseList = await this.getExpenses(filter, currentUserId);
    expenseList.forEach((expense) => {
      expense.group = this.unassignedGroup;
    });
    const response = await fetch("http://localhost:5010/expenses", {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(expenseList),
    });
    if (!response.ok) {
      if (response.status === 400) {
        throw new Error("Error, one of the expenses is invalid!");
      } else {
        throw new Error("Error in the DataBase during the expense update!");
      }
    }
  }

  async deleteExpensesWithGroup(formData: FormData, currentUserId: string) {
    let groupId = formData.get("GroupId") as string;
    let filter: Filter = {
      GroupId: groupId,
    };
    let expenseList = await this.getExpenses(filter, currentUserId);
    let expenseIdList: string[] = [];
    expenseList.forEach((expense) => {
      expenseIdList.push(expense.id);
    });

    const response = await fetch("http://localhost:5010/expenses/", {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(expenseIdList),
    });
    if (!response.ok) {
      if (response.status === 400) {
        throw new Error("Error, one of the expenses is invalid!");
      } else {
        throw new Error("Error in the DataBase during the expense deletion!");
      }
    }

  }

  async resetExpensesCategory(formData: FormData, currentUserId: string) {
    let categoryId = formData.get("CategoryId") as string;
    let filter: Filter = {
      CategoryId: categoryId,
    };
    let expenseList = await this.getExpenses(filter, currentUserId);
    expenseList.forEach((expense) => {
      expense.category = this.unassignedCategory;
    });
    const response = await fetch("http://localhost:5010/expenses", {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(expenseList),
    });
    if (!response.ok) {
      if (response.status === 400) {
        throw new Error("Error, one of the expenses is invalid!");
      } else {
        throw new Error("Error in the DataBase during the expense update!");
      }
    }
  }
}
