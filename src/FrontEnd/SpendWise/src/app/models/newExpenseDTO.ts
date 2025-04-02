export interface NewExpenseDto {
  description: string;
  amount: number;
  date: Date;
  ownerId: string;
}
