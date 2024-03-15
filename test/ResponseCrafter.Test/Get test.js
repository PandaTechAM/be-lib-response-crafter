import http from 'k6/http';
import { check, sleep } from 'k6';

const Api1 = "http://localhost:5074/error-by-exception-controller";
const Api2 = "http://localhost:5074/error-by-result-controller";
const Api3 = "http://localhost:5074/error-by-exception-minimal";
const Api4 = "http://localhost:5074/error-by-result-minimal";

export let options = {
  vus: 100,
  duration: '30s',

  summaryTrendStats: ['avg', 'min', 'max'],
  summaryTimeUnit: 'ms'
  };
export default function () {
    const res = http.get(Api4);
    sleep(0.1);
}
