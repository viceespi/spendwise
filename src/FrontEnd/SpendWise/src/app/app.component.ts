import { Component, computed, Signal } from '@angular/core';
import { ButtonComponent } from './components/general-components/button/button.component';
import { RouterOutlet, Router, RouterLink } from '@angular/router';
import { SessionService } from './services/session.service';

@Component({
  selector: 'app-root',
  imports: [ButtonComponent, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  constructor(private router: Router, private sessionService: SessionService) {}

  title = 'SpendWise';

  currentUser = computed(() => this.sessionService.currentUser());

  navigateToUserInfo() {
    this.router.navigate(['/userInfo']);
  }

  navigateToHomePage() {
    this.router.navigate(['/home']);
  }
}
