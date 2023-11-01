import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  member: Member | undefined;
  user: User | null = null;
// get form access. Template is the child of component
@ViewChild('editForm') editForm : NgForm | undefined

// If user after making the form changes clicked on outside the application like browser history back/forward or type any 
// address in url showing confirm message
@HostListener('window:beforeunload', ['$event']) unloadNotification($event: any){
  if(this.editForm?.dirty) {
    console.log('called');
    $event.returnValue = true;
  }
}
  constructor(private accountService: AccountService, private memberService: MembersService
    , private toastr : ToastrService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user
    })
  }
  ngOnInit(): void {
    this.loadMember();
  }

  loadMember(){
    if(!this.user) return;
    this.memberService.getMember(this.user.userName).subscribe({
      next : member => this.member = member
    })
  }

  updateMember(){
    //We are using ngModel its two way binding so the member will receive all the updated value from ui
    console.log(this.member);
    this.memberService.updateMember(this.editForm?.value).subscribe({
      next : _ =>{
         //Reset the form to current state
        this.editForm?.reset(this.member);
        this.toastr.success('Profile updated successfully')
      }
    })

   
  }
}
