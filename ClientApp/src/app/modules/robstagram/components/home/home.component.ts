import { Component, OnInit } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { RobstagramService, ProfileData, PostData } from '../../../../api/api.service.generated';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  private _hubConnection: signalR.HubConnection | undefined;

  entries: PostData[] = [];
  page = 1;

  constructor(private robstagramService: RobstagramService) { }

  ngOnInit() {
    // init SignalR hub
    this.initHub();

    // get home feed data
    this.getPosts();
  }

  like(id: number) {
    this.robstagramService.postLike(id)
      .subscribe((result: PostData) => {
        const idx = this.entries.findIndex(x => x.id === id);
        this.entries[idx] = result;
        console.log('Like successful');
        // notify other clients
        this.sendLike(id);
      },
      error => {
        console.log(error);
      });
  }

  initHub(): void {
    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/appHub')
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this._hubConnection.start().catch(err => console.error(err.toString()));

    this._hubConnection.on('ReceiveLike', (data: any) => {
      this.receiveLike(data);
    });
  }

  sendLike(id: number): void {
    const data = id;

    if (this._hubConnection) {
      this._hubConnection.invoke('SendLike', data);
    }
  }

  receiveLike(data: any): void {
    const id = data;
      // check if entry id exists in our collection
      const idx = this.entries.findIndex(x => x.id === id);
      if (idx !== -1) {
        this.robstagramService.getPost(id)
          .subscribe((result: PostData) => {
            const entry = result;
            this.entries[idx] = entry;
            console.log('Entry updated');
          },
          error => {
            console.log('Entry not in list');
            console.log(error);
          });
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
