import { AdminGuard } from './_guards/admin.guard';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { ServerErrorComponent } from './server-error/server-error.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes-guard';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { AuthGuard } from './_guards/auth.guard';
import { Routes } from "@angular/router";

import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { MembersListComponent } from './members/members-list/members-list.component';
import { HomeComponent } from './home/home.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { ErrorTesterComponent } from './error-tester/error-tester.component';


export const appRoutes: Routes = [
    {path:'', component: HomeComponent},
    {
        path: '',
        canActivate: [AuthGuard],
        children: [
            {path: 'members/edit', component: MemberEditComponent,
             resolve:{user: MemberEditResolver }, canDeactivate: [PreventUnsavedChanges]},
            {path: 'members/:id', component: MemberDetailComponent, resolve:{user: MemberDetailResolver}},
            {path: 'members', component: MembersListComponent, resolve: {users: MemberListResolver}},
            {path: 'lists', component:ListsComponent},
            {path: 'messages', component: MessagesComponent},
            {path: 'admin', component: AdminPanelComponent, canActivate: [AdminGuard]}
        ],
        runGuardsAndResolvers: 'always'
    
    },
    {path: 'errors', component: ErrorTesterComponent},
    {path: 'not-found', component:NotFoundComponent},
    {path:'server-error', component:ServerErrorComponent},
    {path: '**', redirectTo:'/not-found', pathMatch:'full'}
    
]