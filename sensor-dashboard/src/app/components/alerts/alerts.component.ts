import { Component, Input } from '@angular/core';
import { AnomalyAlert, AlertSeverity } from '../../models/sensor.models';
import { TimeTrackerService } from '../../services/time-tracker.service';

@Component({
  selector: 'app-alerts',
  templateUrl: './alerts.component.html',
  styleUrls: ['./alerts.component.scss'],
  standalone: false
})
export class AlertsComponent {
  @Input() alerts: AnomalyAlert[] = [];
  
  constructor(public timeTracker: TimeTrackerService) {}

  getAlertSeverityClass(severity: number): string {
    switch (severity) {
      case 0: return 'alert-low';
      case 1: return 'alert-medium';
      case 2: return 'alert-high';
      case 3: return 'alert-critical';
      default: return 'alert-low';
    }
  }

  getAlertSeverityIcon(severity: number): string {
    switch (severity) {
      case 0: return 'fas fa-info-circle';
      case 1: return 'fas fa-exclamation-triangle';
      case 2: return 'fas fa-exclamation-circle';
      case 3: return 'fas fa-skull-crossbones';
      default: return 'fas fa-info-circle';
    }
  }

  getAlertSeverityText(severity: number): string {
    switch (severity) {
      case 0: return 'Low';
      case 1: return 'Medium';
      case 2: return 'High';
      case 3: return 'Critical';
      default: return 'Low';
    }
  }


  trackByAlert(index: number, alert: AnomalyAlert): number {
    return alert.id;
  }
}
