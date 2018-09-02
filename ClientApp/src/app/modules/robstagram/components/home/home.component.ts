import { Component, OnInit } from '@angular/core';

import { HomeDetails } from '../../models/home.details.interface';
import { Image } from '../../models/image.interface';
import { RobstagramService } from '../../services/robstagram.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  homeDetails: HomeDetails;
  homeImages: Image[];
  //images: any;

  constructor(private robstagramService: RobstagramService) { }

  ngOnInit() {
    this.robstagramService.getHomeDetails()
      .subscribe((homeDetails: HomeDetails) => {
        this.homeDetails = homeDetails;
      }),
      error => {
        console.log(error);
        // this.notificationService.printErrorMessage(error);
      }

    this.robstagramService.getImages()
        .subscribe((images: Image[]) => {
          this.homeImages = images;
          console.log(this.homeImages);
        }),
      error => {
        console.log(error);
        // this.notificationService.printErrorMessage(error);
      }      
  }

}
