import { AuthService } from './../../_services/auth.service';
import { UserService } from './../../_services/user.service';
import { AlertifyService } from './../../_services/alertify.service';
import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/_models/user';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.scss']
})
export class MemberEditComponent implements OnInit {
  user:User;
  photoUrl: string
  
  @ViewChild('editForm', {static:true}) editForm: NgForm;
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event:any){
    if(this.editForm.dirty)
      $event.returnValue = true;
  }
  constructor(private router:ActivatedRoute, private alertify:AlertifyService,
    private userService:UserService, private authService:AuthService) { }

  ngOnInit() {
    this.router.data.subscribe(data =>{
      this.user = data['user'];
      
    })
    this.authService.currentPhotoUrl.subscribe(photoUrl =>{
      this.photoUrl = photoUrl;
    })
  }

  updateUser(){
   this.userService.updateUser(this.authService.decodedToken.nameid, this.user).subscribe(next =>{
    this.alertify.success("Profile Updated Successfully");
    this.editForm.reset(this.user);
   },error =>{
     this.alertify.error(error);
   })
    
  }

}
