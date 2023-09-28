import {Category} from "../Entities/Category";

export async function getCategories(): Promise<Category[]> {
  try {
    const response = await fetch(`/api/categories`);
    return response.ok ? await response.json() : [];
  } catch (error) {
    return [];
  }
}

export async function createCategory(name: string): Promise<number> {
  try {
    const response = await fetch("/api/categories", {
      method: "POST",
      headers: {"Content-Type": "application/json"},
      body: JSON.stringify({name: name}),
    });

    if (response.ok) return await response.json();

    return -1;
  } catch (error) {
    return -1;
  }
}

export async function deleteCategory(id: number): Promise<boolean> {
  try {
    const response = await fetch(`/api/categories/${id}`, {
      method: "DELETE",
    });

    return response.status === 204;
  } catch (error) {
    return false;
  }
}
