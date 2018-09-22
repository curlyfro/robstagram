import { Injectable } from '@angular/core';
import { BaseService } from './base.service';
import { Observable } from 'rxjs';
import { HttpEvent, HttpHeaders, HttpRequest, HttpClient } from '@angular/common/http';

@Injectable()
export class UploadService extends BaseService {

  constructor(private http: HttpClient) {
    super();
   }

   private getAuthorizedHeaders(): HttpHeaders {
     let headers = new HttpHeaders();
     headers = headers.append('Authorization', `Bearer ${localStorage.getItem('auth_token')}`);
     return headers;
   }

   uploadSingle(file: File): Observable<HttpEvent<{}>> {
    const headers = this.getAuthorizedHeaders();

    let data = new FormData();
    data.append('file', file, file.name);

    const req = new HttpRequest('POST', '/api/upload/single', data, {
      reportProgress: true,
      headers: headers,
      responseType: 'json'
    });

    return this.http.request(req);
   }

   uploadMultiple(files: FileList): Observable<HttpEvent<{}>> {
    const headers = this.getAuthorizedHeaders();

    let data = new FormData();
    for (const file in files) {
      data.append('files', file);
    }
    
    const req = new HttpRequest('POST', '/api/upload/multiple', data, {
      reportProgress: true,
      headers: headers,
      responseType: 'json'
    });

    return this.http.request(req);
   }

   uploadBase64(base64: string): Observable<HttpEvent<{}>> {
    const headers = this.getAuthorizedHeaders();

    let data = new FormData();
    data.append('base64', base64);

    const req = new HttpRequest('POST', '/api/upload/base64', data, {
      reportProgress: true,
      headers: headers,
      responseType: 'json'
    });

    return this.http.request(req);
   }
}
