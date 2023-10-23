import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../_models/User';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  //Due to strict mode we need to initialize the current user with | (union) and of operator with default value as null
  //currentUser$: Observable<User | null> = of(null)
 
  constructor(public accountService: AccountService) {

  }

  ngOnInit(): void {
    console.log('ng nav init');
    
  }

  login() {
    console.log(this.model);
    this.accountService.login(this.model)
      .subscribe({
        next: response => {
          console.log(response);
        },
        error: error => {
          console.log(error);
        }
      })

  }
  
  logout(){
    this.accountService.logout();
  }
}
