import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
  stages: [
    { duration: '1m', target: 100 }, // ramp up to 100 users over 1 minute
    { duration: '2m', target: 100 }, // stay at 100 users for 2 minutes
    { duration: '1m', target: 0 },   // ramp down to 0 users over 1 minute
  ],
};

export default function () {
  const res = http.get('https://your-api-url.com/endpoint');  // Replace with your API URL
  check(res, {
    'status is 200': (r) => r.status === 200,
    'response time is below 1s': (r) => r.timings.duration < 1000,  // 1 second
  });
  sleep(1);  // Pause for 1 second between requests
}
