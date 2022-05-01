import { AccountService } from './../_services/account.service';
import { AlertifyService } from './../_services/alertify.service';
import { Component, OnInit } from '@angular/core';


import { Router } from '@angular/router';
import { take } from 'rxjs/operators';



@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit {
  model:any = {};
  photoUrl: string;
  isLoggedIn: boolean = false; // angular will initialize boolean variables to default values of false. I just did this here because i like it this way
  
  constructor(
     private alertify:AlertifyService, 
     private router: Router,
     public accountService: AccountService) { }

  ngOnInit() {
    this.accountService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
  }

  login(){
    this.accountService.login(this.model).subscribe(()=>{
      //console.log(this.authService.decodedToken);
      this.alertify.success("Logged in successfully");
    }, error =>{
      this.alertify.error(error);
    }, () => {
      this.router.navigate(['/members']);
    })
  }

  loggedIn(){
    return this.accountService.loggedIn();
  }

  logout(){
    // localStorage.removeItem('token');
    // localStorage.removeItem('userReturned');
    this.accountService.logout();
    this.accountService.decodedToken = null;
    //this.accountService.setCurrentUser(null);
    this.alertify.message("Logged out");
    this.router.navigate(['/']);
    
  }

  getCurrentUser(){
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.isLoggedIn = !!user; //the double exclamation mark turns an object to a boolean. Take note
    })
  }

}
