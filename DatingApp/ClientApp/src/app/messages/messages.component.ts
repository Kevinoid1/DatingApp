import { AuthService } from './../_services/auth.service';
import { MessageService } from './../_services/message.service';
import { Pagination } from './../_models/pagination';
import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.scss']
})
export class MessagesComponent implements OnInit {
  messages:Message[];
  pagination: Pagination;
  pageNumber = 1;
  pageSize = 5;
  container = 'Unread';
  userId: number;
  constructor(private messageService:MessageService) { 
    this.userId = JSON.parse(localStorage.getItem('user')).id;
  }

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages(){
    this.messageService.getMessages(this.pageNumber, this.pageSize, this.container).subscribe(response => {
      this.messages = response.result;
      this.pagination = response.pagination;
    })
  }

  pageChanged(event:any){
    this.pageNumber = event.page;
    this.loadMessages();
  }

}
