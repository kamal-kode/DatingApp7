import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, of, take } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { User } from '../_models/user';
import { getPaginatedResult, getPaginationHeaders } from './PaginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();
  userParams: UserParams | undefined;
  user: User | undefined

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) {
          this.userParams = new UserParams(user)
          this.user = user;
        }
      }
    })
  }

  getUserParams() {
    return this.userParams;
  }

  setUserParams(params: UserParams) {
    // if (params.pageNumber !== this.userParams?.pageNumber) params.pageNumber = 1;
    this.userParams = params;
    console.log('mS', this.userParams);

  }

  resetUserPrams() {
    if (this.user) {
      console.log('this.user', this.user);

      this.userParams = new UserParams(this.user)
      console.log('this.userParams', this.userParams);

      return this.userParams;
    }
    return
  }
  getMembers(userParams: UserParams) {
    // console.log(Object.values(userParams).join('-'));
    const response = this.memberCache.get(Object.values(userParams).join('-'));

    if (response) return of(response);

    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http)
      .pipe(map(
        response => {
          console.log(JSON.stringify(response));
          this.memberCache.set(Object.values(userParams).join('-'), response);
          return response;
        }
      ))
  }
  
  //Get member from Cache
  getMember(username: string) {
    const member = [...this.memberCache.values()]
      .reduce((arr, currElement) => arr.concat(currElement.result), [])
      .find((_member: Member) => _member.userName === username)

    console.log('member', member);
    if (member) return of(member);

    return this.http.get<Member>(this.baseUrl + 'users/' + username)
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        //Update the latest member to static members property. now members will be sync with the latest updated details
        const index = this.members.indexOf(member);
        this.members[index] = { ...this.members[index], ...member }
      })
    );
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {});
  }
  getLikes(predicate: string, pageNumber : number, pageSize : number) {
    let params= getPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);
    return getPaginatedResult<Member[]>(this.baseUrl + 'likes', params, this.http);
    // return this.http.get<Member[]>(this.baseUrl + 'likes?predicate=' + predicate);
  }
}
