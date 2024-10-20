import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { Router, RouterModule } from '@angular/router';
import { MatListModule } from '@angular/material/list';
import { AuthService } from '../services/auth.service';
import { Observable, Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';

@Component({
  selector: 'app-main-layout',
  templateUrl: './main-layout.component.html',
  styleUrls: ['./main-layout.component.css'],
  standalone: true,
  
  imports: [CommonModule, RouterModule, MatToolbarModule, MatButtonModule, MatSidenavModule, MatListModule, MatIconModule]
})
export class MainLayoutComponent implements OnInit, OnDestroy {
  isLoggedIn$: Observable<boolean> = new Observable();
  isCustomer: boolean = false;
  isManager: boolean = false;
  userID: string | null = null;
  private authSubscription: Subscription | undefined;
  constructor(private authService: AuthService, private router: Router) {
    this.isLoggedIn$ = this.authService.isLoggedIn$;
  }
  ngOnInit() {
    this.authSubscription = this.authService.currentUser.subscribe(user => {
    
    if(user){
      this.isCustomer = this.authService.isCustomer();
      this.isManager = this.authService.isManager();
      this.userID= this.authService.name || '';
    } else {
      this.userID = null;
        this.isCustomer = false;
        this.isManager = false;
    }})
  }

  ngOnDestroy() {
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
  }
  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}