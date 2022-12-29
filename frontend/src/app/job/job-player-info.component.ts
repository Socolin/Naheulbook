import {Component, Input} from '@angular/core';
import {Job} from './job.model';

@Component({
    selector: 'app-job-player-info',
    templateUrl: './job-player-info.component.html',
    styleUrls: ['./job-player-info.component.scss']
})
export class JobPlayerInfoComponent {
    @Input() job: Job;
}
