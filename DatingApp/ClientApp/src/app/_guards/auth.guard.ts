import { AccountService } from './../_services/account.service';
import { AlertifyService } from './../_services/alertify.service';
import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private accountService:AccountService, private router:Router, private alertify:AlertifyService){}
  canActivate(): boolean {
    if(this.accountService.loggedIn())
      return true;
    this.router.navigate(['/']);
    this.alertify.error('You must be logged in to use the app')
    return false;
  }
  
}
