import type { Category, Group } from "./expenses";

export interface BankStatementMessageObject {
    BSFormatType: string,
    BSContent: number[],
    ExpensesCategory: Category,
    ExpensesGroup: Group
}