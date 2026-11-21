import {ChangeDetectionStrategy, Component, Input, OnChanges} from '@angular/core';
import {MatIcon} from '@angular/material/icon';
import {MarkdownPipe} from '../markdown/markdown.pipe';
import {SpellBookKind, SpellResponse} from '../api/responses';
import {
    creatureStats,
    invocationQuickInfos,
    SPELL_KIND_LABELS,
    SpellListSection,
    spellListSections,
    SpellQuickInfo,
    spellQuickInfos
} from './spell-view';

/**
 * Card displaying a spell / prodige / ritual / invocation: the main values are always visible,
 * the rest of the book text is in a collapsible "details" section.
 */
@Component({
    selector: 'spell-card',
    imports: [MatIcon, MarkdownPipe],
    templateUrl: './spell-card.component.html',
    styleUrl: './spell-card.component.scss',
    changeDetection: ChangeDetectionStrategy.Eager,
})
export class SpellCardComponent implements OnChanges {
    @Input({required: true}) spell!: SpellResponse;
    @Input({required: true}) bookKind!: SpellBookKind;

    kindLabel = '';
    quickInfos: SpellQuickInfo[] = [];
    invocationInfos: SpellQuickInfo[] = [];
    creatureStats: { label: string, value: string }[] = [];
    listSections: SpellListSection[] = [];
    hasDetails = false;

    ngOnChanges() {
        const spell = this.spell;
        this.kindLabel = SPELL_KIND_LABELS[spell.kind] ?? spell.kind;
        this.quickInfos = spellQuickInfos(spell);
        this.invocationInfos = invocationQuickInfos(spell);
        this.creatureStats = creatureStats(spell);
        this.listSections = spellListSections(spell, this.bookKind);
        this.hasDetails = this.listSections.length > 0
            || this.invocationInfos.length > 0
            || this.creatureStats.length > 0
            || !!spell.data.ritualDetails
            || !!spell.data.notes;
    }
}
