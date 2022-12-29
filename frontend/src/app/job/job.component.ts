import {Component, Input} from '@angular/core';

import {Job} from './job.model';
import {GmModeService} from '../shared';

@Component({
    selector: 'job',
    templateUrl: './job.component.html',
    styleUrls: ['./job.component.scss']
})
export class JobComponent {
    @Input() job: Job;

    constructor(public readonly gmModeService: GmModeService) {
    }
}
