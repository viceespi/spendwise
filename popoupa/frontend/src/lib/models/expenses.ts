export interface Expense {
  id: string;
  description: string;
  date: Date;
  amount: number;
  category: Category;
  group: Group;
  ownerId: string;
  isShared: boolean;
}

export interface Group {
  id: string;
  name: string;
}

export interface Category {
  id: string;
  name: string;
}
