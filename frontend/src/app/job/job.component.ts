import {Component, Input} from '@angular/core';

import {Job} from './job.model';
import {GmModeService} from '../shared';
import { MatExpansionPanel, MatExpansionPanelHeader } from '@angular/material/expansion';
import { JobPlayerInfoComponent } from './job-player-info.component';
import { JobGmInfoComponent } from './job-gm-info.component';
import { AsyncPipe } from '@angular/common';

@Component({
    selector: 'job',
    templateUrl: './job.component.html',
    styleUrls: ['./job.component.scss'],
    imports: [MatExpansionPanel, MatExpansionPanelHeader, JobPlayerInfoComponent, JobGmInfoComponent, AsyncPipe]
})
export class JobComponent {
    @Input() job: Job;

    constructor(public readonly gmModeService: GmModeService) {
    }
}
