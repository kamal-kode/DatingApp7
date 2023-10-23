import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ToastrModule } from 'ngx-toastr';

// Declare shared module here

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    BsDropdownModule.forRoot(),
    ToastrModule.forRoot({
      positionClass : 'toast-bottom-right'
    }),
  ],
  //This is required. Just need to pass module name
  exports:[
    BsDropdownModule,
    ToastrModule
  ]
})
export class SharedModule { }
