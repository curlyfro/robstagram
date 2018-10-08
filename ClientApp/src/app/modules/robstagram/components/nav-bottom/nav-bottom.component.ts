import { Component, OnInit, OnDestroy } from '@angular/core';
import { NotificationService } from '../../services/notification.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-nav-bottom',
  templateUrl: './nav-bottom.component.html',
  styleUrls: ['./nav-bottom.component.css']
})
export class NavBottomComponent implements OnInit, OnDestroy {
  private _notificationSubscription: Subscription;
  public newPostCount = 0;

  constructor(private notificationService: NotificationService) { }

  ngOnInit() {
    this._notificationSubscription = this.notificationService.posts$.subscribe(
      (next: number) => {
        this.newPostCount = next as number;
      },
      error => console.log(error)
    );
  }

  ngOnDestroy() {
    if (this._notificationSubscription) {
      this._notificationSubscription.unsubscribe();
    }
  }

  resetNewPostCount(): void {
    this.notificationService.resetNewPost();
  }
}
