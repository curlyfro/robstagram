import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})
export class UploadComponent implements OnInit {
  @ViewChild('fileInput') fileInput;

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
    //formData.append(files[0].name, files[0]);

    for (var i = 0; i < files.length; i++) {
      formData.append('files', files[i], files[i].name);
    }
    
    console.log(files);

    this.http.post(this.baseUrl + 'api/UploadFiles/UploadMultipleFilesCollection', formData)
      .subscribe(
        result => { console.log(result) },
        error => { console.error(error) }
     );
  }
}
