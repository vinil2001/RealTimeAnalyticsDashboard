import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { SensorReading, SensorStatistics, AnomalyAlert, DashboardData } from '../models/sensor.models';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection: HubConnection | null = null;
  private connectionState = new BehaviorSubject<string>('Disconnected');
  
  // Data streams
  private newReadingsSubject = new BehaviorSubject<SensorReading[]>([]);
  private statisticsSubject = new BehaviorSubject<SensorStatistics[]>([]);
  private alertsSubject = new BehaviorSubject<AnomalyAlert[]>([]);
  private initialDataSubject = new BehaviorSubject<DashboardData | null>(null);

  public connectionState$ = this.connectionState.asObservable();
  public newReadings$ = this.newReadingsSubject.asObservable();
  public statistics$ = this.statisticsSubject.asObservable();
  public alerts$ = this.alertsSubject.asObservable();
  public initialData$ = this.initialDataSubject.asObservable();

  constructor() {
    this.createConnection();
  }

  private createConnection(): void {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(environment.signalRUrl)
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build();

    this.setupEventHandlers();
  }

  private setupEventHandlers(): void {
    if (!this.hubConnection) return;

    // Connection state events
    this.hubConnection.onreconnecting(() => {
      this.connectionState.next('Reconnecting');
    });

    this.hubConnection.onreconnected(() => {
      this.connectionState.next('Connected');
      this.requestInitialData();
    });

    this.hubConnection.onclose(() => {
      this.connectionState.next('Disconnected');
    });

    // Data events
    this.hubConnection.on('NewReadings', (readings: SensorReading[]) => {
      // Convert timestamp strings to Date objects
      const processedReadings = readings.map(r => ({
        ...r,
        timestamp: new Date(r.timestamp)
      }));
      this.newReadingsSubject.next(processedReadings);
    });

    this.hubConnection.on('StatisticsUpdate', (data: any) => {
      const processedStats = data.statistics.map((s: any) => ({
        ...s,
        lastUpdate: new Date(s.lastUpdate)
      }));
      this.statisticsSubject.next(processedStats);
    });

    this.hubConnection.on('NewAlerts', (alerts: AnomalyAlert[]) => {
      const processedAlerts = alerts.map(a => ({
        ...a,
        timestamp: new Date(a.timestamp)
      }));
      this.alertsSubject.next(processedAlerts);
    });

    this.hubConnection.on('InitialData', (data: any) => {
      const processedData: DashboardData = {
        readings: data.readings.map((r: any) => ({
          ...r,
          timestamp: new Date(r.timestamp)
        })),
        statistics: data.statistics.map((s: any) => ({
          ...s,
          lastUpdate: new Date(s.lastUpdate)
        })),
        alerts: data.alerts.map((a: any) => ({
          ...a,
          timestamp: new Date(a.timestamp)
        })),
        totalCount: data.totalCount
      };
      this.initialDataSubject.next(processedData);
    });

    this.hubConnection.on('Error', (error: string) => {
      console.error('SignalR Error:', error);
    });
  }

  public async startConnection(): Promise<void> {
    if (!this.hubConnection) {
      this.createConnection();
    }

    try {
      this.connectionState.next('Connecting');
      await this.hubConnection!.start();
      this.connectionState.next('Connected');
      console.log('SignalR connection established');
      
      // Request initial data after connection
      await this.requestInitialData();
    } catch (error) {
      console.error('Error starting SignalR connection:', error);
      this.connectionState.next('Disconnected');
      throw error;
    }
  }

  public async stopConnection(): Promise<void> {
    if (this.hubConnection) {
      await this.hubConnection.stop();
      this.connectionState.next('Disconnected');
    }
  }

  private async requestInitialData(): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === 'Connected') {
      try {
        await this.hubConnection.invoke('GetInitialData');
      } catch (error) {
        console.error('Error requesting initial data:', error);
      }
    }
  }

  public async joinGroup(groupName: string): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === 'Connected') {
      await this.hubConnection.invoke('JoinGroup', groupName);
    }
  }

  public async leaveGroup(groupName: string): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === 'Connected') {
      await this.hubConnection.invoke('LeaveGroup', groupName);
    }
  }

  public getConnectionState(): string {
    return this.connectionState.value;
  }

  public isConnected(): boolean {
    return this.hubConnection?.state === 'Connected';
  }
}
