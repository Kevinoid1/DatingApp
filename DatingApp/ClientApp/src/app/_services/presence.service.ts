import { take } from 'rxjs/operators';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { AlertifyService } from './alertify.service';
import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { User } from '../_models/user';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl
  private hubConnection: HubConnection;
  private onlineUsersSouce = new BehaviorSubject<string[]>([]);
  onlineUser$ = this.onlineUsersSouce.asObservable();

  constructor(private alertify: AlertifyService,
              private toastr: ToastrService,
              private router: Router) { }

  createConnection(user){
    this.hubConnection = new HubConnectionBuilder()
        .withUrl(this.hubUrl + 'presence', {
          accessTokenFactory: () => user.token
        })
        .withAutomaticReconnect()
        .build();

    this.hubConnection.start().catch(error => console.log(error))

    this.hubConnection.on('UserIsOnline', (username) => {
      this.onlineUser$.pipe(take(1)).subscribe( usernames => {
        this.onlineUsersSouce.next([...usernames, username])
      })
    });

    this.hubConnection.on('UserIsOffline', (username) =>{
      this.onlineUser$.pipe(take(1)).subscribe( usernames => {
        this.onlineUsersSouce.next([...usernames.filter(x => x !== username)])
      })
    })

    this.hubConnection.on('GetAllOnlineUsers', (usernames:string[]) =>{
      this.onlineUsersSouce.next(usernames);
    })

    this.hubConnection.on('NewMessageReceived', ({userId, knownAs}) => {
      this.toastr.info(knownAs + ' has sent you a message')
        .onTap.pipe(take(1)).subscribe(() => {
          this.router.navigateByUrl('/members/'+userId + '?tab=3')
        })
    })
  }

  stopConnection(){
    this.hubConnection.stop().catch(error => console.log(error));
  }
}
