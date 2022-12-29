import {Component, Input} from '@angular/core';
import {Origin} from './origin.model';

@Component({
    selector: 'app-origin-player-info',
    templateUrl: './origin-player-info.component.html',
    styleUrls: ['./origin-player-info.component.scss'],
})
export class OriginPlayerInfoComponent {
    @Input() origin: Origin;
}
