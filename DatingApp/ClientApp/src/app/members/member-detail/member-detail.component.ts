import { take } from "rxjs/operators";
import { AccountService } from "./../../_services/account.service";
import { PresenceService } from "./../../_services/presence.service";
import { MessageService } from "./../../_services/message.service";
import { AlertifyService } from "./../../_services/alertify.service";
import { UserService } from "./../../_services/user.service";
import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { User } from "src/app/_models/user";
import {
  NgxGalleryAnimation,
  NgxGalleryImage,
  NgxGalleryOptions,
} from "ngx-gallery";
import { TabDirective, TabsetComponent } from "ngx-bootstrap";
import { Message } from "src/app/_models/message";

@Component({
  selector: "app-member-detail",
  templateUrl: "./member-detail.component.html",
  styleUrls: ["./member-detail.component.scss"],
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  user: User;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  @ViewChild("memberDetailTabset", { static: true }) tabset: TabsetComponent;
  messages: Message[] = [];
  activeTab: TabDirective;
  member: any;

  constructor(
    public presenceService: PresenceService,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private accountService: AccountService,
    private router: Router
  ) {
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.user = data["user"];
    });

    this.accountService.currentUser$.pipe(take(1)).subscribe((user) => {
      this.member = user;
    });

    this.route.queryParams.subscribe((params) => {
      params.tab ? this.selectTab(params.tab) : this.selectTab(0);
    });

    this.galleryOptions = [
      {
        width: "500px",
        height: "500px",
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false,
      },
    ];
    this.galleryImages = this.getImages();
  }

  getImages() {
    const imageUrls = [];
    for (const photo of this.user.photos) {
      imageUrls.push({
        small: photo.url,
        medium: photo.url,
        big: photo.url,
        description: photo.description,
      });
    }
    return imageUrls;
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === "Messages")
      this.messageService.createConnection(this.member, this.user.username);
    else this.messageService.stopHubConnection();
  }

  loadMessages() {
    this.messageService
      .getMessageThread(this.user.username)
      .subscribe((res) => {
        this.messages = res;
      });
  }

  selectTab(tabIndex) {
    this.tabset.tabs[tabIndex].active = true;
    if(tabIndex == 3) this.messageService.createConnection(this.member, this.user.username)
  }
  // loadUser(){
  //   this.userService.getUser(+this.router.snapshot.params['id']).subscribe(user =>{
  //     this.user = user;
  //   }, error =>{
  //     this.alertify.error(error);
  //   })
  // }
}
