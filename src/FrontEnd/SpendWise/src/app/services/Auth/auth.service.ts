import { computed, Injectable } from '@angular/core';
import { User } from '../../models/User';
import { SessionService } from '../session.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private sessionService: SessionService) {}

  currentUser = computed(() => this.sessionService.currentUser());

  isLoggedIn(): boolean {
    return this.currentUser() !== null;
  }
}
