import { Component, OnInit, OnDestroy } from "@angular/core";
import { RobstagramService, PostData } from "../../../../api/api.service.generated";
import { Router, NavigationEnd} from "@angular/router";
import { Subscription } from "rxjs";
import { NotificationService } from "../../services/notification.service";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
  styleUrls: ["./home.component.css"]
})
export class HomeComponent implements OnInit, OnDestroy {
  private _navigationSubscription: Subscription;
  private _notificationSubscription: Subscription;

  public posts: PostData[] = [];
  public page = 1;

  constructor(
    private robstagramService: RobstagramService,
    private notificationService: NotificationService,
    private toastrService: ToastrService,
    private router: Router
  ) {}

  ngOnInit() {
    // subscribe to the router events - storing the subscription so we can unsubscribe later
    this._navigationSubscription = this.router.events.subscribe((e: any) => {
      // if it is a navigationend event re-initialise the component
      if (e instanceof NavigationEnd) {
        this.posts = [];
        this.page = 1;
        this.getPosts();
      }
    });

    // subscribe to hub
    this._notificationSubscription = this.notificationService.likes$.subscribe(
      (postId: number) => {
        this.receiveLike(postId);
      },
      error => this.toastrService.error(error)
    );

    // get home feed data
    this.getPosts();
  }

  ngOnDestroy() {
    // avoid memory leaks here by cleaning up after ourselves. if we
    // dont then we will continue to run our subscription method
    // on NavigationEnd
    if (this._navigationSubscription) {
      this._navigationSubscription.unsubscribe();
    }
    if (this._notificationSubscription) {
      this._notificationSubscription.unsubscribe();
    }
  }

  private like(id: number) {
    this.robstagramService.postLike(id).subscribe(
      (post: PostData) => {
        const idx = this.posts.findIndex(x => x.id === id);
        if (idx === -1) {
          return;
        }
        this.posts[idx] = post;
        this.notificationService.notifyNewLike(id);
      },
      error => this.toastrService.error(error)
    );
  }

  private receiveLike(postId: number): void {
    // check if entry id exists in our collection so we can update it
    const idx = this.posts.findIndex(x => x.id === postId);
    if (idx !== -1) {
      this.robstagramService.getPost(postId).subscribe(
        (post: PostData) => {
          this.posts[idx] = post;
        },
        error => this.toastrService.error(error)
      );
    }
  }

  private onScroll(): void {
    this.page = this.page + 1;
    this.getPosts();
  }

  private getPosts(): void {
    this.robstagramService.getPosts(this.page).subscribe(
      (posts: PostData[]) => {
        if (posts !== undefined) {
          posts.forEach(element => {
            this.posts.push(element);
          });
        }
      },
      error => this.toastrService.error(error)
    );
  }

}
