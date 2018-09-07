import { Component, OnInit } from '@angular/core';
import { RobstagramService } from '../../services/robstagram.service';
import { HttpEventType } from '@angular/common/http';

@Component({
  selector: 'app-entry',
  templateUrl: './entry.component.html',
  styleUrls: ['./entry.component.css']
})
export class EntryComponent implements OnInit {

  files: FileList;
  filestring: string;

  errors: string;
  isRequesting: boolean;
  submitted: boolean = false;

  uploadProgress: number;
  uploadMessage: string;
  uploadImageUrl: string;

  constructor(private robstagramService: RobstagramService) { }

  ngOnInit() {
  }

  post({ value, valid }: { value, valid: boolean }) {
    this.submitted = true;
    this.isRequesting = true;
    this.errors = '';

    if (valid) {
      this.robstagramService.postEntry({description: value.description, image: this.files[0]})
        .subscribe(
          event => {
            if(event.type === HttpEventType.UploadProgress)
              this.uploadProgress = Math.round(100 * event.loaded / event.total);
            else if (event.type === HttpEventType.Response) {
              this.uploadMessage = JSON.stringify(event.body);
              this.isRequesting = false; } 
          },
          error => {
            this.errors = JSON.stringify(error);
          }
        );
    }
  }

  getFiles(event) {
    this.files = event.target.files;
    var reader = new FileReader();
    reader.onload = this._handleReaderLoaded.bind(this);
    reader.readAsBinaryString(this.files[0]);
  }

  _handleReaderLoaded(readerEvt) {
    var binaryString = readerEvt.target.result;
    this.filestring = btoa(binaryString); // converting binary string data
  }
}
