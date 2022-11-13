import { Pagination } from './../_models/pagination';
import { UserService } from './../_services/user.service';
import { Component, OnInit } from '@angular/core';
import { User } from '../_models/user';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.scss']
})
export class ListsComponent implements OnInit {
  users: Partial<User[]> = [];
  predicate = 'liked';
  pageNumber = 1;
  pageSize = 5;
  pagination:Pagination;
  constructor(private userService: UserService) { }

  ngOnInit() {
    this.loadLikes();
  }

  loadLikes(){
    this.userService.getLikes(this.predicate, this.pageNumber, this.pageSize).subscribe(response =>{
      this.users = response.result;
      this.pagination = response.pagination;
    })
  }

  pageChanged(event){
    this.pageNumber = event.page;
    this.loadLikes();
  }
}
