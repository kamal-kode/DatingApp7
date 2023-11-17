import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();

  constructor(private tostr: ToastrService, private router : Router) { }

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();
    this.hubConnection.start().catch(error => {
      console.log(error);
    })
    this.hubConnection.on('UserIsOnline', username => {
      this.tostr.info(username + ' has connected')
      this.onlineUsers$.pipe(take(1)).subscribe({
        next : usernames => {
          this.onlineUsersSource.next([...usernames, username])
        }
      })
    })
    this.hubConnection.on('UserIsOffline', username => {
      this.tostr.warning(username + ' has disconnected')
      this.onlineUsers$.pipe(take(1)).subscribe({
        next : usernames => {
          this.onlineUsersSource.next(usernames
            .filter( u => u !== username))
        }
      })

    })
    this.hubConnection.on('GetOnlineUsers', username => {
      this.onlineUsersSource.next(username);
    })
    this.hubConnection.on('NewMessageReceived',({username, knownAs}) =>{
      this.tostr.info(knownAs + ' has sent you a new message! Click me to see it')
      .onTap
      .pipe(take(1))
      .subscribe({
        next : () => this.router.navigateByUrl('/members/'+ username + '?tab=Messages')
      });
    })
  }
  
  stopHubConnection() {
    this.hubConnection?.stop().catch(error => {
      console.log(error);
    })
  }
}
