import { Pipe, PipeTransform, inject } from '@angular/core';
import { TimeTrackerService } from '../services/time-tracker.service';

@Pipe({
  name: 'timeAgo',
  standalone: false,
  pure: true // Pure pipe - stable for change detection
})
export class TimeAgoPipe implements PipeTransform {
  private timeTracker = inject(TimeTrackerService);

  transform(value: Date | string, currentTime?: number): string {
    if (!value) return '';
    
    const date = new Date(value);
    const now = currentTime || this.timeTracker.getCurrentTime();
    
    const diff = now - date.getTime();
    const seconds = Math.floor(diff / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);
    
    if (days > 0) {
      return `${days}d ago`;
    } else if (hours > 0) {
      return `${hours}h ago`;
    } else if (minutes > 0) {
      return `${minutes}m ago`;
    } else {
      return `${seconds}s ago`;
    }
  }
}
