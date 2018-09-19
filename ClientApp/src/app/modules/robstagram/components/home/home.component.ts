import { Component, OnInit } from '@angular/core';

import { Profile } from '../../models/profile.interface';
import { Entry } from '../../models/entry.interface';
import { RobstagramService } from '../../services/robstagram.service';
import * as signalR from '@aspnet/signalr';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  private _hubConnection: signalR.HubConnection | undefined;

  profile: Profile;
  entries: Entry[];

  constructor(private robstagramService: RobstagramService) { }

  ngOnInit() {
    this.initHub();

    // get current user profile data
    this.robstagramService.getProfile()
      .subscribe((profile: Profile) => {
        this.profile = profile;
      },
      error => {
        console.log(error);
      });
    // get home feed data
    this.robstagramService.getEntries()
      .subscribe(
      entries => {
        this.entries = entries;
      },
      error => {
        console.log(error);
      });
  }

  like(id: number) {
    this.robstagramService.postLike(id)
      .subscribe((result: Entry) => {
        const idx = this.entries.findIndex(x => x.id === id);
        this.entries[idx] = result;
        console.log(result);
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
        this.robstagramService.getEntry(id).subscribe((result: Entry) => {
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
}
