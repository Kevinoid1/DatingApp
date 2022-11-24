import { NgForm } from '@angular/forms';
import { MessageService } from './../../_services/message.service';
import { AfterViewChecked, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { Message } from 'src/app/_models/message';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit, AfterViewChecked {
  @Input() messages:Message[];
  @Input() username: string;
  messageContent: string;
  userId: number;
  messageSent = true;
  @ViewChild('messageForm', {static:false}) messageForm : NgForm
  @ViewChild('messageBox', {static: false}) messageBox: ElementRef;
  constructor(private messageService: MessageService) { 
    this.userId = JSON.parse(localStorage.getItem('user')).id;
  }
  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }

  ngOnInit() {
  }

  sendMessage(){
    this.messageService.sendMessage(this.username, this.messageContent).subscribe(message => {
      this.messages.push(message);
      this.messageForm.reset();
      this.messageSent = true;
    })
  }

  scrollToBottom(){
    if(this.messageBox && this.messageSent){
      this.messageBox.nativeElement.scrollTop = this.messageBox.nativeElement.scrollHeight;
      this.messageSent = false;
    }
  }

  deleteMessage(id: number){
    this.messageService.deleteMessage(id).subscribe(() => {
      this.messages.splice(this.messages.findIndex(m => m.id === id), 1);
    })
  }
}
