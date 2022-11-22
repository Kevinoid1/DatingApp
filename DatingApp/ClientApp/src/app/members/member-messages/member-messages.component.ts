import { NgForm } from '@angular/forms';
import { MessageService } from './../../_services/message.service';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { Message } from 'src/app/_models/message';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() messages:Message[];
  @Input() username: string;
  messageContent: string;
  userId: number;
  @ViewChild('messageForm', {static:false}) messageForm : NgForm
  constructor(private messageService: MessageService) { 
    this.userId = JSON.parse(localStorage.getItem('user')).userReturned.id;
  }

  ngOnInit() {
  }

  sendMessage(){
    this.messageService.sendMessage(this.username, this.messageContent).subscribe(message => {
      this.messages.push(message);
      this.messageForm.reset();
    })
  }

  deleteMessage(id: number){
    this.messageService.deleteMessage(id).subscribe(() => {
      this.messages.splice(this.messages.findIndex(m => m.id === id), 1);
    })
  }
}
