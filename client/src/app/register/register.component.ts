import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})

export class RegisterComponent implements OnInit {
  //Transfer data from parent to child. - Home to register
  // @Input() usersFromHomeComponent: any;
  //Child to parent - Register to home
  @Output() cancelRegister = new EventEmitter()
  model: any = {};
  constructor(private accountService: AccountService ,private toastr : ToastrService) {  }

  ngOnInit(): void {
  }

  register() {
    console.log(this.model);
    this.accountService.register(this.model).subscribe(
      {
        next: response => {
          console.log(response);
          this.cancel();
        },
        error: error => {
          this.toastr.error(error.error);
          console.log(error)
        }
      }
    )
  }

  cancel() {
    console.log('cancel');
    this.cancelRegister.emit(false);
  }

}
