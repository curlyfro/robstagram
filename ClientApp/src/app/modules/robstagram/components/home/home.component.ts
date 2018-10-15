import { Component, OnInit, OnDestroy } from '@angular/core';
import { RobstagramService, PostData, ProfileData } from '../../../../api/api.service.generated';
import { Router, NavigationEnd} from '@angular/router';
import { Subscription } from 'rxjs';
import { NotificationService } from '../../services/notification.service';
import { ToastrService } from 'ngx-toastr';
import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';
import { UserService } from '../../../../shared/services/user.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, OnDestroy {
  private _navigationSubscription: Subscription;
  private _notificationSubscription: Subscription;

  public user: ProfileData = undefined;
  public posts: PostData[] = [];
  public page = 1;

  closeResult: string;

  constructor(
    private robstagramService: RobstagramService,
    private notificationService: NotificationService,
    private toastrService: ToastrService,
    private modalService: NgbModal,
    private userService: UserService,
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

    // subscribe to current authenticated user
    this.userService.userProfile$.subscribe(
      (profile: ProfileData) => this.user = profile,
      (error: any) => this.toastrService.error(error)
    );

    // subscribe to hub
    this._notificationSubscription = this.notificationService.likes$.subscribe(
      (postId: number) => this.onPostLiked(postId),
      (error: any) => this.toastrService.error(error)
    );
    this._notificationSubscription = this.notificationService.deleted$.subscribe(
      (postId: number) => this.onPostDeleted(postId),
      (error: any) => this.toastrService.error(error)
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
        this.notificationService.notifyPostLiked(id);
      },
      error => this.toastrService.error(error)
    );
  }

  private delete(id: number) {
    this.robstagramService.deletePost(id).subscribe(
      (res: string) => {
        this.toastrService.success(`Post (${id}) deleted`);
        this.notificationService.notifyPostDeleted(id);
      },
      error => this.toastrService.error(error)
    );
  }

  private comment(id: number, text: string) {
    this.robstagramService.createComment(id, text).subscribe(
      (post: PostData) => {
        this.toastrService.success(`Comment created`);
        this.notificationService.notifyPostLiked(id);
      },
      error => this.toastrService.error(error)
    );
  }

  private deleteComment(postId: number, commentId: number) {
    this.robstagramService.deleteComment(commentId).subscribe(
      (post: PostData) => {
        this.toastrService.success('Comment deleted');
        this.notificationService.notifyPostLiked(postId);
      },
      error => this.toastrService.error(error)
    );
  }

  private onPostLiked(postId: number): void {
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

  private onPostDeleted(postId: number): void {
    // check if entry id exists in our collection so we can update it
    const idx = this.posts.findIndex(x => x.id === postId);
    if (idx !== -1) {
      this.posts.splice(idx, 1);
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

  openDeletePostModal(content: any, postId: number) {
    this.modalService.open(content, {
      ariaLabelledBy: 'modal-basic-title',
      centered: true
    })
    .result
    .then((result) => {
      this.closeResult = `Closed with: ${result}`;
      this.delete(postId);
    }, (reason) => {
      this.closeResult = `Dismissed ${this.getDismissReason(reason)}`;
    });
  }

  openDeleteCommentModal(content: any, postId: number, commentId: number) {
    this.modalService.open(content, {
      ariaLabelledBy: 'modal-basic-title',
      centered: true
    })
    .result
    .then((result) => {
      this.closeResult = `Closed with: ${result}`;
      this.deleteComment(postId, commentId);
    }, (reason) => {
      this.closeResult = `Dismissed ${this.getDismissReason(reason)}`;
    });
  }

  private getDismissReason(reason: any): string {
    if (reason === ModalDismissReasons.ESC) {
      return 'by pressing ESC';
    } else if (reason === ModalDismissReasons.BACKDROP_CLICK) {
      return 'by clicking on a backdrop';
    } else {
      return `with: ${reason}`;
    }
  }

  private toggleExpandComments(postId: number) {
    const idx = this.posts.findIndex(x => x.id === postId);
    if (idx !== -1) {
      this.posts[idx]['commentsExpanded'] = !this.posts[idx]['commentsExpanded'];
    }
  }

  private toggleComment(postId: number) {
    const idx = this.posts.findIndex(x => x.id === postId);
    if (idx !== -1) {
      this.posts[idx]['commentInputVisible'] = !this.posts[idx]['commentInputVisible'];
    }
  }
}
