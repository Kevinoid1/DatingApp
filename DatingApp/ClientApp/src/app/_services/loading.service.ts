import { NgxSpinnerService } from 'ngx-spinner';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {

  constructor(private spinnerService: NgxSpinnerService ) { }
  count: number = 0;
  show(){
    this.count++;
    this.spinnerService.show(undefined, {
      bdColor: 'rgba(255, 255, 255, 0)',
      size: 'large',
      color: '#ff8533',
      type: 'line-scale-pulse-out'
    })
  }

  hide(){
    this.count--;
    if (this.count <= 0) {
      this.count = 0;
    this.spinnerService.hide();
    }
  }
}
