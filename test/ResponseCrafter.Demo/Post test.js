import http from 'k6/http';
import { check, sleep } from 'k6';

const Api1 = "http://localhost:5074/load-controller";
const Api2 = "http://localhost:5074/load-minimal";

export let options = {
    vus: 3000,
    duration: '45s',
    summaryTrendStats: ['avg', 'min', 'max'],
    summaryTimeUnit: 'ms'
};

export default function () {
    const payload = JSON.stringify({
        Name: "This is random and a very long name",
        PhoneNumber: "This is a very long number",
        Email: "This is a very long email",
        Status: "This is a very long status"
    });

    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    const res = http.post(Api1, payload, params);

    sleep(0.1);
}