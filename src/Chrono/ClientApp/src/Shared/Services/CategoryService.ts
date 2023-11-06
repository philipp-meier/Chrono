import {Category} from "../Entities/Category";
import JSendApiClient, {API_ENDPOINTS} from "./JSendApiClient.ts";

export async function getCategories(): Promise<Category[]> {
  return await JSendApiClient.get<Category[]>(API_ENDPOINTS.Categories) ?? [];
}

export async function createCategory(name: string): Promise<number> {
  return await JSendApiClient.create(API_ENDPOINTS.Categories, {name: name});
}

export async function deleteCategory(id: number): Promise<boolean> {
  return await JSendApiClient.delete(`${API_ENDPOINTS.Categories}/${id}`);
}
