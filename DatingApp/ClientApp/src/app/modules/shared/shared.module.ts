import { ReactiveFormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule, ButtonsModule, PaginationModule, TabsModule } from 'ngx-bootstrap';
import { JwtModule } from '@auth0/angular-jwt';
import { NgxGalleryModule } from 'ngx-gallery';
import { FileUploadModule } from 'ng2-file-upload';
import { NgxSpinnerModule } from 'ngx-spinner';



@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    BsDropdownModule.forRoot(),
    TabsModule.forRoot(),
    NgxGalleryModule,
    FileUploadModule,
    PaginationModule.forRoot(),
    ButtonsModule.forRoot(),
    NgxSpinnerModule,
    ReactiveFormsModule
    
  ],
  exports:[
    BsDropdownModule,
    JwtModule,
    TabsModule,
    NgxGalleryModule,
    FileUploadModule,
    PaginationModule,
    ButtonsModule,
    NgxSpinnerModule,
  ]
})
export class SharedModule { }
