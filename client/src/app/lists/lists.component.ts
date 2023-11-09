import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { MembersService } from '../_services/members.service';
import { Pagination } from '../_models/pagination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  members: Member[] | undefined;
  predicate = 'liked';
  pageNumber = 1;
  pageSize = 5;
  pagination : Pagination | undefined
  /**
   *
   */
  constructor(private memberService: MembersService) {
  }
  ngOnInit(): void {
    this.loadLikes();
  }
  loadLikes() {
  console.log(this.predicate);
  
    this.memberService.getLikes(this.predicate, this.pageNumber, this.pageSize)
      .subscribe({
        next: (response) => {
          console.log(response);
          this.members= response.result
          this.pagination = response.pagination;
        }
      })
  }

  pageChanged(event : any){
    if(this.pageNumber !== event.page){
      this.pageNumber = event.page;
      this.loadLikes();
    }
  }
}
