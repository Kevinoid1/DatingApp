import { HttpClient } from '@angular/common/http';
import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { addPaginationToParams, getPaginationResult } from './paginationHelper';
import { Message } from '../_models/message';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  private baseUrl = environment.apiUrl;
  constructor(private http:HttpClient) { }

  getMessages(pageNumber:number, pageSize:number, container:string){
    let params = addPaginationToParams(pageNumber, pageSize);
    params = params.append('container', container)
    return getPaginationResult<Message[]>(this.baseUrl + 'message', params, this.http);
  }

  getMessageThread(username:string){
    return this.http.get<Message[]>(this.baseUrl + `message/thread/${username}`);
  }

  sendMessage(recipientUsername:string, content:string){
    return this.http.post<Message>(this.baseUrl + 'message', {content, recipientUsername});
  }

  deleteMessage(id){
    return this.http.delete(this.baseUrl + `message/${id}`);
  }
}
