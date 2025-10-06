const https = require('https');
const fs = require('fs');

// Performance test configuration
const API_BASE_URL = 'https://localhost:7000/api';
const TEST_DURATION_MS = 30000; // 30 seconds
const CONCURRENT_REQUESTS = 10;

// Disable SSL verification for localhost testing
process.env["NODE_TLS_REJECT_UNAUTHORIZED"] = 0;

class PerformanceTest {
    constructor() {
        this.results = {
            totalRequests: 0,
            successfulRequests: 0,
            failedRequests: 0,
            averageResponseTime: 0,
            minResponseTime: Infinity,
            maxResponseTime: 0,
            responseTimes: [],
            errors: [],
            startTime: null,
            endTime: null
        };
    }

    async makeRequest(endpoint) {
        const startTime = Date.now();
        
        return new Promise((resolve, reject) => {
            const req = https.get(`${API_BASE_URL}${endpoint}`, (res) => {
                let data = '';
                
                res.on('data', (chunk) => {
                    data += chunk;
                });
                
                res.on('end', () => {
                    const endTime = Date.now();
                    const responseTime = endTime - startTime;
                    
                    this.results.totalRequests++;
                    this.results.responseTimes.push(responseTime);
                    
                    if (res.statusCode >= 200 && res.statusCode < 300) {
                        this.results.successfulRequests++;
                    } else {
                        this.results.failedRequests++;
                        this.results.errors.push(`HTTP ${res.statusCode}: ${endpoint}`);
                    }
                    
                    // Update response time statistics
                    this.results.minResponseTime = Math.min(this.results.minResponseTime, responseTime);
                    this.results.maxResponseTime = Math.max(this.results.maxResponseTime, responseTime);
                    
                    resolve({
                        statusCode: res.statusCode,
                        responseTime: responseTime,
                        dataLength: data.length
                    });
                });
            });
            
            req.on('error', (error) => {
                this.results.totalRequests++;
                this.results.failedRequests++;
                this.results.errors.push(`Request error: ${error.message}`);
                reject(error);
            });
            
            req.setTimeout(5000, () => {
                req.destroy();
                this.results.totalRequests++;
                this.results.failedRequests++;
                this.results.errors.push(`Timeout: ${endpoint}`);
                reject(new Error('Request timeout'));
            });
        });
    }

    async runConcurrentRequests(endpoint, duration) {
        const endTime = Date.now() + duration;
        const promises = [];
        
        for (let i = 0; i < CONCURRENT_REQUESTS; i++) {
            promises.push(this.runRequestLoop(endpoint, endTime));
        }
        
        await Promise.allSettled(promises);
    }
    
    async runRequestLoop(endpoint, endTime) {
        while (Date.now() < endTime) {
            try {
                await this.makeRequest(endpoint);
                // Small delay to prevent overwhelming the server
                await new Promise(resolve => setTimeout(resolve, 10));
            } catch (error) {
                // Error already recorded in makeRequest
            }
        }
    }

    calculateStatistics() {
        if (this.results.responseTimes.length > 0) {
            this.results.averageResponseTime = 
                this.results.responseTimes.reduce((a, b) => a + b, 0) / this.results.responseTimes.length;
        }
        
        // Calculate percentiles
        const sortedTimes = this.results.responseTimes.sort((a, b) => a - b);
        const len = sortedTimes.length;
        
        this.results.p50 = len > 0 ? sortedTimes[Math.floor(len * 0.5)] : 0;
        this.results.p95 = len > 0 ? sortedTimes[Math.floor(len * 0.95)] : 0;
        this.results.p99 = len > 0 ? sortedTimes[Math.floor(len * 0.99)] : 0;
        
        // Calculate requests per second
        const durationSeconds = (this.results.endTime - this.results.startTime) / 1000;
        this.results.requestsPerSecond = this.results.totalRequests / durationSeconds;
    }

    async testEndpoint(endpoint, testName) {
        console.log(`\n--- Testing ${testName} ---`);
        console.log(`Endpoint: ${endpoint}`);
        console.log(`Duration: ${TEST_DURATION_MS / 1000} seconds`);
        console.log(`Concurrent requests: ${CONCURRENT_REQUESTS}`);
        
        // Reset results
        this.results = {
            totalRequests: 0,
            successfulRequests: 0,
            failedRequests: 0,
            averageResponseTime: 0,
            minResponseTime: Infinity,
            maxResponseTime: 0,
            responseTimes: [],
            errors: [],
            startTime: Date.now(),
            endTime: null
        };
        
        await this.runConcurrentRequests(endpoint, TEST_DURATION_MS);
        
        this.results.endTime = Date.now();
        this.calculateStatistics();
        
        return { ...this.results, testName, endpoint };
    }

    printResults(testResult) {
        console.log(`\n=== ${testResult.testName} Results ===`);
        console.log(`Total Requests: ${testResult.totalRequests}`);
        console.log(`Successful: ${testResult.successfulRequests}`);
        console.log(`Failed: ${testResult.failedRequests}`);
        console.log(`Success Rate: ${((testResult.successfulRequests / testResult.totalRequests) * 100).toFixed(2)}%`);
        console.log(`Requests/Second: ${testResult.requestsPerSecond.toFixed(2)}`);
        console.log(`Average Response Time: ${testResult.averageResponseTime.toFixed(2)}ms`);
        console.log(`Min Response Time: ${testResult.minResponseTime}ms`);
        console.log(`Max Response Time: ${testResult.maxResponseTime}ms`);
        console.log(`P50 Response Time: ${testResult.p50}ms`);
        console.log(`P95 Response Time: ${testResult.p95}ms`);
        console.log(`P99 Response Time: ${testResult.p99}ms`);
        
        if (testResult.errors.length > 0) {
            console.log(`\nErrors (showing first 5):`);
            testResult.errors.slice(0, 5).forEach(error => console.log(`  - ${error}`));
        }
    }

    async runFullTest() {
        console.log('Starting Performance Test Suite...');
        console.log(`Target API: ${API_BASE_URL}`);
        
        const testResults = [];
        
        // Test different endpoints
        const endpoints = [
            { path: '/sensor/health', name: 'Health Check' },
            { path: '/sensor/readings?count=100', name: 'Recent Readings' },
            { path: '/sensor/statistics', name: 'Statistics' },
            { path: '/sensor/alerts?count=20', name: 'Recent Alerts' }
        ];
        
        for (const endpoint of endpoints) {
            try {
                const result = await this.testEndpoint(endpoint.path, endpoint.name);
                testResults.push(result);
                this.printResults(result);
                
                // Wait between tests
                console.log('\nWaiting 5 seconds before next test...');
                await new Promise(resolve => setTimeout(resolve, 5000));
            } catch (error) {
                console.error(`Error testing ${endpoint.name}:`, error.message);
            }
        }
        
        // Generate summary report
        this.generateReport(testResults);
        
        return testResults;
    }
    
    generateReport(testResults) {
        const report = {
            timestamp: new Date().toISOString(),
            testConfiguration: {
                duration: TEST_DURATION_MS,
                concurrentRequests: CONCURRENT_REQUESTS,
                apiBaseUrl: API_BASE_URL
            },
            results: testResults,
            summary: {
                totalRequests: testResults.reduce((sum, r) => sum + r.totalRequests, 0),
                totalSuccessful: testResults.reduce((sum, r) => sum + r.successfulRequests, 0),
                totalFailed: testResults.reduce((sum, r) => sum + r.failedRequests, 0),
                averageRequestsPerSecond: testResults.reduce((sum, r) => sum + r.requestsPerSecond, 0) / testResults.length
            }
        };
        
        // Save to file
        fs.writeFileSync('performance-test-results.json', JSON.stringify(report, null, 2));
        
        console.log('\n=== PERFORMANCE TEST SUMMARY ===');
        console.log(`Total Requests Across All Tests: ${report.summary.totalRequests}`);
        console.log(`Total Successful: ${report.summary.totalSuccessful}`);
        console.log(`Total Failed: ${report.summary.totalFailed}`);
        console.log(`Overall Success Rate: ${((report.summary.totalSuccessful / report.summary.totalRequests) * 100).toFixed(2)}%`);
        console.log(`Average Requests/Second: ${report.summary.averageRequestsPerSecond.toFixed(2)}`);
        console.log(`\nDetailed results saved to: performance-test-results.json`);
    }
}

// Run the test
if (require.main === module) {
    const test = new PerformanceTest();
    test.runFullTest().catch(console.error);
}

module.exports = PerformanceTest;
