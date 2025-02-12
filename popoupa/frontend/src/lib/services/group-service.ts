import { type Group } from "../models/expenses";

export class GroupService {
  readonly allExpensesGroup: Group = {
    id: "all-elements",
    name: "All Expenses",
  };

  readonly unassignedGroup: Group = {
    id: "00000000-0000-0000-0000-000000000000",
    name: "Unassigned",
  };

  async getGroups(userId: string): Promise<Map<string, Group>> {
    const response = await fetch(
      `http://localhost:5010/users/${userId}/groups`
    );
    if (!response.ok) {
      throw new Error("Error during group list fetch");
    }
    const responseBodyData = await response.json();
    const userGroups = responseBodyData as Group[];
    userGroups.unshift(this.unassignedGroup);
    userGroups.unshift(this.allExpensesGroup);
    const groupsHashMap: Map<string, Group> = new Map();
    userGroups.forEach((group) => {
      groupsHashMap.set(group.id, group);
    });
    return groupsHashMap;
  }

  getCurrentGroup(
    urlParams: URLSearchParams,
    groupsHashMap: Map<string, Group>
  ): Group {
    let currentGroupId = urlParams.get("ExpenseGroupId");
    if (currentGroupId === "all-elements" || currentGroupId === null) {
      return this.allExpensesGroup;
    }
    let currentGroup = groupsHashMap.get(currentGroupId);
    if (currentGroup === undefined) {
      throw new Error("Invalid Group request");
    }
    return currentGroup;
  }

  async addNewGroup(formData: FormData, userId: string) {
    const groupName = formData.get("NewGroupName") as string;
    const newGroup: Group = {
      id: "00000000-0000-0000-0000-000000000000",
      name: groupName,
    };

    const response = await fetch(
      `http://localhost:5010/users/${userId}/groups`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(newGroup),
      }
    );
    if (!response.ok) {
      throw new Error("Error in database during new group creation!");
    }
  }

  async editGroup(formData: FormData) {
    const groupNewName = formData.get("GroupName") as string;
    const groupId = formData.get("GroupId") as string;
    const editedGroup: Group = {
      name: groupNewName,
      id: groupId,
    };

    const response = await fetch("http://localhost:5010/users/groups", {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(editedGroup),
    });
    if (!response.ok) {
      throw new Error("Error in database during group update!");
    }
  }

  async deleteGroup(formData: FormData) {
    const groupId = formData.get("GroupId") as string;

    const response = await fetch("http://localhost:5010/users/groups", {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(groupId),
    });
    if (!response.ok) {
      throw new Error("Error in database during group deletion!");
    }
  }

  
}
