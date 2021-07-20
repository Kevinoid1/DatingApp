import { AlertifyService } from '../../_services/alertify.service';
import { UserService } from '../../_services/user.service';
import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-members-list',
  templateUrl: './members-list.component.html',
  styleUrls: ['./members-list.component.scss']
})
export class MembersListComponent implements OnInit {
  users: User[];
  constructor(private userService:UserService, private alertify: AlertifyService,
     private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data =>{
      this.users = data['users']
    })
  }

  // loadUsers(){
  //   this.userService.getUsers().subscribe((users : User[])=>{
  //     this.users = users;
  //   }, error => {
  //     this.alertify.error(error);
  //   })
  // }

}
