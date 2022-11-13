import { map } from 'rxjs/operators';
import { PaginatedResult } from './../_models/pagination';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { User } from '../_models/user';


// const httpOptions = {
//   headers: new HttpHeaders({
//     'Authorization': 'Bearer ' + localStorage.getItem('token')
//   })
// }
@Injectable({
  providedIn: 'root'
})
export class UserService {

  baseUrl = environment.apiUrl;
constructor(private http:HttpClient) { }

  getUsers(pageNo?, itemsPerPage?, userParams?): Observable<PaginatedResult<User[]>>{
    //const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();
    //let httpParams = new HttpParams();
    let params: HttpParams;

    if(pageNo != null && itemsPerPage != null){
      // httpParams = httpParams.append("pageNumber", pageNo);
      // httpParams = httpParams.append("pageSize", itemsPerPage);
      params = this.addPaginationToParams(pageNo, itemsPerPage);
    }

    if(userParams != null){
      // httpParams = httpParams.append("minAge",userParams.minAge);
      // httpParams = httpParams.append("maxAge",userParams.maxAge);
      // httpParams = httpParams.append("gender",userParams.gender);
      // httpParams = httpParams.append("orderBy",userParams.orderBy);
      params = params.append("minAge",userParams.minAge);
      params = params.append("maxAge",userParams.maxAge);
      params = params.append("gender",userParams.gender);
      params = params.append("orderBy",userParams.orderBy);

    }
   
    return this.getPaginationResult<User[]>(this.baseUrl + 'user', params);
    // return this.http.get<User[]>(this.baseUrl + 'user',{observe : "response", params: httpParams})
    //   .pipe(
    //     map(response => {
    //       paginatedResult.result = response.body;
    //       if(response.headers.get('Pagination') != null)
    //           paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
    //       return paginatedResult
    //     })
    //   );
  }

  getUser(id): Observable<User>{
    return this.http.get<User>(this.baseUrl + 'user/'+ id);
  }

  updateUser(id, user:User){
    return this.http.put(this.baseUrl + 'user/'+ id, user);
  }

  setMainPhoto(userId: number, id:number){
    return this.http.post(this.baseUrl + 'user/' + userId + '/photos/' + id + '/ismain',{})
  }

  deletePhoto(userId:number, id:number){
    return this.http.delete(this.baseUrl + 'user/' + userId + '/photos/' + id);
  }

  addLike(username:string){
    return this.http.post(`${this.baseUrl}likes/${username}`,{});
  }

  getLikes(predicate:string, pageNumber, pageSize) : Observable<PaginatedResult<Partial<User[]>>>{
    let params = this.addPaginationToParams(pageNumber, pageSize);
    params = params.append('predicate', predicate);

    return this.getPaginationResult<Partial<User[]>>(this.baseUrl + 'likes', params);
  }

  private addPaginationToParams(pageNumber:number, pageSize:number){
    let httpParams = new HttpParams();
    httpParams = httpParams.append('pageNumber', pageNumber.toString());
    httpParams = httpParams.append('pageSize', pageSize.toString());

    return httpParams;
  }

  private getPaginationResult<T>(url:string, params):Observable<PaginatedResult<T>>{
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    return this.http.get<T>(url, {observe:'response', params}).pipe(
      map(response => {
        paginatedResult.result = response.body;
        if(response.headers.get('Pagination') != null)
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        return paginatedResult;
      })
    )
  }

}
