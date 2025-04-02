import { Component, computed } from '@angular/core';
import { ButtonComponent } from '../../components/general-components/button/button.component';
import { SessionService } from '../../services/session.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [ButtonComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent {
  constructor(private sessionService: SessionService, private router: Router) {}

  currentUser = computed(() => this.sessionService.currentUser());

  navigateToUserSelection() {
    this.router.navigate(['/userSelect']);
  }

  navigateToExpensePage() {
    this.router.navigate(['/expenses']);
  }
}
