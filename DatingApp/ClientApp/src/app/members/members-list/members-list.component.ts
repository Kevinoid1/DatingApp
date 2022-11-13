import { PaginatedResult, Pagination } from './../../_models/pagination';
import { AlertifyService } from '../../_services/alertify.service';
import { UserService } from '../../_services/user.service';
import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { ActivatedRoute } from '@angular/router';
import { PageChangedEvent } from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-members-list',
  templateUrl: './members-list.component.html',
  styleUrls: ['./members-list.component.scss']
})
export class MembersListComponent implements OnInit {
  users: User[];
  user: User = JSON.parse(localStorage.getItem('user')).userReturned;
  userParams:any = {};
  genderValues = [{value:"male", displayName:"Males"},{value:"female", displayName:"Females"}]
  pagination: Pagination;
  constructor(private userService:UserService, private alertify: AlertifyService,
     private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data =>{
      this.users = data['users'].result
      this.pagination=data['users'].pagination
    })
    this.userParams.gender = this.user.gender ==="male"? "female" : "male";
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.userParams.orderBy = "lastActive";

  }

  loadUsers(){
    this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, this.userParams)
      .subscribe((res : PaginatedResult<User[]>)=>{
        this.users = res.result;
        this.pagination = res.pagination;
      })
  }

  pageChanged(event: PageChangedEvent): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
    
  }

  resetFilters(){
    this.userParams.gender = this.user.gender ==="male"? "female" : "male";
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.userParams.orderBy = "lastActive";
    this.loadUsers();
  }

}
