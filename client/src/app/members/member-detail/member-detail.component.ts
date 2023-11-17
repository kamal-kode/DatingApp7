import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/message';
import { PresenceService } from 'src/app/_services/presence.service';
import { AccountService } from 'src/app/_services/account.service';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [CommonModule, TabsModule, GalleryModule, TimeagoModule, MemberMessagesComponent]
})
export class MemberDetailComponent implements OnInit , OnDestroy {
  //Get tab html access to component 
  @ViewChild('memberTabs', {static : true}) memberTabs?: TabsetComponent
  member: Member = {} as Member;
  images: GalleryItem[] = [];
  activeTab?: TabDirective
  messages: Message[] = []
  user?:User;
  constructor(private route: ActivatedRoute
    , private messageService: MessageService,
    public presenceService : PresenceService
    ,private accountService : AccountService) {
      this.accountService.currentUser$.subscribe({
        next : user =>{
          if(user)
          this.user = user;
        }
      })
  }
  ngOnDestroy(): void {
   this.messageService.stopHubConnection();
  }

  ngOnInit(): void {
    // Read data from root resolver. before loading the component 
    this.route.data.subscribe({
      next : data => this.member = data['member']
    })
    //Read data from query string
    this.route.queryParams.subscribe({
      next : params =>{
        console.log(params['tab']);
        console.log('md',this.member);
        params['tab'] && this.selectTab(params['tab'])
      }
    })
    this.getImages();
  }
  onTabActivated(data: TabDirective) {
    this.activeTab = data
    if (this.activeTab.heading === 'Messages' && this.user) {
      this.messageService.createHubConnection(this.user, this.member.userName);
    }
    else {
      this.messageService.stopHubConnection();
    }
  }

  loadMessages() {
    if (this.member) {
      this.messageService.getMessageThread(this.member.userName).subscribe({
        next: response => {
          this.messages = response
          console.log('messages', this.messages);

        }
      })
    }
  }
  //Select message tab on Messages button click
  selectTab(heading: string) {
    if (this.memberTabs) {
      this.memberTabs.tabs.find(t => t.heading === heading)!.active = true;
    }
  }
  getImages() {
    if (!this.member) return
    for (const photo of this.member?.photos) {
      this.images.push(new ImageItem({ src: photo.url, thumb: photo.url }))
    }
  }
}
