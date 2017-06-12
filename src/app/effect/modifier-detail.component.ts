import {Component, EventEmitter, Input, Output} from '@angular/core';
import {ActiveStatsModifier} from '../shared/stat-modifier.model';

@Component({
    selector: 'modifier-detail',
    templateUrl: './modifier-detail.component.html',
    styleUrls: ['./modifier-detail.component.scss'],
})
export class ModifierDetailComponent {
    @Input() modifier: ActiveStatsModifier;
    @Output() onRemove: EventEmitter<ActiveStatsModifier> = new EventEmitter<ActiveStatsModifier>();
}
