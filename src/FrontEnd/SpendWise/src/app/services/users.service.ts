import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '../models/User';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: null,
})
export class UsersService {
  constructor(private http: HttpClient) {}

  GetUsers(): Observable<User[]> {
    return this.http.get<User[]>('http://localhost:5029/users');
  }

  GetFriends(currentUserId: string): Observable<User[]> {
    return this.http.get<User[]>(
      `http://localhost:5029/users/${currentUserId}`
    );
  }
}
