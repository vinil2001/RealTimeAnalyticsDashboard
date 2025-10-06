import { Component, OnInit, OnDestroy } from '@angular/core';
import { SignalRService } from './services/signalr.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  standalone: false
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'Real-Time Sensor Analytics Dashboard';

  constructor(private signalRService: SignalRService) {}

  async ngOnInit() {
    try {
      await this.signalRService.startConnection();
    } catch (error) {
      console.error('Failed to start SignalR connection:', error);
    }
  }

  async ngOnDestroy() {
    await this.signalRService.stopConnection();
  }
}
