import { Component, OnInit } from '@angular/core';

import { Profile } from '../../models/profile.interface';
import { Entry } from '../../models/entry.interface';
import { RobstagramService } from '../../services/robstagram.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  profile: Profile;
  entries: Entry[];

  constructor(private robstagramService: RobstagramService) { }

  ngOnInit() {
    // get current user profile data
    this.robstagramService.getProfile()
      .subscribe((profile: Profile) => {
        this.profile = profile;
      },
      error => {
        console.log(error);
      });
    // get home feed data
    this.robstagramService.getEntries()
      .subscribe(
      entries => {
        this.entries = entries;
      },
      error => {
        console.log(error);
      });     
  }

  like(id: number) {
    this.robstagramService.postLike(id)
      .subscribe((result: Entry) => {
        let idx = this.entries.findIndex(x => x.id == id);
        this.entries[idx] = result;
        console.log(result);
        console.log('Like successful');
      },
      error => {
        console.log(error);
      });
  }

}
