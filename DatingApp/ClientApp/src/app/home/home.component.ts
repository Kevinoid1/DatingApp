import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;
  // values:any;
  constructor(private http:HttpClient) { }

  ngOnInit() {
    // this.getvalues();
  }

  registerToggler(){
    this.registerMode = true;
  }

  // getvalues(){
  //   this.http.get('https://localhost:44315/api/weatherforecast/view').subscribe(response =>{
  //       this.values = response;
  //   })
  // }

  registerModeCancel(registerMode:boolean){
    this.registerMode=registerMode;
  }
}
