import { Component, Input } from '@angular/core';
import { SensorStatistics, SensorType } from '../../models/sensor.models';

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.component.html',
  styleUrls: ['./statistics.component.scss'],
  standalone: false
})
export class StatisticsComponent {
  @Input() statistics: SensorStatistics[] = [];
  Math = Math; // Make Math available in template

  getSensorTypeIcon(type: SensorType): string {
    switch (type) {
      case SensorType.Temperature: return 'fas fa-thermometer-half';
      case SensorType.Humidity: return 'fas fa-tint';
      case SensorType.Pressure: return 'fas fa-gauge-high';
      case SensorType.Light: return 'fas fa-lightbulb';
      case SensorType.Motion: return 'fas fa-running';
      case SensorType.Sound: return 'fas fa-volume-up';
      case SensorType.AirQuality: return 'fas fa-wind';
      default: return 'fas fa-microchip';
    }
  }

  getSensorTypeName(type: SensorType): string {
    return SensorType[type];
  }

  getSensorTypeColor(type: SensorType): string {
    switch (type) {
      case SensorType.Temperature: return 'text-danger';
      case SensorType.Humidity: return 'text-info';
      case SensorType.Pressure: return 'text-warning';
      case SensorType.Light: return 'text-warning';
      case SensorType.Motion: return 'text-success';
      case SensorType.Sound: return 'text-primary';
      case SensorType.AirQuality: return 'text-secondary';
      default: return 'text-muted';
    }
  }

  getHealthStatus(stats: SensorStatistics): { status: string, class: string } {
    const now = new Date();
    const lastUpdate = new Date(stats.lastUpdate);
    const timeDiff = now.getTime() - lastUpdate.getTime();
    const minutesDiff = timeDiff / (1000 * 60);

    if (minutesDiff < 1) {
      return { status: 'Active', class: 'text-success' };
    } else if (minutesDiff < 5) {
      return { status: 'Recent', class: 'text-warning' };
    } else {
      return { status: 'Inactive', class: 'text-danger' };
    }
  }

  getVariabilityLevel(standardDeviation: number, average: number): { level: string, class: string } {
    if (average === 0) return { level: 'Unknown', class: 'text-muted' };
    
    const coefficient = (standardDeviation / Math.abs(average)) * 100;
    
    if (coefficient < 5) {
      return { level: 'Stable', class: 'text-success' };
    } else if (coefficient < 15) {
      return { level: 'Moderate', class: 'text-warning' };
    } else {
      return { level: 'Variable', class: 'text-danger' };
    }
  }

  trackByStats(index: number, stats: SensorStatistics): string {
    return stats.sensorId;
  }

  formatNumber(value: number, decimals: number = 2): string {
    return value.toFixed(decimals);
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
}
