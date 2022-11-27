import { PresenceService } from './../../_services/presence.service';
import { AlertifyService } from './../../_services/alertify.service';
import { UserService } from './../../_services/user.service';
import { Component, Input, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.scss']
})
export class MemberCardComponent implements OnInit {
  @Input('user') user:User;
  constructor(
    private userService:UserService,
    private alertify:AlertifyService,
    public presenceService:PresenceService
  ) { }

  ngOnInit() {
  }

  addLike(user:User){
    this.userService.addLike(user.username).subscribe(() => {
      this.alertify.success(`You liked ${user.knownAs}`);
    })
  }

}
