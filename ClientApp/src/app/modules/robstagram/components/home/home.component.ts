import { Component, OnInit } from '@angular/core';

import { HomeDetails } from '../../models/home.details.interface';
import { RobstagramService } from '../../services/robstagram.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  homeDetails: HomeDetails;

  constructor(private robstagramService: RobstagramService) { }

  ngOnInit() {
    this.robstagramService.getHomeDetails()
      .subscribe((homeDetails: HomeDetails) => {
        this.homeDetails = homeDetails;
      }),
      error => {
        // this.notificationService.printErrorMessage(error);
      }
  }

}
