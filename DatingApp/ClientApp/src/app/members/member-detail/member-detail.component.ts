import { MessageService } from './../../_services/message.service';
import { AlertifyService } from './../../_services/alertify.service';
import { UserService } from './../../_services/user.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/_models/user';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from 'ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap';
import { Message } from 'src/app/_models/message';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.scss']
})
export class MemberDetailComponent implements OnInit {
  user:User;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  @ViewChild('memberDetailTabset', {static: true}) tabset : TabsetComponent;
  messages:Message[] = [];
  activeTab: TabDirective;
  constructor(private userService:UserService, private alertify:AlertifyService,
    private router: ActivatedRoute, private messageService: MessageService) { }

  ngOnInit() {
    this.router.data.subscribe(data =>{
      this.user = data['user'];
    });

    this.router.queryParams.subscribe(params => {
      params.tab ? this.selectTab(params.tab) : this.selectTab(0);
    })

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ];
    this.galleryImages = this.getImages();
  }

  getImages(){
    const imageUrls = [];
    for (const photo of this.user.photos) {
      imageUrls.push({
        small: photo.url,
        medium: photo.url,
        big: photo.url,
        description: photo.description
      })
    }
    return imageUrls;
  }

  onTabActivated(data:TabDirective){
    this.activeTab = data;
    if(this.activeTab.heading === 'Messages' && this.messages.length === 0)
      this.loadMessages();
  }

  loadMessages(){
    this.messageService.getMessageThread(this.user.username).subscribe(res =>{
      this.messages = res;
    })
  }

  selectTab(tabIndex){
    this.tabset.tabs[tabIndex].active = true;
    if(this.messages.length === 0) this.loadMessages();
  }
  // loadUser(){
  //   this.userService.getUser(+this.router.snapshot.params['id']).subscribe(user =>{
  //     this.user = user;
  //   }, error =>{
  //     this.alertify.error(error);
  //   })
  // }
}
