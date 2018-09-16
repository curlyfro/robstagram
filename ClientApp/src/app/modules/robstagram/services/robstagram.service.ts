import { Injectable } from '@angular/core';
import { Profile } from '../models/profile.interface';
import { BaseService } from '../../../shared/services/base.service';
import { Observable } from 'rxjs/Rx'; 
import { Entry } from '../models/entry.interface';
import { HttpRequest, HttpClient, HttpHeaders, HttpEvent } from '@angular/common/http';
import { ConfigService } from '../../../shared/utils/config.service';

@Injectable()
export class RobstagramService extends BaseService {

  baseUrl: string = ''; 

  constructor(private http: HttpClient, private configService: ConfigService) {
    super();
    this.baseUrl = configService.getApiURI();
  }

  private getAuthorizedHttpHeaders(): HttpHeaders {
    let headers = new HttpHeaders();
    let authToken = localStorage.getItem('auth_token');
    headers = headers.append('Authorization', `Bearer ${authToken}`);
    return headers;
  }

  // GET api/robstagram/profile
  getProfile(): Observable<Profile> {
    let headers = this.getAuthorizedHttpHeaders();
    headers.append('Content-Type', 'application/json');
  
    return this.http.get<Profile>(this.baseUrl + "/robstagram/profile",
    {
      headers: headers
    })
    .catch(this.handleError);
  }

  // POST api/robstagram/entries
  postEntry(entry: { description: string, image: File}): Observable<HttpEvent<{}>> {
    let headers = this.getAuthorizedHttpHeaders();

    let formData = new FormData();
    formData.append("description", entry.description);
    formData.append("image", entry.image, entry.image.name);    

    // for (const prop in entry) {
    //   if (!entry.hasOwnProperty(prop)) { continue; }
    //   formData.append(prop, entry[prop])
    // }

    const uploadReq = new HttpRequest('POST', this.baseUrl + '/robstagram/entries', formData, {
      reportProgress: true, 
      headers: headers, 
      responseType: 'text'
    });

    return this.http.request(uploadReq);
  }

  // PUT api/robstagram/entries
  putEntry(image64: string, description: string): Observable<HttpEvent<{}>> {
    let headers = this.getAuthorizedHttpHeaders();

    let formData = new FormData();
    formData.append("image64", image64);
    formData.append("description", description);

    const uploadReq = new HttpRequest('PUT', this.baseUrl + '/robstagram/entries', formData, {
        reportProgress: true,
        headers: headers,
        responseType: 'text'
      });

    return this.http.request(uploadReq);
  }

  // GET api/robstagram/entries
  getEntries(): Observable<Entry[]> {
    let headers = this.getAuthorizedHttpHeaders();

    return this.http.get<Entry[]>(this.baseUrl + '/robstagram/entries', {
      headers: headers,
      responseType: 'json'
    });
  }

  // POST api/robstagram/entries/{id}/likes
  postLike(id: number): Observable<Entry> {
    let headers = this.getAuthorizedHttpHeaders();

    return this.http.post<Entry>(this.baseUrl + `/robstagram/likes/${id}`, null, {
      headers: headers,
      responseType: 'json'
    });
  }
}
