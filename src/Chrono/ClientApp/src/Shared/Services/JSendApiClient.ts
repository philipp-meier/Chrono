export const API_ENDPOINTS = {
  UserSettings: "/api/user/settings",
  Categories: "/api/categories",
  TaskLists: "/api/tasklists",
  Tasks: "/api/tasks",
  Notes: "/api/notes",
  User: "/api/user"
}

class JSendApiClient {

  static async get<T>(uri: string): Promise<T | null> {
    return await this.handleJSendResponseData(await fetch(uri));
  }

  static async create(uri: string, data: any): Promise<number> {
    const init = {
      method: "POST",
      headers: {"Content-Type": "application/json"},
      body: JSON.stringify(data),
    };

    return await this.handleJSendResponseData(await fetch(uri, init)) ?? -1;
  }

  static async update(uri: string, data: any): Promise<boolean> {
    try {
      const init = {
        method: "PUT",
        headers: {"Content-Type": "application/json"},
        body: JSON.stringify(data),
      };

      const response = await fetch(uri, init);
      return response.ok;
    } catch {
      return false;
    }
  }

  static async delete(uri: string): Promise<boolean> {
    try {
      const result = await fetch(uri, {method: "DELETE"});
      return result.ok;
    } catch {
      return false;
    }
  }

  private static async handleJSendResponseData<T>(response: Response): Promise<T | null> {
    try {
      if (!response.ok)
        return null;

      const responseBody = await response.json();
      return responseBody.data;
    } catch {
      return null;
    }
  }
}

export default JSendApiClient;