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
import {normalizeForSearch, specialAttackSearchText} from './special-attack-view';
import {SpecialAttackCardComponent} from './special-attack-card.component';

interface SpecialAttackItem {
    id: string;
    specialAttack: SpecialAttackResponse;
    searchText: string;
}

export interface SpecialAttackGroupView {
    id: string;
    name: string;
    descriptionParagraphs: string[];
    specialAttacks: SpecialAttackItem[];
    visibleSpecialAttacks: SpecialAttackItem[];
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
        SpecialAttackCardComponent,
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
                const specialAttacks = group.specialAttacks.map(specialAttack => ({
                    id: specialAttack.id,
                    specialAttack,
                    searchText: specialAttackSearchText(specialAttack),
                }));
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
