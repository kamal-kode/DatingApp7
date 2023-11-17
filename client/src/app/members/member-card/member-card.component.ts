import { Component, Input } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { PresenceService } from 'src/app/_services/presence.service';

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
  constructor(private memberService: MembersService
    , private tostr: ToastrService
    //If public we can use this in template and use asyc pipe.
    , public presenceService: PresenceService) {
    console.log('mcard', this.member);

  }

  addLike(member: Member) {
    console.log('addliek called');

    this.memberService.addLike(member.userName).subscribe({
      next: () => this.tostr.success('You have liked ' + member.knownAs)
    })
  }
}
