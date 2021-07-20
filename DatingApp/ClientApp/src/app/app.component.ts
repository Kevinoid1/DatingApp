import { JwtHelperService } from '@auth0/angular-jwt';

import { Component, OnInit } from '@angular/core';
import { AuthService } from './_services/auth.service';
import { User } from './_models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  jwtHelper = new JwtHelperService();

  constructor(private authService:AuthService){

  }
  ngOnInit(): void {
    const token = localStorage.getItem('token');
    const user = localStorage.getItem('userReturned');
    if(token){
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
    }
    if(user){
      this.authService.currentUser = JSON.parse(user);
      this.authService.changeMemberPhoto(this.authService.currentUser.photoUrl)
    }
  }
}
