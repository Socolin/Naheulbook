import {Component, Input, OnInit} from '@angular/core';
import {Job} from './job.model';
import { PercentPipe } from '@angular/common';

@Component({
    selector: 'app-job-gm-info',
    templateUrl: './job-gm-info.component.html',
    styleUrls: ['./job-gm-info.component.scss'],
    imports: [PercentPipe]
})
export class JobGmInfoComponent implements OnInit {
    @Input() job: Job;

    constructor() {
    }

    ngOnInit() {
    }

}
