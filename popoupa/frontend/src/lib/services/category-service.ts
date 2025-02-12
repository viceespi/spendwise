import { type Category } from "../models/expenses";

export class CategoryService {
  readonly allCategoriesCategory: Category = {
    id: "all-elements",
    name: "All Categories",
  };
  readonly unassignedCategory: Category = {
    id: "00000000-0000-0000-0000-000000000000",
    name: "Unassigned",
  };

  async getCategories(userId: string): Promise<Map<string, Category>> {
    const response = await fetch(
      `http://localhost:5010/users/${userId}/categories`
    );
    if (!response.ok) {
      throw new Error("Error in the database during Category list fetch");
    }
    const responseBodyData = await response.json();
    const userCategories = responseBodyData as Category[];
    userCategories.unshift(this.unassignedCategory);
    userCategories.unshift(this.allCategoriesCategory);
    const categoriesHashMap: Map<string, Category> = new Map();
    userCategories.forEach((category) => {
      categoriesHashMap.set(category.id, category);
    });
    return categoriesHashMap;
  }

  getCurrentCategory(
    urlParams: URLSearchParams,
    categoriesHashMap: Map<string, Category>
  ): Category {
    let currentCategoryId = urlParams.get("ExpenseCategoryId");
    if (currentCategoryId === "all-elements" || currentCategoryId === null) {
      return this.allCategoriesCategory;
    }
    let currentCategory = categoriesHashMap.get(currentCategoryId);
    if (currentCategory === undefined) {
      throw new Error("Invalid category request");
    }
    return currentCategory;
  }

  async addNewCategory(formData: FormData, userId: string) {
    const categoryName = formData.get("NewCategoryName") as string;
    const newCategory: Category = {
      id: "00000000-0000-0000-0000-000000000000",
      name: categoryName,
    };
    
    const response = await fetch(
      `http://localhost:5010/users/${userId}/categories`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(newCategory),
      }
    );
    if (!response.ok) {
      throw new Error("Error in the database during category creation");
    }
  }

  async editCategory(formData: FormData) {
    const categoryNewName = formData.get("CategoryName") as string;
    const categoryId = formData.get("CategoryId") as string;
    const editedCategory: Category = {
      name: categoryNewName,
      id: categoryId,
    };

    const response = await fetch("http://localhost:5010/users/categories", {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(editedCategory),
    });
    if (!response.ok) {
      throw new Error("Error in the database during category update");
    }
  }

  async deleteCategory(formData: FormData) {
    const categoryId = formData.get("CategoryId") as string;

    const response = await fetch("http://localhost:5010/users/categories", {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(categoryId),
    });
    if (!response.ok) {
      throw new Error("Error in the database during category deletion");
    }
  }
}
