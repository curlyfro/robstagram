import { Injectable, Inject } from '@angular/core';
 
@Injectable()
export class ConfigService {
     
  _apiURI : string;
 
  constructor(@Inject('BASE_URL') baseUrl: string) {
    // this._apiURI = baseUrl + "api";
    // this._apiURI = 'http://localhost:25850/api';
    this._apiURI = 'http://192.168.0.59:12345/api';
  }
 
  getApiURI() {
    return this._apiURI;
  }    
}
