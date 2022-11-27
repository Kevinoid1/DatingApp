import { PresenceService } from './presence.service';
import { map } from 'rxjs/operators';
import { environment } from './../../environments/environment';
import { User } from './../_models/user';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject, Observable, BehaviorSubject } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl
  currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  photoUrl = new BehaviorSubject<string>('../../assets/user.png');
  currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private http:HttpClient, private presenceService:PresenceService) { }

  login(model){
    return this.http.post(`${this.baseUrl}auth/login`, model).pipe(
      map((response: any) => {
        const user = response;
        if(user){
          this.doStuff(user);
          this.presenceService.createConnection(user);
        }
      })
    )
  }

  doStuff(user){
    this.decodedToken = JSON.parse(atob(user.token.split('.')[1]));
    user.roles = [];
    const roles = this.decodedToken.role;
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    localStorage.setItem('user', JSON.stringify(user));
    /*this observable created here never completes so remember to unsubscribe from 
    the observable when finished  otherwise can lead to memory leaks */
    this.currentUserSource.next(user);
    this.photoUrl.next(user.photoUrl);
    //this.decodedToken = this.jwtHelper.decodeToken(user.token);
  }

  changeMemberPhoto(photoUrl:string){
    this.photoUrl.next(photoUrl);
  }

  setCurrentUser(user){
    this.currentUserSource.next(user)
  }

  logout(){
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    this.presenceService.stopConnection();
  }

  register(model:any){
    return this.http.post(this.baseUrl + 'auth/register', model).pipe(
      map( (response: any) => {
        const user = response;
        if(user){
          this.doStuff(user);
          this.presenceService.createConnection(user);
        }
      })
    );
  }

  loggedIn(){
    const userAndToken = JSON.parse(localStorage.getItem('user'));
    if(userAndToken)
      return !this.jwtHelper.isTokenExpired(userAndToken.token);

    return false;
  }
}
