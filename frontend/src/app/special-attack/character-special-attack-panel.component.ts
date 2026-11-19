import {ChangeDetectionStrategy, Component, Input} from '@angular/core';
import {firstValueFrom} from 'rxjs';
import {MatCardModule} from '@angular/material/card';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatButtonModule} from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import {MatMenuModule} from '@angular/material/menu';
import {MatTooltip} from '@angular/material/tooltip';
import {Character} from '../character/character.model';
import {NhbkMatDialog} from '../material-workaround';
import {SpecialAttackService} from './special-attack.service';
import {CharacterSpecialAttack} from './character-special-attack.model';
import {SpecialAttackCardComponent} from './special-attack-card.component';
import {
    AddSpecialAttackDialogComponent,
    AddSpecialAttackDialogData,
    AddSpecialAttackDialogResult
} from './add-special-attack-dialog.component';
import {duration2shortText} from '../date/util';

type AvailabilityStatus = 'available' | 'cooldown' | 'failure' | 'unlimited';

@Component({
    selector: 'character-special-attack-panel',
    imports: [
        MatCardModule,
        MatToolbarModule,
        MatButtonModule,
        MatIconModule,
        MatMenuModule,
        MatTooltip,
        SpecialAttackCardComponent,
    ],
    templateUrl: './character-special-attack-panel.component.html',
    styleUrl: './character-special-attack-panel.component.scss',
    changeDetection: ChangeDetectionStrategy.Eager,
})
export class CharacterSpecialAttackPanelComponent {
    @Input({required: true}) character: Character;

    expandedId?: string;
    pendingIds = new Set<string>();

    constructor(
        private readonly dialog: NhbkMatDialog,
        private readonly specialAttackService: SpecialAttackService,
    ) {
    }

    get sortedSpecialAttacks(): CharacterSpecialAttack[] {
        return [...this.character.specialAttacks].sort((a, b) => a.specialAttack.name.localeCompare(b.specialAttack.name));
    }

    status(characterSpecialAttack: CharacterSpecialAttack): AvailabilityStatus {
        if (!characterSpecialAttack.hasUsageLimit) {
            return 'unlimited';
        }
        if (characterSpecialAttack.isAvailable) {
            return 'available';
        }
        return characterSpecialAttack.blockingCooldown?.failure ? 'failure' : 'cooldown';
    }

    statusText(characterSpecialAttack: CharacterSpecialAttack): string {
        switch (this.status(characterSpecialAttack)) {
            case 'unlimited':
                return 'Sans limite';
            case 'available': {
                const uses = characterSpecialAttack.specialAttack.data.usesPerCooldown ?? 1;
                return uses > 1 ? `Disponible (${characterSpecialAttack.remainingUses}/${uses})` : 'Disponible';
            }
            default: {
                const cooldown = characterSpecialAttack.blockingCooldown;
                if (!cooldown) {
                    return 'Indisponible';
                }
                const prefix = cooldown.failure ? 'Échec · ' : '';
                switch (cooldown.durationType) {
                    case 'time':
                        return prefix + 'Recharge : ' + duration2shortText(cooldown.currentTimeDuration ?? 0);
                    case 'combat':
                        return prefix + ((cooldown.currentCombatCount ?? 1) > 1 ? `Dans ${cooldown.currentCombatCount} combats` : 'Au prochain combat');
                    default:
                        return prefix + 'En recharge';
                }
            }
        }
    }

    toggleDetails(specialAttackId: string) {
        this.expandedId = this.expandedId === specialAttackId ? undefined : specialAttackId;
    }

    openAddDialog() {
        const dialogRef = this.dialog.openFullScreen<AddSpecialAttackDialogComponent, AddSpecialAttackDialogData, AddSpecialAttackDialogResult>(
            AddSpecialAttackDialogComponent,
            {
                data: {
                    characterLevel: this.character.level,
                    knownSpecialAttacks: this.character.specialAttacks.map(s => ({
                        specialAttackId: s.specialAttack.id,
                        successCount: s.successCount,
                    })),
                }
            }
        );
        dialogRef.afterClosed().subscribe(async result => {
            if (!result) {
                return;
            }
            for (const specialAttackId of result.selectedSpecialAttackIds) {
                const response = await firstValueFrom(this.specialAttackService.addCharacterSpecialAttack(this.character.id, {specialAttackId}));
                this.character.updateSpecialAttack(response);
            }
        });
    }

    use(characterSpecialAttack: CharacterSpecialAttack, success: boolean) {
        const specialAttackId = characterSpecialAttack.specialAttack.id;
        this.pendingIds.add(specialAttackId);
        this.specialAttackService.useCharacterSpecialAttack(this.character.id, specialAttackId, {success}).subscribe({
            next: response => {
                this.character.updateSpecialAttack(response);
                // A success gives the XP of the special attack (the change is also logged in the character history)
                this.character.onChangeCharacterStat({stat: 'experience', value: response.experience});
                this.pendingIds.delete(specialAttackId);
            },
            error: () => this.pendingIds.delete(specialAttackId),
        });
    }

    resetCooldowns(characterSpecialAttack: CharacterSpecialAttack) {
        this.specialAttackService.updateCharacterSpecialAttack(this.character.id, characterSpecialAttack.specialAttack.id, {resetCooldowns: true})
            .subscribe(response => this.character.updateSpecialAttack(response));
    }

    remove(characterSpecialAttack: CharacterSpecialAttack) {
        const specialAttackId = characterSpecialAttack.specialAttack.id;
        this.specialAttackService.removeCharacterSpecialAttack(this.character.id, specialAttackId)
            .subscribe(() => this.character.removeSpecialAttack(specialAttackId));
    }
}
