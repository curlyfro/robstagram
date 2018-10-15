import { Component, OnInit } from '@angular/core';
import { RobstagramService, ActivityTimeRange, ActivityData, ActivityAction } from '../../../../api/api.service.generated';
import { ToastrService } from 'ngx-toastr';
import { KeyValue } from '@angular/common';

@Component({
  selector: 'app-activity',
  templateUrl: './activity.component.html',
  styleUrls: ['./activity.component.css']
})
export class ActivityComponent implements OnInit {
  activities: { [key in keyof typeof ActivityTimeRange] : ActivityData[] } = undefined;
  activityAction = ActivityAction;

  constructor(private robstagramService: RobstagramService, private toastrService: ToastrService) { }

  ngOnInit() {
    this.robstagramService.getActivities().subscribe(
      (activities) => {
        this.activities = activities;
        console.log(activities);
      },
      error => this.toastrService.error(error)
    );
  }

  activityComparator(a: KeyValue<ActivityTimeRange, ActivityData[]>, b: KeyValue<ActivityTimeRange, ActivityData[]>) {
    const va = ActivityTimeRange[a.key];
    const vb = ActivityTimeRange[b.key];

    if (va === vb) {
      return 0;
    }
    return va < vb ? -1 : 1;
  }
}
