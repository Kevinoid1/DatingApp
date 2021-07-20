import { AuthService } from './../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { UserService } from '../_services/user.service';
import { User } from 'src/app/_models/user';
import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve, Router, RouterStateSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()

export class MemberEditResolver implements Resolve<User>{

    constructor(private userService:UserService, private alertify: AlertifyService, 
        private router: Router, private authService: AuthService){}

    resolve(route: ActivatedRouteSnapshot): Observable<User>{
        return this.userService.getUser(+(this.authService.decodedToken.nameid)).pipe(
            catchError(error =>{
                this.alertify.error("A problem occured trying to retrieve the User!");
                this.router.navigate(['/members']);
                return of(null)
            })
        )
    }

}