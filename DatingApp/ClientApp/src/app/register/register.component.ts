import { AlertifyService } from './../_services/alertify.service';
import { AuthService } from './../_services/auth.service';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  model:any ={};
  //pass value from parent to child using input
  // @Input('valuesFromHome') homeValues;
  @Output() cancelRegister = new EventEmitter();
  constructor(private authService:AuthService, private alertify:AlertifyService) { }

  ngOnInit() {
  }

  register(){
    this.authService.register(this.model).subscribe((response)=>{
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
