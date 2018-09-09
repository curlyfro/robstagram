import { Component } from '@angular/core';

import { UserService } from '../../shared/services/user.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {

  constructor(private userService: UserService) {}

  logout() {
    this.userService.logout();
  }
}
