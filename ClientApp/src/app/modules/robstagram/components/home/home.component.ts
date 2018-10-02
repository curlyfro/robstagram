import { Component, OnInit, OnDestroy } from '@angular/core';
import { RobstagramService, PostData } from '../../../../api/api.service.generated';
import { Router, NavigationEnd } from '../../../../../../node_modules/@angular/router';
import { Subscription } from '../../../../../../node_modules/rxjs';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, OnDestroy {
  navigationSubscription: Subscription;
  notificationSubscription: Subscription;

  entries: PostData[] = [];
  page = 1;

  constructor(
    private robstagramService: RobstagramService,
    private notificationService: NotificationService,
    private router: Router) { }

  ngOnInit() {
    // subscribe to the router events - storing the subscription so we can unsubscribe later
    this.navigationSubscription = this.router.events.subscribe((e: any) => {
      // if it is a navigationend event re-initialise the component
      if (e instanceof NavigationEnd) {
        this.entries = [];
        this.page = 1;
        this.getPosts();
      }
    });

    // subscribe to hub
    this.notificationSubscription = this.notificationService.getNewLikeSubscription().subscribe(
      (next: number) => {
        this.receiveLike(next);
      },
      error => console.log(error)
    );

    // get home feed data
    this.getPosts();
  }

  ngOnDestroy() {
    // avoid memory leaks here by cleaning up after ourselves. if we
    // dont then we will continue to run our subscription method
    // on NavigationEnd
    if (this.navigationSubscription) {
      this.navigationSubscription.unsubscribe();
    }

    this.notificationSubscription.unsubscribe();
  }

  like(id: number) {
    this.robstagramService.postLike(id)
      .subscribe(
        (result: PostData) => {
          const idx = this.entries.findIndex(x => x.id === id);
          this.entries[idx] = result;
          console.log('Like successful');
          // notify other clients
          this.notificationService.notifyNewLike(id);
        },
        error => {
          console.log(error);
        });
  }

  receiveLike(postId: number): void {
    // check if entry id exists in our collection
    const idx = this.entries.findIndex(x => x.id === postId);
    if (idx !== -1) {
      this.robstagramService.getPost(postId).subscribe(
        (result: PostData) => {
          const entry = result;
          this.entries[idx] = entry;
          console.log('Entry updated');
        },
        error => {
          console.log('Entry not in list');
          console.log(error);
        }
      );
    }
  }

  onScroll(): void {
    console.log('scrolled');
    this.page = this.page + 1;
    this.getPosts();
  }

  getPosts(): void {
    this.robstagramService.getPosts(this.page)
      .subscribe((entries: PostData[]) => {
        this.onSuccess(entries);
      },
        error => {
          console.log(error);
        });
  }

  onSuccess(entries) {
    console.log(entries);
    if (entries !== undefined) {
      entries.forEach(element => {
        this.entries.push(element);
      });
    }
  }
}
