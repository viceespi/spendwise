import { Component, output, signal } from '@angular/core';
import { User } from '../../../models/User';
import { ButtonComponent } from '../../../components/general-components/button/button.component';
import { UsersService } from '../../../services/users.service';
import { Router } from '@angular/router';
import { InfoModalComponent } from '../../../components/general-components/info-modal/info-modal.component';
import { SessionService } from '../../../services/session.service';

@Component({
  selector: 'app-user-select-page',
  imports: [ButtonComponent],
  templateUrl: './user-select-page.component.html',
  styleUrl: './user-select-page.component.css',
  providers: [UsersService],
})
export class UserSelectPageComponent {
  constructor(
    private usersService: UsersService,
    private router: Router,
    private sessionService: SessionService
  ) {}

  usersList = signal<User[]>([]);

  ngOnInit() {
    this.usersService.GetUsers().subscribe({
      next: (response) => {
        let newUsersList: User[] = response;
        this.usersList.update((usersList) => newUsersList);
      },
    });
  }

  changeUser(newUser: User) {
    this.sessionService.ChangeUser(newUser);
  }

  goToExpensesPage() {
    this.router.navigate(['/expenses']);
  }
}
