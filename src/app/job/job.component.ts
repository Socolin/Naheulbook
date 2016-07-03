import {Component} from '@angular/core';

import {Job} from "./job.model";

@Component({
    selector: 'job',
    templateUrl: 'app/job/job.component.html',
    styleUrls: ['/styles/job.css'],
    inputs: ['job']
})
export class JobComponent {
    public job: Job;
    public folded: boolean;

    constructor() {
        this.folded = true;
    }

    toggleFold(event: Event) {
        event.preventDefault();
        event.stopPropagation();
        this.folded = !this.folded;
    }
}
