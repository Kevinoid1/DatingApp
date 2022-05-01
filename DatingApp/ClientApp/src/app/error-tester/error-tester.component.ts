import { environment } from './../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-error-tester',
  templateUrl: './error-tester.component.html',
  styleUrls: ['./error-tester.component.css']
})
export class ErrorTesterComponent implements OnInit {
  baseUrl = environment.apiUrl
  constructor(private http: HttpClient) { }

  ngOnInit() {
  }

  sendServerError(){
    this.http.get(`${this.baseUrl}buggy/server-error`).subscribe(r => {
      console.log(r)
    }, (err) => console.log(err))
  }

  sendBadRequest(){
    this.http.get(`${this.baseUrl}buggy/bad-request`).subscribe(r => {
      console.log(r)
    }, (err) => console.log(err))
  }

  sendUnauthorized(){
    this.http.get(`${this.baseUrl}buggy/auth`).subscribe(r => {
      console.log(r)
    }, (err) => console.log(err))
  }

  sendUnauth(){
    this.http.get(`${this.baseUrl}buggy/unauthorized`).subscribe(r => {
      console.log(r)
    }, (err) => console.log(err))
  }

  sendNotFound(){
    this.http.get(`${this.baseUrl}buggy/unauthorized`).subscribe(r => {
      console.log(r)
    }, (err) => console.log(err))
  }

  sendValidationError(){
    const user = {
      username : "",
      password : ""
    }


    this.http.post(`${this.baseUrl}auth/register`, user)
      .subscribe(r => console.log(r), (err) => console.log(err))
  }
}
