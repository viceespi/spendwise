import { Routes } from '@angular/router';
import { ExpensesComponent } from './pages/expenses/expenses.component';
import { UserSelectPageComponent } from './pages/users/user-select-page/user-select-page.component';
import { HomeComponent } from './pages/home/home.component';
import { RouteGuardService } from './services/Auth/route-guard.service';
import { UserInfoComponent } from './pages/users/user-info/user-info.component';

export const routes: Routes = [
  {
    path: 'expenses',
    component: ExpensesComponent,
    canActivate: [RouteGuardService],
  },
  { path: 'home', component: HomeComponent },
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'userSelect', component: UserSelectPageComponent },
  { path: 'userInfo', component: UserInfoComponent },
];
