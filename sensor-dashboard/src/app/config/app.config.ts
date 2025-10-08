export interface AppConfig {
  backend: {
    httpPort: number;
    httpsPort: number;
    host: string;
    useHttps: boolean;
  };
  frontend: {
    port: number;
  };
}

export const APP_CONFIG: AppConfig = {
  backend: {
    httpPort: 5000,
    httpsPort: 7206,
    host: 'localhost',
    useHttps: false // Set to true for production
  },
  frontend: {
    port: 4200
  }
};

// Helper function to build backend URLs
export function getBackendUrl(useApi: boolean = true): string {
  const config = APP_CONFIG.backend;
  const protocol = config.useHttps ? 'https' : 'http';
  const port = config.useHttps ? config.httpsPort : config.httpPort;
  const baseUrl = `${protocol}://${config.host}:${port}`;
  
  return useApi ? `${baseUrl}/api` : baseUrl;
}

export function getSignalRUrl(): string {
  return `${getBackendUrl(false)}/sensorHub`;
}
