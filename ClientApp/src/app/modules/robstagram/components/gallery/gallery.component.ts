import { Component, OnInit } from '@angular/core';
import { RobstagramService, PostData } from '../../../../api/api.service.generated';

@Component({
  selector: 'app-gallery',
  templateUrl: './gallery.component.html',
  styleUrls: ['./gallery.component.css']
})
export class GalleryComponent implements OnInit {

  posts: PostData[] = [];
  page = 1;
  hasNextPage = true;

  constructor(private robstagramService: RobstagramService) { }

  ngOnInit() {
    this.getPosts();
  }

  getPosts(): void {
    this.robstagramService.getEntries(this.page, true).subscribe(
      (posts: PostData[]) => {
        if (posts !== undefined) {
          console.log(posts);
          this.hasNextPage = posts.length > 0;

          posts.forEach(post => this.posts.push(post));
          console.log(`count: ${this.posts.length}`);

          if (this.hasNextPage && !this.hasScrollBar()) {
            this.page++;
            this.getPosts();
          }
        }
      },
      error => {
        console.log(error);
      }
    );
  }

  onScroll(): void {
    console.log('scrolled');
    this.page = this.page + 1;
    this.getPosts();
  }

  private hasScrollBar(): boolean {
    return document.documentElement.scrollHeight > (window.innerHeight + window.pageYOffset);
  }
}
