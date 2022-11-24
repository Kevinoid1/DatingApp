import { AccountService } from './_services/account.service';
import { JwtHelperService } from '@auth0/angular-jwt';

import { Component, OnInit } from '@angular/core';
import { User } from './_models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  jwtHelper = new JwtHelperService();

  constructor(private accountService: AccountService){

  }
  ngOnInit(): void {
    const userAndToken = JSON.parse(localStorage.getItem('user'));
    let token = null;
    let user: User = null;
    if(userAndToken){
       token = userAndToken.token;
       user = userAndToken;
    }
   
    if(token){
      this.accountService.decodedToken = this.jwtHelper.decodeToken(token);
    }
    if(user){
      //this.authService.currentUser = JSON.parse(user);
      this.accountService.setCurrentUser(user);
      this.accountService.changeMemberPhoto(user.photoUrl)
    }
  }
}
