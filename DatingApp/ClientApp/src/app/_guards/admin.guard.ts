import { map } from 'rxjs/operators';
import { AlertifyService } from './../_services/alertify.service';
import { AccountService } from './../_services/account.service';
import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {
  constructor(private accountService: AccountService, private alertify:AlertifyService){}
  canActivate(
    ): Observable<boolean> {
    return this.accountService.currentUser$.pipe(
      map((user) => {
        if(user.roles.includes("Admin") || user.roles.includes('Moderator'))
          return true

        this.alertify.error("You cannot access this area")
      })
      )
  }
  
}
