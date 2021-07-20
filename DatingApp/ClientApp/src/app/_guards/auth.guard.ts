import { AlertifyService } from './../_services/alertify.service';
import { AuthService } from './../_services/auth.service';
import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService:AuthService, private router:Router, private alertify:AlertifyService){}
  canActivate(): boolean {
    if(this.authService.loggedIn())
      return true;
    this.router.navigate(['/']);
    this.alertify.error('You must be logged in to use the app')
    return false;
  }
  
}
