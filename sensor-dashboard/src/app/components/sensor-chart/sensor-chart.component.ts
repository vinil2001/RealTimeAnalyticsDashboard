import { Component, Input, OnInit, OnDestroy, ViewChild, ElementRef, OnChanges, SimpleChanges } from '@angular/core';
import { Chart, ChartConfiguration, ChartType, registerables } from 'chart.js';
import 'chartjs-adapter-date-fns';

Chart.register(...registerables);

@Component({
  selector: 'app-sensor-chart',
  templateUrl: './sensor-chart.component.html',
  styleUrls: ['./sensor-chart.component.scss'],
  standalone: false
})
export class SensorChartComponent implements OnInit, OnDestroy, OnChanges {
  @Input() chartData: any = null;
  @Input() height: number = 400;
  @ViewChild('chartCanvas', { static: true }) chartCanvas!: ElementRef<HTMLCanvasElement>;

  private chart: Chart | null = null;

  ngOnInit() {
    this.initializeChart();
  }

  ngOnDestroy() {
    if (this.chart) {
      this.chart.destroy();
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['chartData'] && this.chart && this.chartData) {
      this.updateChart();
    }
  }

  private initializeChart() {
    const ctx = this.chartCanvas.nativeElement.getContext('2d');
    if (!ctx) return;

    const config: ChartConfiguration = {
      type: 'line' as ChartType,
      data: {
        datasets: []
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        interaction: {
          mode: 'index',
          intersect: false,
        },
        plugins: {
          title: {
            display: true,
            text: 'Real-time Sensor Readings'
          },
          legend: {
            display: true,
            position: 'top'
          },
          tooltip: {
            callbacks: {
              title: (context) => {
                const date = new Date(context[0].parsed.x);
                return date.toLocaleString();
              },
              label: (context) => {
                return `${context.dataset.label}: ${context.parsed.y.toFixed(2)}`;
              }
            }
          }
        },
        scales: {
          x: {
            type: 'time',
            time: {
              displayFormats: {
                second: 'HH:mm:ss',
                minute: 'HH:mm',
                hour: 'HH:mm'
              }
            },
            title: {
              display: true,
              text: 'Time'
            }
          },
          y: {
            title: {
              display: true,
              text: 'Value'
            },
            beginAtZero: false
          }
        },
        animation: {
          duration: 0 // Disable animations for better performance with real-time data
        },
        elements: {
          point: {
            radius: 2,
            hoverRadius: 4
          },
          line: {
            tension: 0.1
          }
        }
      }
    };

    this.chart = new Chart(ctx, config);
    
    if (this.chartData) {
      this.updateChart();
    }
  }

  private updateChart() {
    if (!this.chart || !this.chartData) return;

    // Update chart data
    this.chart.data.datasets = this.chartData.datasets;
    
    // Limit the number of data points for performance
    this.chart.data.datasets.forEach(dataset => {
      if (dataset.data.length > 100) {
        dataset.data = dataset.data.slice(-100);
      }
    });

    this.chart.update('none'); // Use 'none' mode for better performance
  }

  // Method to manually refresh the chart
  refreshChart() {
    if (this.chart) {
      this.chart.update();
    }
  }
}
