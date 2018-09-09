import { Component, OnInit, Inject } from '@angular/core';
import { HttpHeaders, HttpClient, HttpRequest, HttpEventType } from '@angular/common/http';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})
export class UploadComponent implements OnInit {

  uploadProgress: number;
  uploadMessage: string;
  uploadImageUrl: string;
  downloadImage: any;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  ngOnInit() {
  }  

  upload(files) {
    if (files.length === 0)
      return;

    const formData = new FormData();
    const file = files[0];
    formData.append("file", file, file.name);
    // for (let file of files) {
    //   formData.append("files", file, file.name);
    // }

    const uploadReq = new HttpRequest('POST', this.baseUrl + 'api/UploadFiles/UploadSingleFile', formData, {
      reportProgress: true,
    });

    this.http.request(uploadReq).subscribe(
      event => {
        if(event.type === HttpEventType.UploadProgress)
          this.uploadProgress = Math.round(100 * event.loaded / event.total);
        else if (event.type === HttpEventType.Response) {
          this.uploadMessage = JSON.stringify(event.body);
          let response: { count?: number, path?: string } = event.body;
          this.uploadImageUrl = response.path; } 
      },
      error => {
        this.uploadMessage = error;
      });
  }

  uploadBase64(files) {
    if (files.length === 0)
      return;

    let reader = new FileReader();
    let file = files[0];
    reader.readAsDataURL(file);
    reader.onload = () => {
      console.log(reader.result.split(',')[1]);
    }
  }
}
