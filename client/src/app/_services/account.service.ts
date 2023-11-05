import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BehaviorSubject, map } from 'rxjs';
import { User } from "../_models/user";
import { environment } from 'src/environments/environment';


@Injectable({
  providedIn: 'root' // Now no need to provide service in app module provider. This property automatically takes care
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentUserSource.asObservable();
  constructor(private http: HttpClient) { }

  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model)
      //Pipe is used for transform the data
      .pipe(
        map((response: User) => {
          const user = response;
          if (user) {
            this.setCurrentUser(user);
          }
        })
      );
  }

  register(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model)
      .pipe(
        map(user => {
          if (user) {
            this.setCurrentUser(user);
          }
          return user;
        })
      )
  }

  logout() {
    localStorage.removeItem('user')
    this.currentUserSource.next(null);
  }

  setCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }
}
