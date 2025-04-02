export interface ToUpdateExpenseDto {
  description: string;
  amount: number;
  date: Date;
  id: string;
  ownerId: string;
}
