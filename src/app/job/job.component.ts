import {Component, Input} from '@angular/core';

import {Job} from './job.model';

@Component({
    selector: 'job',
    templateUrl: './job.component.html',
    styleUrls: ['./job.component.scss']
})
export class JobComponent {
    @Input() job: Job;
}
