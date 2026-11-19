import {ChangeDetectionStrategy, Component, EventEmitter, Input, Output} from '@angular/core';
import {MatCardModule} from '@angular/material/card';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatButtonModule} from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import {MatMenuModule} from '@angular/material/menu';
import {MatTooltip} from '@angular/material/tooltip';
import {EntryView} from './entry-group-tabs.model';

/**
 * Card of the character sheet listing what the character has (talents, masteries, handicaps), with add / remove actions
 */
@Component({
    selector: 'character-entry-panel',
    imports: [
        MatCardModule,
        MatToolbarModule,
        MatButtonModule,
        MatIconModule,
        MatMenuModule,
        MatTooltip,
    ],
    templateUrl: './character-entry-panel.component.html',
    styleUrl: './character-entry-panel.component.scss',
    changeDetection: ChangeDetectionStrategy.Eager,
})
export class CharacterEntryPanelComponent {
    @Input({required: true}) title!: string;
    @Input({required: true}) entries!: EntryView[];
    @Input({required: true}) addLabel!: string;
    @Input({required: true}) emptyText!: string;
    /** Ids of the entries whose conditions (level, prerequisite) are not met anymore */
    @Input() warnings: Record<string, string[]> = {};

    @Output() add = new EventEmitter<void>();
    @Output() remove = new EventEmitter<EntryView>();
}
