import type { User } from "../models/user";

export class UserServices {

  async getCurrentUser(): Promise<User> {
    var fooUser: User = {
        userName: "",
        userEmail: "",
        userId: ""
    }
    return fooUser;
  }

  async editUser(user: User) {}

  async getUserFriendList(userId: string): Promise<string[]> {
    return [];
  }

  async editUserFriendList(userId: string) {}
}
