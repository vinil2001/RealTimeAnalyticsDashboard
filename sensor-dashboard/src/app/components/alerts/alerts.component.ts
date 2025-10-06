import { Component, Input } from '@angular/core';
import { AnomalyAlert, AlertSeverity } from '../../models/sensor.models';

@Component({
  selector: 'app-alerts',
  templateUrl: './alerts.component.html',
  styleUrls: ['./alerts.component.scss'],
  standalone: false
})
export class AlertsComponent {
  @Input() alerts: AnomalyAlert[] = [];

  getAlertSeverityClass(severity: AlertSeverity): string {
    switch (severity) {
      case AlertSeverity.Low: return 'alert-low';
      case AlertSeverity.Medium: return 'alert-medium';
      case AlertSeverity.High: return 'alert-high';
      case AlertSeverity.Critical: return 'alert-critical';
      default: return 'alert-low';
    }
  }

  getAlertSeverityIcon(severity: AlertSeverity): string {
    switch (severity) {
      case AlertSeverity.Low: return 'fas fa-info-circle';
      case AlertSeverity.Medium: return 'fas fa-exclamation-triangle';
      case AlertSeverity.High: return 'fas fa-exclamation-circle';
      case AlertSeverity.Critical: return 'fas fa-skull-crossbones';
      default: return 'fas fa-info-circle';
    }
  }

  getAlertSeverityText(severity: AlertSeverity): string {
    return AlertSeverity[severity];
  }

  getTimeAgo(timestamp: Date): string {
    const now = new Date();
    const diff = now.getTime() - new Date(timestamp).getTime();
    const seconds = Math.floor(diff / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);

    if (hours > 0) {
      return `${hours}h ago`;
    } else if (minutes > 0) {
      return `${minutes}m ago`;
    } else {
      return `${seconds}s ago`;
    }
  }

  trackByAlert(index: number, alert: AnomalyAlert): string {
    return alert.id;
  }
}
