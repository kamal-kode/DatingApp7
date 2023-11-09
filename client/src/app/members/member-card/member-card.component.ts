import { Component, Input } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent {
  @Input() member: Member | undefined;
  /**
   *
   */
  constructor(private memberService: MembersService, private tostr: ToastrService) {
  }

  addLike(member: Member) {
    console.log('addliek called');
    
    this.memberService.addLike(member.userName).subscribe({
      next: () => this.tostr.success('You have liked ' + member.knownAs)
    })
  }
}
