import { take } from 'rxjs/operators';
import { BehaviorSubject } from 'rxjs';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { HttpClient } from '@angular/common/http';
import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { addPaginationToParams, getPaginationResult } from './paginationHelper';
import { Message } from '../_models/message';
import { Group } from '../_models/group';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  private baseUrl = environment.apiUrl;
  private hubUrl = environment.hubUrl;
  private hubConnection : HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([])
  messageThread$ = this.messageThreadSource.asObservable();
  constructor(private http:HttpClient) { }

  createConnection(user, otherUser: string){
    this.hubConnection = new HubConnectionBuilder()
        .withUrl(this.hubUrl + 'message?user=' + otherUser, {
          accessTokenFactory: () => user.token
        })
        .withAutomaticReconnect()
        .build()

    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on('ReceivedMessageThread', messageThread => {
      this.messageThreadSource.next(messageThread);
    })

    this.hubConnection.on('NewMessage', message => {
      this.messageThread$.pipe(take(1)).subscribe( messages =>{
        this.messageThreadSource.next([...messages, message])
      })
      
    })

    this.hubConnection.on('UpdatedGroup', (group:Group) => {
      if(group.connections.some(c => c.username === otherUser)){
        this.messageThread$.pipe(take(1)).subscribe(messages => {
          messages.forEach( message => {
            if(!message.dateRead){
              message.dateRead = new Date(Date.now())
            }
          })
          this.messageThreadSource.next([...messages]);
        })
      }
    })
  }

  stopHubConnection(){
    if(this.hubConnection){
      this.hubConnection.stop().catch(err => console.log(err));
    }
  }

  getMessages(pageNumber:number, pageSize:number, container:string){
    let params = addPaginationToParams(pageNumber, pageSize);
    params = params.append('container', container)
    return getPaginationResult<Message[]>(this.baseUrl + 'message', params, this.http);
  }

  getMessageThread(username:string){
    return this.http.get<Message[]>(this.baseUrl + `message/thread/${username}`);
  }

  async sendMessage(recipientUsername:string, content:string){
    return this.hubConnection.invoke('SendMessage',{content, recipientUsername})
            .catch(error => console.log(error));
  }

  deleteMessage(id){
    return this.http.delete(this.baseUrl + `message/${id}`);
  }
}
