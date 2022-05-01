import { DateInputComponent } from './_formComponents/date-input/date-input.component';
import { TextInputComponent } from './_formComponents/text-input/text-input.component';
import { LoadingInterceptorProvider } from './_services/loading.interceptor';
import { SharedModule } from './modules/shared/shared.module';
import { BrowserModule, HammerGestureConfig, HAMMER_GESTURE_CONFIG } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
// import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
 import { RouterModule } from '@angular/router';
import { JwtModule } from '@auth0/angular-jwt';
// import { TabsModule } from 'ngx-bootstrap/tabs';
// import { NgxGalleryModule } from 'ngx-gallery';
// import { FileUploadModule } from 'ng2-file-upload';

import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { ErrorInterceptorProvider } from './_services/error.interceptor';
import { MembersListComponent } from './members/members-list/members-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { MemberCardComponent } from './members/member-card/member-card.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes-guard';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { appRoutes } from './routes';
import { PhotoEditComponent } from './members/photo-edit/photo-edit.component';
//import { ButtonsModule, PaginationModule } from 'ngx-bootstrap';
import { ErrorTesterComponent } from './error-tester/error-tester.component';

export function tokenGetter(){
  const userAndToken = JSON.parse(localStorage.getItem('user'));
  if(userAndToken)
    return userAndToken.token;

  return null;
}

export class CustomHammerConfig extends HammerGestureConfig  {
  overrides = {
      pinch: { enable: false },
      rotate: { enable: false }
  };
}

@NgModule({
  declarations: [						
    AppComponent,
    HomeComponent,
    NavComponent,
    HomeComponent,
    RegisterComponent,
      MembersListComponent,
      MessagesComponent,
      ListsComponent,
      MemberCardComponent,
      MemberDetailComponent,
      MemberEditComponent,
      PhotoEditComponent,
      ErrorTesterComponent,
      TextInputComponent,
      DateInputComponent
      
   ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    RouterModule.forRoot(appRoutes),
    SharedModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    JwtModule.forRoot({
      config:{
        tokenGetter: tokenGetter,
        allowedDomains: ['localhost:44315'],
        disallowedRoutes:['https://localhost:44315/api/auth']
      }
    }),

    
    // RouterModule.forRoot([
    //   { path: '', component: HomeComponent, pathMatch: 'full' },
    //   { path: 'counter', component: CounterComponent },
    //   { path: 'fetch-data', component: FetchDataComponent },
    // ])
  ],
  providers: [
    //interceptors
    ErrorInterceptorProvider,
    LoadingInterceptorProvider,

    //resolvers
    MemberDetailResolver,
    MemberListResolver,
    MemberEditResolver,

    //config fix for ngx bootstrap bug
    { provide: HAMMER_GESTURE_CONFIG, useClass: CustomHammerConfig },

  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
