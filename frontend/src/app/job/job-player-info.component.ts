import {Component, Input, ChangeDetectionStrategy} from '@angular/core';
import {Job} from './job.model';
import { TextFormatterPipe } from '../shared/text-formatter.pipe';

@Component({
    selector: 'app-job-player-info',
    templateUrl: './job-player-info.component.html',
    styleUrls: ['./job-player-info.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [TextFormatterPipe]
})
export class JobPlayerInfoComponent {
    @Input() job: Job;
}
