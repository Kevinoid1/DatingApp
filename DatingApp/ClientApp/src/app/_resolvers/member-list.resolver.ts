import { AlertifyService } from '../_services/alertify.service';
import { UserService } from '../_services/user.service';
import { User } from 'src/app/_models/user';
import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve, Router, RouterStateSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()

export class MemberListResolver implements Resolve<User[]>{
    pageNumber = 1;
    pageSize = 5;

    constructor(private userService:UserService, private alertify: AlertifyService, 
        private router: Router){}
    resolve(route: ActivatedRouteSnapshot): Observable<User[]>{
        return this.userService.getUsers(this.pageNumber, this.pageSize).pipe(
            catchError(error =>{
                this.alertify.error("A problem occured!");
                this.router.navigate(['/home']);
                return of(null)
            })
        )
    }

}