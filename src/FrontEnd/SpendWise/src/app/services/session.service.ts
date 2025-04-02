import { Injectable, signal } from '@angular/core';
import { User } from '../models/User';

@Injectable({
  providedIn: 'root',
})
export class SessionService {
  constructor() {}

  currentUser = signal<User | null>(null);

  ChangeUser(newUser: User) {
    this.currentUser.set(newUser);
  }
}
