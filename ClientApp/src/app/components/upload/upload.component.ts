import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})
export class UploadComponent implements OnInit {

  @ViewChild('fileInput') fileInput;
  imageToShow: any;
  isImageLoading: boolean;  

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  ngOnInit() {
  }  

  upload() {
    const files: FileList = this.fileInput.nativeElement.files;
    if (files.length === 0) {
      return;
    }

    const formData = new FormData();
    //formData.append('files', files);
    formData.append('file', files[0], files[0].name);

    //for (var i = 0; i < files.length; i++) {
    //  formData.append('files', files[i], files[i].name);
    //}
    
    console.log(files);

    this.http.post(this.baseUrl + 'api/UploadFiles/UploadSingleFile', formData)
      .subscribe(
        result => { console.log(result) },
        error => { console.error(error) }
     );
  }

  view() {
    this.isImageLoading = true;

    this.getImage()
      .subscribe(
      result => {
        //var fileUrl = URL.createObjectURL(result);
        //window.open(fileUrl);
        this.createImageFromBlob(result);
      }, error => {
          console.log(error);
        });    
  }

  getImage(): Observable<Blob> {
    let authToken = localStorage.getItem('auth_token');
    let headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${authToken}`
    });

    console.log(JSON.stringify(headers));
    console.log(authToken);

    return this.http.get(this.baseUrl + 'api/robstagram/viewimage/4', { headers: headers, responseType: "blob" });
  }

  createImageFromBlob(image: Blob) {
    let reader = new FileReader();
    reader.addEventListener("load", () => {
      this.imageToShow = reader.result;
      console.log(this.imageToShow);
    }, false);

    if (image) {
      reader.readAsDataURL(image);
    }
  }
}
