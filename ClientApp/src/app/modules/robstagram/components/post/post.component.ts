import { Component, OnInit } from "@angular/core";
import { HttpEventType } from "@angular/common/http";
import { Router } from "@angular/router";
import {
  RobstagramService,
  EntryViewModel
} from "../../../../api/api.service.generated";
import { UploadService } from "../../../../shared/services/upload.service";

@Component({
  selector: "app-post",
  templateUrl: "./post.component.html",
  styleUrls: ["./post.component.css"]
})
export class PostComponent implements OnInit {
  // file data from input
  files: FileList;
  filestring: string;

  // flow variables
  errors: string;
  isRequesting: boolean;
  submitted = false;

  // response data
  uploadProgress: number;
  uploadMessage: string;
  uploadImageUrl: string;

  constructor(
    private uploadService: UploadService,
    private robstagramService: RobstagramService,
    private router: Router
  ) {}

  ngOnInit() {
    this.takePhoto();
  }

  takePhoto() {
    const element: HTMLElement = document.getElementById(
      "files"
    ) as HTMLElement;
    element.click();
  }

  onFileChange(event) {
    this.files = event.target.files;

    const reader = new FileReader();
    reader.onload = this._handleReaderLoaded.bind(this);
    reader.readAsBinaryString(this.files[0]);

    this.uploadProgress = 0;
  }

  _handleReaderLoaded(readerEvt) {
    const binaryString = readerEvt.target.result;
    this.filestring = btoa(binaryString); // converting binary string data

    const element: HTMLImageElement = document.getElementById(
      "preview"
    ) as HTMLImageElement;
    element.src = "data:image/jpeg;base64, " + this.filestring;
  }

  post({ value, valid }: { value: any; valid: boolean }) {
    this.uploadProgress = 0;
    this.submitted = true;
    this.isRequesting = true;
    this.errors = "";

    if (valid) {
      // first upload image to server
      this.uploadService.uploadSingle(this.files[0]).subscribe(
        event => {
          if (event.type === HttpEventType.UploadProgress) {
            this.uploadProgress = Math.round((100 * event.loaded) / event.total);
          } else if (event.type === HttpEventType.Response) {
            this.uploadMessage = JSON.stringify(event.body);
            const result = JSON.parse(JSON.stringify(event.body));
            console.log(result);

            // once uploaded create entry in db
            let entry = new EntryViewModel({
              description: value.description,
              imageUrl: result.path,
              size: result.size
            });

            this.robstagramService.postEntry(entry).subscribe(
              result => {
                console.log(result);
                this.isRequesting = false;
                this.router.navigate(["robstagram/home"]);
              },
              error => {
                console.log(error);
              }
            );
          }
        },
        error => {
          this.errors = JSON.stringify(error);
        }
      );
    }
  }
}
