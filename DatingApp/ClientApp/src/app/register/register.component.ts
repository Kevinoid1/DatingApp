import { AlertifyService } from './../_services/alertify.service';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  model:any ={};
  registerForm : FormGroup;
  //pass value from parent to child using input
  // @Input('valuesFromHome') homeValues;
  @Output() cancelRegister = new EventEmitter();
  constructor(private accountService:AccountService,
    private fb: FormBuilder,
    private alertify:AlertifyService) { }

  ngOnInit() {
    this.initRegisterForm();
  }

  initRegisterForm(){
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.passwordConfirmValidator('password')]]
    })
  }

  passwordConfirmValidator(key:string) : ValidatorFn{
    return (control : AbstractControl) => {
      return control.parent.controls[key].value === control.value ? null : {doesNotMatch: true}
    }
  }

  register(){
    this.accountService.register(this.model).subscribe((response)=>{
      //console.log(response);
      this.alertify.success("Registration was successful");
    },error =>{
      this.alertify.error(error);
      
    })
    
  }

  cancel(){
    this.cancelRegister.emit(false);
  }

}
