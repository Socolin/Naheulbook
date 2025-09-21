import {Component, Input} from '@angular/core';
import {Origin} from './origin.model';
import { TextFormatterPipe } from '../shared/text-formatter.pipe';

@Component({
    selector: 'app-origin-player-info',
    templateUrl: './origin-player-info.component.html',
    styleUrls: ['./origin-player-info.component.scss'],
    imports: [TextFormatterPipe]
})
export class OriginPlayerInfoComponent {
    @Input() origin: Origin;
}
