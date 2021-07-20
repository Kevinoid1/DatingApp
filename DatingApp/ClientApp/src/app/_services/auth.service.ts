import { JwtHelperService } from '@auth0/angular-jwt';


import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {map} from 'rxjs/operators'
import { User } from '../_models/user';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = 'https://localhost:44315/api/auth/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;
  photoUrl = new BehaviorSubject<string>('../../assets/user.png');
  currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private http:HttpClient) { }

  changeMemberPhoto(photoUrl: string){
    this.photoUrl.next(photoUrl);
  }

  login(model:any){
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map(response =>{
        const user:any =  response;
        if(user){
          localStorage.setItem('token', user.token);
          localStorage.setItem('userReturned', JSON.stringify(user.userReturned));
        this.decodedToken = this.jwtHelper.decodeToken(user.token);
        this.currentUser = user.userReturned;
        this.changeMemberPhoto(this.currentUser.photoUrl);
       
        
        }
          

      })
    )
  }

  register(model:any){
    return this.http.post(this.baseUrl + 'register', model);
  }

  loggedIn(){
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }

}
