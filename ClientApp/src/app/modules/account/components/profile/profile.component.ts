import { Component, OnInit } from '@angular/core';
import { UserService } from '../../../../shared/services/user.service';
import { ProfileData } from '../../../../api/api.service.generated';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  profile: ProfileData = undefined;
  loggedIn: boolean = false;

  constructor(private userService: UserService) { }

  ngOnInit() {
    this.userService.authStatus$.subscribe(
      (result) => {
        this.loggedIn = result;
      },
      error => console.log(error)
    );
    this.userService.userProfile$.subscribe(
      (profile: ProfileData) => {
        this.profile = profile;
      },
      error => console.log(error)
    );
  }

  logout() {
    this.userService.logout();
  }
}
