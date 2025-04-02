import { Component, computed, signal } from '@angular/core';
import { ButtonComponent } from '../../../components/general-components/button/button.component';
import { UsersService } from '../../../services/users.service';
import { User } from '../../../models/User';
import { SessionService } from '../../../services/session.service';

@Component({
  selector: 'app-user-info',
  imports: [ButtonComponent],
  templateUrl: './user-info.component.html',
  styleUrl: './user-info.component.css',
  providers: [UsersService],
})

// COLOCAR GUARD AQUI, ESSA PÁGINA SÓ PODE SER ACESSADA SE CURRENTUSER != NULL;;
export class UserInfoComponent {
  constructor(
    private userService: UsersService,
    private sessionService: SessionService
  ) {}

  currentUser = computed(() => this.sessionService.currentUser());
  userFriends = signal<User[]>([]);

  ngOnInit() {
    this.userService.GetFriends(this.currentUser()?.id!).subscribe({
      next: (object) => {
        this.userFriends.set(object);
      },
    });
  }
}
