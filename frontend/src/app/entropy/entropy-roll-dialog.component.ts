import {ChangeDetectionStrategy, Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogActions, MatDialogClose, MatDialogContent, MatDialogRef, MatDialogTitle} from '@angular/material/dialog';
import {MatButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatFormField, MatLabel} from '@angular/material/form-field';
import {MatOption, MatSelect} from '@angular/material/select';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {FormsModule} from '@angular/forms';

import {EntropyEventResponse} from '../api/responses';
import {getRandomInt} from '../shared';
import {EntropyKind, entropyPointsNeeded, EntropyService} from './entropy.service';

export interface EntropyRollDialogCharacter {
    id: number;
    name: string;
}

export interface EntropyRollDialogData {
    groupId: number;
    kind: EntropyKind;
    points: number;
    characters: EntropyRollDialogCharacter[];
}

export interface EntropyRollDialogResult {
    applied: true;
}

interface CharacterRoll {
    character: EntropyRollDialogCharacter;
    /** One roll per round: several rounds happen when characters are tied */
    rolls: number[];
}

@Component({
    templateUrl: './entropy-roll-dialog.component.html',
    styleUrls: ['./entropy-roll-dialog.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose, MatButton, MatIcon, MatFormField, MatLabel, MatSelect, MatOption, MatProgressSpinner, FormsModule]
})
export class EntropyRollDialogComponent implements OnInit {
    public readonly pointsNeeded = entropyPointsNeeded;
    public readonly isMankdebol: boolean;
    public readonly title: string;
    public readonly characterRule: string;

    public loading = true;
    public saving = false;
    public error?: string;

    public entries: EntropyEventResponse[] = [];
    public diceRoll?: number;
    public selectedEvent?: EntropyEventResponse;

    public characterRolls: CharacterRoll[] = [];
    public selectedCharacterId?: number;

    constructor(
        private readonly dialogRef: MatDialogRef<EntropyRollDialogComponent, EntropyRollDialogResult>,
        private readonly entropyService: EntropyService,
        @Inject(MAT_DIALOG_DATA) public readonly data: EntropyRollDialogData,
    ) {
        this.isMankdebol = data.kind === 'mankdebol';
        this.title = this.isMankdebol ? 'Calamité de Mankdebol' : 'Bienfait de Débilibeuk';
        this.characterRule = this.isMankdebol
            ? 'Le personnage concerné est celui qui fait le plus petit score au D6'
            : 'Le personnage concerné est celui qui fait le plus haut score au D6';
    }

    ngOnInit(): void {
        this.entropyService.getEntropyTables().subscribe({
            next: tables => {
                this.entries = this.isMankdebol ? tables.mankdebolCalamities : tables.debilibeukBlessings;
                this.loading = false;
                this.rollEvent();
            },
            error: () => {
                this.loading = false;
                this.error = 'Impossible de charger la table';
            }
        });
    }

    rollEvent(): void {
        if (this.entries.length === 0) {
            return;
        }
        this.diceRoll = getRandomInt(1, 100);
        // The official calamity table has 2 entries on the same roll: pick one of them
        const matching = this.entries.filter(e => e.diceRoll === this.diceRoll);
        this.selectedEvent = matching.length
            ? matching[getRandomInt(0, matching.length - 1)]
            : undefined;
    }

    selectEvent(event: EntropyEventResponse): void {
        this.selectedEvent = event;
        this.diceRoll = undefined;
    }

    rollCharacter(): void {
        this.characterRolls = this.data.characters.map(character => ({character, rolls: []}));
        let contenders = this.characterRolls;
        while (contenders.length > 0) {
            for (const contender of contenders) {
                contender.rolls.push(getRandomInt(1, 6));
            }
            const scores = contenders.map(c => c.rolls[c.rolls.length - 1]);
            const best = this.isMankdebol ? Math.min(...scores) : Math.max(...scores);
            contenders = contenders.filter(c => c.rolls[c.rolls.length - 1] === best);
            if (contenders.length === 1) {
                this.selectedCharacterId = contenders[0].character.id;
                break;
            }
        }
    }

    isSelectedCharacter(characterRoll: CharacterRoll): boolean {
        return characterRoll.character.id === this.selectedCharacterId;
    }

    validate(): void {
        if (!this.selectedEvent || this.saving) {
            return;
        }
        this.saving = true;
        this.error = undefined;
        this.entropyService.applyEntropyEvent(this.data.groupId, this.data.kind, {
            eventId: this.selectedEvent.id,
            characterId: this.selectedCharacterId,
        }).subscribe({
            next: () => this.dialogRef.close({applied: true}),
            error: () => {
                this.saving = false;
                this.error = 'Erreur lors de l\'enregistrement';
            }
        });
    }
}
