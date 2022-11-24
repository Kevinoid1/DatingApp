import { RolesModalComponent } from './../../modals/roles-modal/roles-modal.component';
import { User } from 'src/app/_models/user';
import { AdminService } from './../../_services/admin.service';
import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users: Partial<User[]>;
  bsModalRef: BsModalRef;
  constructor(private adminService : AdminService, private modalService: BsModalService) { }

  ngOnInit() {
    this.getUsersWithRoles();
  }

  getUsersWithRoles(){
    this.adminService.getUsersWithRoles().subscribe((users) => {
      this.users = users;
    })
  }

  openRolesModal(user: User){
    const config: ModalOptions = {
      class: 'modal-dialog-centered',
      initialState: {
        user,
        roles: this.getRoles(user) 
      }
    }
    this.bsModalRef = this.modalService.show(RolesModalComponent, config);
    this.bsModalRef.content.updateSelectedRoles.subscribe((values:any[]) => {
      const rolesToUpdate = {
        roles: [...values.filter(r => r.checked === true).map(r => r.name)]
      }
      if(rolesToUpdate){
        this.adminService.updateUserRoles(user.username, rolesToUpdate.roles).subscribe((result : string[]) =>{
          user.roles = [...result];
        })
      }
    });
  }

  private getRoles(user){
    const roles = [];
    const userRoles: string[] = user.roles;
    const availableRoles = [
      {name:"Admin"},
      {name: "Moderator"},
      {name: "Member"},
    ];

    availableRoles.forEach(role => {
      if(userRoles.includes(role.name)){
        role['checked'] = true;
        roles.push(role)
      } else{
        role['checked'] = false;
        roles.push(role)
      }
    })

    return roles;
  }

}
