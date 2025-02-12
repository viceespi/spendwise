const currencyFormatter = new Intl.NumberFormat("pt-BR", {
  style: "currency",
  currency: "BRL",
});

export class ExpenseModelMapper {

  getDateElements(expenseDate: Date) : string[] {
    let expenseDay = expenseDate.getDate().toString();
    let expenseMonth = (expenseDate.getMonth() + 1).toString();
    let expenseYear = expenseDate.getFullYear().toString();
    if (expenseDate.getMonth() < 9) {
      expenseMonth = `0${expenseMonth}`;
    }
    if (expenseDate.getDate() < 10) {
      expenseDay = `0${expenseDay}`;
    }
    let dateElements: string[] = [expenseDay,expenseMonth,expenseYear];
    return dateElements;
  }

  getDateToRender(expenseDate: Date): string {
    let dateElements = this.getDateElements(expenseDate);
    let fullDate = `${dateElements[0]}/${dateElements[1]}/${dateElements[2]}`;
    return fullDate;
  }
  
  getValidDateForEditing(expenseDate: Date): string {
    let dateElements = this.getDateElements(expenseDate);
    let fullDate = `${dateElements[2]}-${dateElements[1]}-${dateElements[0]}`;
    return fullDate;
  }
  
  getCurrencyAmount (expenseAmount: number) : string{
    let moduleExpenseAmount = Math.abs(expenseAmount);
    let negativeExpenseAmount = moduleExpenseAmount * -1;
    let expenseToRenderAmount = currencyFormatter.format(
      negativeExpenseAmount
    );
    return expenseToRenderAmount;
  }
}
