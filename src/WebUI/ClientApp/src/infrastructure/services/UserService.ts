import { UserInfo } from "../../domain/models/UserInfo";

export async function getCurrentUserInfo(): Promise<UserInfo> {
  try {
    const response = await fetch("/api/user");
    return await response.json();
  } catch (error) {
    return { isAuthenticated: false };
  }
}
