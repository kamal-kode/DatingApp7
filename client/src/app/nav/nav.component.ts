import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})

export class NavComponent implements OnInit {
  model: any = {};
  //Due to strict mode we need to initialize the current user with | (union) and of operator with default value as null
  //currentUser$: Observable<User | null> = of(null)

  constructor(public accountService: AccountService, 
    private router: Router, private toastr : ToastrService) {
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
          this.router.navigateByUrl('/members')
        }
        // ,error: error => {
        //   console.log(error);
        //   this.toastr.error(error.error)
        // }
      })
  }

  logout() {
    this.accountService.logout();
    //Home page 
    this.router.navigateByUrl('/')
  }
}
