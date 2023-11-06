import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-date-picker',
  templateUrl: './date-picker.component.html',
  styleUrls: ['./date-picker.component.css']
})
export class DatePickerComponent implements ControlValueAccessor {
@Input() label= '';
@Input() maxDate : Date | undefined;
//Make all property optional of date picker control
bsConfig : Partial<BsDatepickerConfig> | undefined

constructor(@Self() public ngControl: NgControl) {
  this.ngControl.valueAccessor = this;
    this.ngControl.valueAccessor = this;
    this.bsConfig = {
      containerClass :'theme-red',
      dateInputFormat : 'DD MMM YYYY'
    }
  }

  writeValue(obj: any): void {
  }
  registerOnChange(fn: any): void {
  }
  registerOnTouched(fn: any): void {
  }
  setDisabledState?(isDisabled: boolean): void {
  }
get control() : FormControl {
  return this.ngControl.control as FormControl
}
}
