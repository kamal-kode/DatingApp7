import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';

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
  //Reactive Form
  registerForm: FormGroup = new FormGroup({});
  maxDate : Date = new Date();
  validationsErrors : string[] | undefined
  constructor(private accountService: AccountService, private toastr: ToastrService , private formBuilder : FormBuilder , private router : Router) { }

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);

  }

  initializeForm() {
    this.registerForm = this.formBuilder.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]]
    });

    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () =>
        this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }


  //Confirm password validation
  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value
        ? null : { notMatching: true }
    }
  }

  register() {
    console.log(this.registerForm?.value);
    const dob=this.getDateOnly(this.registerForm.controls['dateOfBirth'].value);
    const formValues= {...this.registerForm.value, dateOfBirth : dob}
    this.accountService.register(formValues).subscribe(
      {
        next: response => {
          console.log(response);
          this.cancel();
          this.router.navigateByUrl('/members')
        },
        error: error => {
          console.log(error)
          this.validationsErrors = error
        }
      }
    )
  }

  cancel() {
    console.log('cancel');
    this.cancelRegister.emit(false);
  }

  private getDateOnly(dob: string | undefined){
    if (!dob) return;
    let theDob=new Date(dob);
    return new Date(theDob.setMinutes(theDob.getMinutes() - theDob.getTimezoneOffset()))
           .toISOString().slice(0, 10);
  }

}
