import {Task} from "../Entities/Task";
import JSendApiClient, {API_ENDPOINTS} from "./JSendApiClient.ts";

export async function getTask(id: number): Promise<Task | null> {
  return await JSendApiClient.get<Task>(`${API_ENDPOINTS.Tasks}/${id}`)
}

export async function createTask(task: Task): Promise<boolean> {
  const result = await JSendApiClient.create(API_ENDPOINTS.Tasks, {
    method: "POST",
    headers: {"Content-Type": "application/json"},
    body: JSON.stringify({
      listId: task.listId,
      position: task.position,
      name: task.name,
      businessValue: task.businessValue,
      description: task.description,
      categories: task.categories,
    }),
  });

  return result != -1;
}

export async function updateTask(
  updatedTask: Task,
  newPosition?: number
): Promise<boolean> {
  return await JSendApiClient.update(`${API_ENDPOINTS.Tasks}/${updatedTask.id}`, {
    id: updatedTask.id,
    position: newPosition ?? updatedTask.position,
    name: updatedTask.name,
    businessValue: updatedTask.businessValue,
    description: updatedTask.description,
    done: updatedTask.done,
    categories: updatedTask.categories,
  });
}

export async function deleteTask(id: number): Promise<boolean> {
  return await JSendApiClient.delete(`${API_ENDPOINTS.Tasks}/${id}`);
}
