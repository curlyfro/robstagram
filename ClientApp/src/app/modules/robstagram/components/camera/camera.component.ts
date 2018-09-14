import { Component, OnInit } from '@angular/core';
import { Observable ,  Subject } from 'rxjs';
import { WebcamImage, WebcamInitError, WebcamUtil } from 'ngx-webcam';
import { RobstagramService } from '../../services/robstagram.service';
import { HttpEventType } from '@angular/common/http';


@Component({
  selector: 'app-camera',
  templateUrl: './camera.component.html',
  styleUrls: ['./camera.component.css']
})
export class CameraComponent implements OnInit {

  public showWebcam = true;
  public allowCameraSwitch = true;
  public multipleWebcamsAvailable = false;
  public deviceId: string;
  public videoOptions: MediaTrackConstraints = {
    // width: {ideal: 1024},
    // height: {ideal: 576}
  }
  public errors: WebcamInitError[] = [];

  // latest snapshot
  public webcamImage: WebcamImage = null;
  // webcam snapshot trigger
  private trigger: Subject<void> = new Subject<void>();
  // switch to next / previous / specific webcam; true/false: forward/backwards, string: deviceId
  private nextWebcam: Subject<boolean | string> = new Subject<boolean | string>();

  uploadProgress: number;
  uploadMessage: string;
  uploadImageUrl: string;
  errorsUpload: string;
  isRequesting: boolean;
  submitted: boolean = false;

  constructor(private robstagramService: RobstagramService) { }

  ngOnInit() {
    WebcamUtil.getAvailableVideoInputs().then((mediaDevices: MediaDeviceInfo[]) => {
      this.multipleWebcamsAvailable = mediaDevices && mediaDevices.length > 1;
    });
  }

  public triggerSnapshot(): void {
    this.trigger.next();
  }

  public toggleWebcam(): void {
    this.showWebcam = !this.showWebcam;
  }

  public handleInitError(error: WebcamInitError): void {
    this.errors.push(error);
  }

  public showNextWebcam(directionOrDeviceId: boolean | string): void {
    // true => move forward through devices
    // false => move backwards through devices
    // string => move to device with given deviceId
    this.nextWebcam.next(directionOrDeviceId);
  }

  public handleImage(webcamImage: WebcamImage): void {
    console.info('received webcam image', webcamImage);
    this.webcamImage = webcamImage;
  }

  public cameraWasSwitched(deviceId: string): void {
    console.log('active device: ' + deviceId);
    this.deviceId = deviceId;
  }

  public get triggerObservable(): Observable<void> {
    return this.trigger.asObservable();
  }

  public get nextWebcamObservable(): Observable<boolean | string> {
    return this.nextWebcam.asObservable();
  }

  public uploadImage(): void {
    this.robstagramService.putEntry(this.webcamImage.imageAsBase64, 'test description')
      .subscribe(
        event => {
          if (event.type === HttpEventType.UploadProgress)
            this.uploadProgress = Math.round(100 * event.loaded / event.total);
          else if (event.type === HttpEventType.Response) {
            this.uploadMessage = JSON.stringify(event.body);
            //this.isRequesting = false;
          }
        },
        error => {
          this.errorsUpload = JSON.stringify(error);
        }
      );
  }

}
