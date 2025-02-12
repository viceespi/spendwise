import { type Filter} from "../models/Filter";
export class FilterService  {
    createFilter(filterQueryString : URLSearchParams): Filter {
        let newFilter: Filter = {};
        if (filterQueryString.size === 0) return newFilter;
        const startingDateString = filterQueryString.get("ExpenseStartingDate") as string;
        const endingDateString = filterQueryString.get("ExpenseEndingDate") as string;
        const startingDate = startingDateString === '' || startingDateString === null ? undefined : new Date(startingDateString);
        const endingDate = endingDateString === '' || endingDateString === null ? undefined : new Date(endingDateString);
        const expenseDescription = filterQueryString.get("ExpenseDescriptionCriteria") as string === '' ? undefined : filterQueryString.get("ExpenseDescriptionCriteria") as string;
        const expenseCategoryId = filterQueryString.get("ExpenseCategoryId") as string === '' || filterQueryString.get("ExpenseCategoryId") === "all-elements" ? undefined : filterQueryString.get("ExpenseCategoryId") as string; 
        const expenseGroupId = filterQueryString.get("ExpenseGroupId") as string === '' || filterQueryString.get("ExpenseGroupId") === "all-elements"  ? undefined : filterQueryString.get("ExpenseGroupId") as string;
        newFilter = {
            StartingDate: startingDate,
            EndingDate: endingDate,
            ExpenseDescription: expenseDescription,
            CategoryId: expenseCategoryId,
            GroupId: expenseGroupId,
        }
        return newFilter;
    }
}