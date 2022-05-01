import { Component, forwardRef, Input, OnInit, Self } from '@angular/core';
import { ControlValueAccessor, NgControl, NG_VALUE_ACCESSOR } from '@angular/forms';

// export const textValueAccessor = {
//   provide : NG_VALUE_ACCESSOR,
//   useExisting: forwardRef(() => TextInputComponent),
//   multi: true
// }

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css'],
  // providers: [textValueAccessor]
})

export class TextInputComponent implements ControlValueAccessor {
  @Input() type:string;
  @Input() placeholder: string;
  @Input() name: string

    
  constructor(@Self() public ngControl: NgControl) { 
    this.ngControl.valueAccessor = this;
  }
  // constructor(){}

  writeValue(obj: any): void {
  }
  registerOnChange(fn: any): void {
  }
  registerOnTouched(fn: any): void {
  }

}
