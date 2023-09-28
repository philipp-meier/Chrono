import {Task} from "../Entities/Task";

export async function getTask(id: number): Promise<Task | null> {
  try {
    const response = await fetch(`/api/tasks/${id}`);
    return response.ok ? await response.json() : null;
  } catch (error) {
    return null;
  }
}

export async function createTask(task: Task): Promise<boolean> {
  try {
    const response = await fetch("/api/tasks", {
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

    return response.status === 200;
  } catch (error) {
    return false;
  }
}

export async function updateTask(
  updatedTask: Task,
  newPosition?: number
): Promise<boolean> {
  try {
    const response = await fetch(`/api/tasks/${updatedTask.id}`, {
      method: "PUT",
      headers: {"Content-Type": "application/json"},
      body: JSON.stringify({
        id: updatedTask.id,
        position: newPosition ?? updatedTask.position,
        name: updatedTask.name,
        businessValue: updatedTask.businessValue,
        description: updatedTask.description,
        done: updatedTask.done,
        categories: updatedTask.categories,
      }),
    });

    return response.status === 204;
  } catch (error) {
    return false;
  }
}

export async function deleteTask(id: number): Promise<boolean> {
  try {
    const response = await fetch(`/api/tasks/${id}`, {
      method: "DELETE",
    });

    return response.status === 204;
  } catch (error) {
    return false;
  }
}
