import { Injectable } from '@angular/core';
import { BehaviorSubject, interval } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TimeTrackerService {
  private currentTimeSubject = new BehaviorSubject<number>(Date.now());
  public currentTime$ = this.currentTimeSubject.asObservable();

  constructor() {
    // Update time every 5 seconds
    interval(5000).subscribe(() => {
      this.currentTimeSubject.next(Date.now());
    });
  }

  getCurrentTime(): number {
    return this.currentTimeSubject.value;
  }
}
