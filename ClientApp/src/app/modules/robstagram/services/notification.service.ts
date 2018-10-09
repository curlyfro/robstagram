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

  private _deleted: BehaviorSubject<number> = new BehaviorSubject(-1);
  public deleted$: Observable<number> = this._deleted.asObservable();

  constructor() {
    this.initHub();
  }

  private initHub() {
    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/appHub')
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this._hubConnection.start().catch(err => console.error(err.toString()));

    this._hubConnection.on('ReceivePostCreated', () => {
      this.onPostCreated();
    });
    this._hubConnection.on('ReceivePostLiked', (data: any) => {
      this.onPostLiked(data);
    });
    this._hubConnection.on('ReceivePostDeleted', (data: any) => {
      this.onPostDeleted(data);
    });
  }

  private onPostCreated(): void {
    this._postCount++;
    this._posts.next(this._postCount);
  }

  public notifyPostCreated(): void {
    if (this._hubConnection) {
      this._hubConnection.invoke('PostCreated');
    }
  }

  public resetPostCreatedCounter(): void {
    this._postCount = 0;
    this._posts.next(0);
  }

  private onPostLiked(postId: number): void {
    this._likes.next(postId);
  }

  public notifyPostLiked(postId: number): void {
    if (this._hubConnection) {
      this._hubConnection.invoke('PostLiked', postId);
    }
  }

  public notifyPostDeleted(postId: number): void {
    if (this._hubConnection) {
      this._hubConnection.invoke('PostDeleted', postId);
    }
  }

  private onPostDeleted(postId: number): void {
    this._deleted.next(postId);
  }
}
