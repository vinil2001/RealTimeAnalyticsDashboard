export interface SensorReading {
  sensorId: string;
  timestamp: Date;
  value: number;
  unit: string;
  type: SensorType;
  latitude?: number;
  longitude?: number;
}

export enum SensorType {
  Temperature = 0,
  Humidity = 1,
  Pressure = 2,
  Light = 3,
  Motion = 4,
  Sound = 5,
  AirQuality = 6
}

export interface SensorStatistics {
  sensorId: string;
  type: SensorType;
  averageValue: number;
  minValue: number;
  maxValue: number;
  standardDeviation: number;
  count: number;
  lastUpdate: Date;
}

export interface AnomalyAlert {
  id: number;
  sensorId: string;
  type: SensorType;
  value: number;
  expectedValue: number;
  deviation: number;
  message: string;
  timestamp: Date;
  severity: number; // Alert severity: 0=Low, 1=Medium, 2=High, 3=Critical
}

export enum AlertSeverity {
  Low = 0,
  Medium = 1,
  High = 2,
  Critical = 3
}

export interface DashboardData {
  readings: SensorReading[];
  statistics: SensorStatistics[];
  alerts: AnomalyAlert[];
  totalCount: number;
}

export interface PerformanceMetrics {
  totalReadings: number;
  readingsPerSecond: number;
  memoryUsage: number;
  activeSensors: number;
  uptime: number;
}
