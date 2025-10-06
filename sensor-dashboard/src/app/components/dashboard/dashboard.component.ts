import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { SignalRService } from '../../services/signalr.service';
import { SensorApiService } from '../../services/sensor-api.service';
import { SensorReading, SensorStatistics, AnomalyAlert, SensorType } from '../../models/sensor.models';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  standalone: false
})
export class DashboardComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];
  
  // Data properties
  recentReadings: SensorReading[] = [];
  statistics: SensorStatistics[] = [];
  alerts: AnomalyAlert[] = [];
  totalReadings = 0;
  
  // Performance metrics
  readingsPerSecond = 0;
  memoryUsage = 0;
  uptime = 0;
  
  // UI state
  isLoading = true;
  selectedSensorType: SensorType | null = null;
  
  // Chart data
  chartData: any = null;
  
  constructor(
    private signalRService: SignalRService,
    private sensorApiService: SensorApiService
  ) {}

  ngOnInit() {
    this.setupSubscriptions();
    this.loadInitialData();
    this.startPerformanceMonitoring();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  private setupSubscriptions() {
    // Subscribe to real-time data updates
    const newReadingsSub = this.signalRService.newReadings$.subscribe(readings => {
      if (readings.length > 0) {
        this.recentReadings = [...readings, ...this.recentReadings].slice(0, 1000);
        this.updateChartData();
      }
    });

    const statisticsSub = this.signalRService.statistics$.subscribe(stats => {
      this.statistics = stats;
    });

    const alertsSub = this.signalRService.alerts$.subscribe(alerts => {
      if (alerts.length > 0) {
        this.alerts = [...alerts, ...this.alerts].slice(0, 100);
      }
    });

    const initialDataSub = this.signalRService.initialData$.subscribe(data => {
      if (data) {
        this.recentReadings = data.readings;
        this.statistics = data.statistics;
        this.alerts = data.alerts;
        this.totalReadings = data.totalCount;
        this.updateChartData();
        this.isLoading = false;
      }
    });

    this.subscriptions.push(newReadingsSub, statisticsSub, alertsSub, initialDataSub);
  }

  private loadInitialData() {
    // Fallback API calls if SignalR is not working
    if (!this.signalRService.isConnected()) {
      this.sensorApiService.getRecentReadings(100).subscribe({
        next: (readings) => {
          this.recentReadings = readings;
          this.updateChartData();
        },
        error: (error) => console.error('Error loading readings:', error)
      });

      this.sensorApiService.getAllStatistics().subscribe({
        next: (stats) => this.statistics = stats,
        error: (error) => console.error('Error loading statistics:', error)
      });

      this.sensorApiService.getRecentAlerts().subscribe({
        next: (alerts) => this.alerts = alerts,
        error: (error) => console.error('Error loading alerts:', error)
      });

      this.isLoading = false;
    }
  }

  private updateChartData() {
    if (this.recentReadings.length === 0) return;

    // Filter readings based on selected sensor type
    const filteredReadings = this.selectedSensorType === null 
      ? this.recentReadings 
      : this.recentReadings.filter(r => r.type === this.selectedSensorType);

    // Group filtered readings by sensor type for chart display
    const groupedData = this.groupReadingsBySensorType(filteredReadings);
    this.chartData = this.prepareChartData(groupedData);
  }

  private groupReadingsBySensorType(readings: SensorReading[] = this.recentReadings): Map<SensorType, SensorReading[]> {
    const grouped = new Map<SensorType, SensorReading[]>();
    
    readings.forEach(reading => {
      if (!grouped.has(reading.type)) {
        grouped.set(reading.type, []);
      }
      grouped.get(reading.type)!.push(reading);
    });

    return grouped;
  }

  private prepareChartData(groupedData: Map<SensorType, SensorReading[]>): any {
    const datasets: any[] = [];
    const colors = ['#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF', '#FF9F40', '#FF6384'];
    let colorIndex = 0;

    groupedData.forEach((readings, sensorType) => {
      const sortedReadings = readings
        .sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime())
        .slice(-50); // Last 50 readings per type

      datasets.push({
        label: SensorType[sensorType],
        data: sortedReadings.map(r => ({
          x: r.timestamp,
          y: r.value
        })),
        borderColor: colors[colorIndex % colors.length],
        backgroundColor: colors[colorIndex % colors.length] + '20',
        fill: false,
        tension: 0.1
      });
      colorIndex++;
    });

    return {
      datasets: datasets
    };
  }

  private startPerformanceMonitoring() {
    let lastReadingCount = this.totalReadings;
    
    setInterval(() => {
      // Calculate readings per second
      const currentCount = this.recentReadings.length;
      this.readingsPerSecond = Math.max(0, currentCount - lastReadingCount);
      lastReadingCount = currentCount;

      // Update other metrics
      this.memoryUsage = this.estimateMemoryUsage();
      this.uptime += 1;
    }, 1000);
  }

  private estimateMemoryUsage(): number {
    // Rough estimation based on data structures
    const readingsSize = this.recentReadings.length * 100; // ~100 bytes per reading
    const statsSize = this.statistics.length * 50; // ~50 bytes per stat
    const alertsSize = this.alerts.length * 80; // ~80 bytes per alert
    return readingsSize + statsSize + alertsSize;
  }

  // UI Helper methods
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

  filterBySensorType(type: SensorType | null) {
    this.selectedSensorType = type;
    // Update chart immediately when filter changes
    this.updateChartData();
  }

  getFilteredReadings(): SensorReading[] {
    if (this.selectedSensorType === null) {
      return this.recentReadings.slice(0, 50);
    }
    return this.recentReadings
      .filter(r => r.type === this.selectedSensorType)
      .slice(0, 50);
  }

  getFilteredStatistics(): SensorStatistics[] {
    if (this.selectedSensorType === null) {
      return this.statistics;
    }
    return this.statistics.filter(s => s.type === this.selectedSensorType);
  }

  formatBytes(bytes: number): string {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }

  formatUptime(seconds: number): string {
    const hours = Math.floor(seconds / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    const secs = seconds % 60;
    return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
  }

  trackByReading(index: number, reading: SensorReading): string {
    return `${reading.sensorId}-${reading.timestamp}`;
  }
}
