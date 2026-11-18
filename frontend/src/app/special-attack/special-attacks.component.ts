import {ChangeDetectionStrategy, Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {FormsModule} from '@angular/forms';
import {Subscription} from 'rxjs';
import {MatTab, MatTabGroup, MatTabLabel} from '@angular/material/tabs';
import {MatFormField, MatLabel, MatPrefix, MatSuffix} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {MatIcon} from '@angular/material/icon';
import {MatIconButton} from '@angular/material/button';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {SpecialAttackService} from './special-attack.service';
import {SpecialAttackGroupResponse, SpecialAttackResponse} from '../api/responses';

/**
 * Text from the special attack cards: a first line, optionally followed by detail lines starting with "- ".
 */
export interface FormattedText {
    headline?: string;
    details: string[];
    empty: boolean;
}

export interface SpecialAttackView {
    id: string;
    name: string;
    requiredLevel?: number;
    range?: string;
    xp?: number;
    learning?: string;
    usableBy?: FormattedText;
    test?: FormattedText;
    usage?: FormattedText;
    sections: { label: string, text: FormattedText }[];
    criticalFailure?: FormattedText;
    criticalSuccess?: FormattedText;
    criticalNotes?: FormattedText;
    searchText: string;
}

export interface SpecialAttackGroupView {
    id: string;
    name: string;
    descriptionParagraphs: string[];
    specialAttacks: SpecialAttackView[];
    visibleSpecialAttacks: SpecialAttackView[];
}

const EMPTY_VALUES = ['aucun', 'aucune', 'aucun effet', 'non'];

function normalizeForSearch(text: string): string {
    return text.normalize('NFD').replace(/[̀-ͯ]/g, '').toLowerCase();
}

export function formatText(text?: string): FormattedText | undefined {
    if (!text) {
        return undefined;
    }
    const headlines: string[] = [];
    const details: string[] = [];
    for (const line of text.split('\n').map(l => l.trim()).filter(l => l)) {
        if (line.startsWith('- ')) {
            details.push(line.substring(2));
        } else {
            headlines.push(line);
        }
    }
    const headline = headlines.length ? headlines.join(' ') : undefined;
    return {
        headline,
        details,
        empty: !details.length && !!headline && EMPTY_VALUES.includes(headline.toLowerCase()),
    };
}

function toView(specialAttack: SpecialAttackResponse): SpecialAttackView {
    const data = specialAttack.data;
    const sections: { label: string, text: FormattedText }[] = [];
    for (const [label, value] of [['Effets', data.effects], ['Dégâts', data.damage], ['Conditions', data.conditions]]) {
        const text = formatText(value);
        if (text && !text.empty) {
            sections.push({label: label!, text});
        }
    }
    return {
        id: specialAttack.id,
        name: specialAttack.name,
        requiredLevel: data.requiredLevel,
        range: data.range,
        xp: data.xp,
        learning: data.learning,
        usableBy: formatText(data.usableBy),
        test: formatText(data.test),
        usage: formatText(data.usage),
        sections,
        criticalFailure: formatText(data.criticalFailure),
        criticalSuccess: formatText(data.criticalSuccess),
        criticalNotes: formatText(data.criticalNotes),
        searchText: normalizeForSearch([specialAttack.name, data.usableBy, data.effects, data.range].filter(x => x).join(' ')),
    };
}

@Component({
    selector: 'app-special-attacks',
    imports: [
        FormsModule,
        MatTabGroup,
        MatTab,
        MatTabLabel,
        MatFormField,
        MatLabel,
        MatPrefix,
        MatSuffix,
        MatInput,
        MatIcon,
        MatIconButton,
        MatProgressSpinner,
    ],
    templateUrl: './special-attacks.component.html',
    styleUrl: './special-attacks.component.scss',
    changeDetection: ChangeDetectionStrategy.Eager,
})
export class SpecialAttacksComponent implements OnInit, OnDestroy {
    groups?: SpecialAttackGroupView[];
    selectedIndex = 0;
    filter = '';

    private selectedGroupId?: string;
    private readonly subscription = new Subscription();

    constructor(
        private readonly specialAttackService: SpecialAttackService,
        private readonly route: ActivatedRoute,
        private readonly router: Router,
    ) {
    }

    ngOnInit() {
        this.subscription.add(this.route.params.subscribe(params => {
            this.selectedGroupId = params['specialAttackGroupId'];
            this.updateSelectedIndex();
        }));
        this.subscription.add(this.specialAttackService.getSpecialAttackGroups().subscribe(groups => {
            this.groups = groups.map((group: SpecialAttackGroupResponse) => {
                const specialAttacks = group.specialAttacks.map(toView);
                return {
                    id: group.id,
                    name: group.name,
                    descriptionParagraphs: (group.description ?? '').split('\n').map(p => p.trim()).filter(p => p),
                    specialAttacks,
                    visibleSpecialAttacks: specialAttacks,
                };
            });
            this.updateFilter();
            this.updateSelectedIndex();
        }));
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }

    selectTab(index: number) {
        if (!this.groups || index === this.selectedIndex) {
            return;
        }
        this.selectedIndex = index;
        this.router.navigate(['/database/special-attacks', this.groups[index].id], {replaceUrl: true});
    }

    updateFilter() {
        if (!this.groups) {
            return;
        }
        const terms = normalizeForSearch(this.filter).split(/\s+/).filter(t => t);
        for (const group of this.groups) {
            group.visibleSpecialAttacks = group.specialAttacks.filter(a => terms.every(t => a.searchText.includes(t)));
        }
    }

    clearFilter() {
        this.filter = '';
        this.updateFilter();
    }

    private updateSelectedIndex() {
        if (!this.groups || !this.selectedGroupId) {
            return;
        }
        const index = this.groups.findIndex(g => g.id === this.selectedGroupId);
        if (index !== -1) {
            this.selectedIndex = index;
        }
    }
}
