import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private _hubConnection: signalR.HubConnection | undefined;

  private _postCount = 0;
  private _posts: BehaviorSubject<number> = new BehaviorSubject(this._postCount);
  public posts$: Observable<number> = this._posts.asObservable();

  private _likes: BehaviorSubject<number> = new BehaviorSubject(-1);
  public likes$: Observable<number> = this._likes.asObservable();

  constructor() {
    this.initHub();
  }

  private initHub() {
    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/appHub')
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this._hubConnection.start().catch(err => console.error(err.toString()));

    this._hubConnection.on('ReceivePost', () => {
      this.onNewPostNotification();
    });
    this._hubConnection.on('ReceiveLike', (data: any) => {
      this.onNewLikeNotification(data);
    });
  }

  private onNewPostNotification(): void {
    this._postCount++;
    this._posts.next(this._postCount);
  }

  public notifyNewPost(): void {
    if (this._hubConnection) {
      this._hubConnection.invoke('SendPost');
    }
  }

  public resetNewPost(): void {
    this._postCount = 0;
    this._posts.next(0);
  }

  private onNewLikeNotification(postId: number): void {
    this._likes.next(postId);
  }

  public notifyNewLike(postId: number): void {
    if (this._hubConnection) {
      this._hubConnection.invoke('SendLike', postId);
    }
  }
}
