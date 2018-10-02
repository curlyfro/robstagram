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

  constructor(private userService: UserService) { }

  ngOnInit() {
    this.userService.getProfile().subscribe(
      (profile: ProfileData) => {
        console.log(profile);
        this.profile = profile;
      },
      error => console.log(error)
    );
  }

}
