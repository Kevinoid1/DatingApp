import { User } from './../../_models/user';
import { AccountService } from './../../_services/account.service';
import { AlertifyService } from './../../_services/alertify.service';
import { UserService } from './../../_services/user.service';
import { environment } from './../../../environments/environment';

import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { Photo } from 'src/app/_models/photo';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-photo-edit',
  templateUrl: './photo-edit.component.html',
  styleUrls: ['./photo-edit.component.scss']
})
export class PhotoEditComponent implements OnInit {
  @Input('photos')photos:Photo[];

  uploader:FileUploader;
  hasBaseDropZoneOver:boolean = false;
  baseUrl = environment.apiUrl;
  user: User;
  
  constructor (private accountService: AccountService, private userService:UserService,
    private alertify:AlertifyService){
    
  }

  ngOnInit() {
    this.initiator();
  }

  fileOverBase(e:any):void {
    this.hasBaseDropZoneOver = e;
  }

  initiator(){
    this.uploader = new FileUploader({
      url: this.baseUrl + 'user/' + this.accountService.decodedToken.nameid +'/photos',
      authToken: 'Bearer ' + JSON.parse(localStorage.getItem('user')).token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10*1024*1024
    })
    this.uploader.onAfterAddingFile = (file) => {file.withCredentials = false}

    this.uploader.onSuccessItem = (item, response, status, headers)=>{
        if(response){
          const res:Photo = JSON.parse(response);
          const photo = {
            id: res.id,
            url: res.url,
            isMain: res.isMain,
            description: res.description,
            dateAdded: res.dateAdded
          };
          this.photos.push(photo)
        }
    }
  }

  setMainPhoto(photo:Photo){
    let currentMainPhoto:Photo =  this.photos.filter(p => p.isMain == true)[0];
    this.userService.setMainPhoto(this.accountService.decodedToken.nameid, photo.id).subscribe(
      () => {
          currentMainPhoto.isMain = false;
          photo.isMain = true;

          //using behavior subject
          this.accountService.changeMemberPhoto(photo.url);
          this.accountService.currentUser$.pipe(take(1)).subscribe(x => {
            this.user = x;
          })
          this.user.photoUrl = photo.url;
          this.accountService.setCurrentUser(this.user);
          //localStorage.setItem('userReturned', JSON.stringify(this.authService.currentUser));

          //the way i did any to any communication
          // this.authService.currentUser.photoUrl = photo.url
          // localStorage.setItem('userReturned', JSON.stringify(this.authService.currentUser));
      }
    )
  }

  deletePhoto(id){
    this.alertify.confirm("Are you sure you want to delete this photo?", () =>{
      this.userService.deletePhoto(this.user.id, id).subscribe(()=>{
          this.photos.splice(this.photos.findIndex( p => p.id == id), 1);
          this.alertify.success("Photo was deleted successfully")
      })
    })
    
  }

  

}
