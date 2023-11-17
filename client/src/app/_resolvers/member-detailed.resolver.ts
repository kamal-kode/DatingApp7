import { ResolveFn } from '@angular/router';
import { Member } from '../_models/member';
import { inject } from '@angular/core';
import { MembersService } from '../_services/members.service';

export const memberDetailedResolver: ResolveFn<Member> = (route, state) => {
  const memberService = inject(MembersService);
  console.log('mdr' , route.paramMap.get('username'));
  
  return memberService.getMember(route.paramMap.get('username')!)
};
