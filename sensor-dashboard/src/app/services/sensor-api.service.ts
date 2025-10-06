import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { SensorReading, SensorStatistics, AnomalyAlert } from '../models/sensor.models';

@Injectable({
  providedIn: 'root'
})
export class SensorApiService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getRecentReadings(count: number = 1000): Observable<SensorReading[]> {
    return this.http.get<SensorReading[]>(`${this.apiUrl}/sensor/readings?count=${count}`);
  }

  getSensorReadings(sensorId: string, count: number = 100): Observable<SensorReading[]> {
    return this.http.get<SensorReading[]>(`${this.apiUrl}/sensor/readings/${sensorId}?count=${count}`);
  }

  getAllStatistics(): Observable<SensorStatistics[]> {
    return this.http.get<SensorStatistics[]>(`${this.apiUrl}/sensor/statistics`);
  }

  getSensorStatistics(sensorId: string): Observable<SensorStatistics> {
    return this.http.get<SensorStatistics>(`${this.apiUrl}/sensor/statistics/${sensorId}`);
  }

  getRecentAlerts(count: number = 50): Observable<AnomalyAlert[]> {
    return this.http.get<AnomalyAlert[]>(`${this.apiUrl}/sensor/alerts?count=${count}`);
  }

  getHealth(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/sensor/health`);
  }

  addReading(reading: SensorReading): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/sensor/readings`, reading);
  }

  purgeOldData(): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/sensor/purge`, {});
  }
}
