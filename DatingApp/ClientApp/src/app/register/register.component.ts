import { AlertifyService } from './../_services/alertify.service';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // model:any ={};
  registerForm : FormGroup;
  maxDate: Date;
  //pass value from parent to child using input
  // @Input('valuesFromHome') homeValues;
  @Output() cancelRegister = new EventEmitter();
  constructor(private accountService:AccountService,
    private fb: FormBuilder,
    private alertify:AlertifyService) { 
      this.maxDate = new Date();
      this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
    }

  ngOnInit() {
    this.initRegisterForm();
  }

  initRegisterForm(){
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required]],
      knownAs: ['', Validators.required],
      gender: ['male'],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
    }, {validators: this.passwordConfirmValidator()})
  }

  // passwordConfirmValidator(key:string) : ValidatorFn{
  //   return (control : AbstractControl) => {
  //     return control.value === control.parent.controls[key].value ? null : {doesNotMatch: true}
  //   }
  // }

  passwordConfirmValidator() : ValidatorFn{
    return (group:AbstractControl) => {
      let password = group.get('password').value;
      let confirmPassword = group.get('confirmPassword').value;
      return password === confirmPassword ? null : {doesNotMatch: true}
    } 
  }

  onPasswordChange(){
    let passOne = this.registerForm.get('password');
    let passTwo = this.registerForm.get('confirmPassword');

    if(passOne.value !== passTwo.value)
      passTwo.setErrors({doesNotMatch : true})
  }

  register(registerForm){
    this.accountService.register(registerForm.value).subscribe((response)=>{
      //console.log(response);
      this.alertify.success("Registration was successful");
    })
    
  }

  cancel(){
    this.cancelRegister.emit(false);
  }

}
