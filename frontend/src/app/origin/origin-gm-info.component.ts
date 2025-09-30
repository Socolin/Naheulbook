import {Component, Input} from '@angular/core';
import {Origin} from './origin.model';
import {MatButtonModule} from '@angular/material/button';
import {RouterModule} from '@angular/router';

@Component({
    selector: 'app-origin-gm-info',
    templateUrl: './origin-gm-info.component.html',
    imports: [
        MatButtonModule,
        RouterModule
    ],
    styleUrls: ['./origin-gm-info.component.scss']
})
export class OriginGmInfoComponent {
    @Input() origin: Origin;
}
