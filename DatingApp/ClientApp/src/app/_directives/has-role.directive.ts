import { take } from 'rxjs/operators';
import { AccountService } from './../_services/account.service';
import { Directive, TemplateRef, ViewContainerRef, OnInit, Input } from '@angular/core';
import { User } from '../_models/user';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit{
  @Input() appHasRole: string[];
  user: User;
  constructor(private viewContainerRef: ViewContainerRef, private templateRef: TemplateRef<any>,
    private accountService: AccountService) { }
  
    ngOnInit(): void {
    this.accountService.currentUser$.pipe(take(1)).subscribe((user) => {
      this.user = user;
    })

    if(this.user == null || !this.user.roles){
      this.viewContainerRef.clear();
      return;
    }

    if(this.user.roles.some(r => this.appHasRole.includes(r)))
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    
    else
      this.viewContainerRef.clear();
  }

}
