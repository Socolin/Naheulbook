import {Component, Input} from '@angular/core';
import {Origin} from './origin.model';
import {GmModeService} from '../shared';
import { MatExpansionPanel, MatExpansionPanelHeader } from '@angular/material/expansion';
import { OriginPlayerInfoComponent } from './origin-player-info.component';
import { OriginGmInfoComponent } from './origin-gm-info.component';
import { AsyncPipe } from '@angular/common';

@Component({
    selector: 'origin',
    templateUrl: './origin.component.html',
    styleUrls: ['./origin.component.scss'],
    imports: [MatExpansionPanel, MatExpansionPanelHeader, OriginPlayerInfoComponent, OriginGmInfoComponent, AsyncPipe]
})
export class OriginComponent {
    @Input() origin: Origin;

    constructor(public readonly gmModeService: GmModeService) {
    }
}
