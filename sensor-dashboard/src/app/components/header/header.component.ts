import { Component, OnInit } from '@angular/core';
import { SignalRService } from '../../services/signalr.service';
import { SensorApiService } from '../../services/sensor-api.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  standalone: false
})
export class HeaderComponent implements OnInit {
  connectionStatus = 'Disconnected';
  totalReadings = 0;
  activeSensors = 0;
  currentTime = new Date();

  constructor(
    private signalRService: SignalRService,
    private sensorApiService: SensorApiService
  ) {}

  ngOnInit() {
    // Update time every second
    setInterval(() => {
      this.currentTime = new Date();
    }, 1000);

    // Monitor connection status
    this.signalRService.connectionState$.subscribe(status => {
      this.connectionStatus = status;
    });

    // Monitor statistics updates
    this.signalRService.statistics$.subscribe(stats => {
      this.activeSensors = stats.length;
    });

    // Get initial health data
    this.loadHealthData();
  }

  private loadHealthData() {
    this.sensorApiService.getHealth().subscribe({
      next: (health) => {
        this.totalReadings = health.totalReadings;
        this.activeSensors = health.activeSensors;
      },
      error: (error) => {
        console.error('Error loading health data:', error);
      }
    });
  }

  getConnectionStatusClass(): string {
    switch (this.connectionStatus) {
      case 'Connected': return 'status-online';
      case 'Connecting': 
      case 'Reconnecting': return 'status-warning';
      default: return 'status-offline';
    }
  }
}
