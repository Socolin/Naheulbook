import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogActions, MatDialogClose, MatDialogContent, MatDialogTitle} from '@angular/material/dialog';
import {MatButton} from '@angular/material/button';
import {MatDivider} from '@angular/material/list';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {DeadMonsterResponse} from '../api/responses/dead-monster-response';
import {GroupService} from './group.service';

export interface DeadMonstersDialogData {
    groupId: number;
}

const PAGE_SIZE = 10;
// Notes longer than this are collapsed by default
const COLLAPSED_NOTE_MAX_LENGTH = 200;

@Component({
    selector: 'app-dead-monsters-dialog',
    templateUrl: './dead-monsters-dialog.component.html',
    styleUrls: ['./dead-monsters-dialog.component.scss'],
    imports: [MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose, MatButton, MatDivider, MatProgressSpinner]
})
export class DeadMonstersDialogComponent implements OnInit {
    public deadMonsters: DeadMonsterResponse[] = [];
    public allLoaded = false;
    public loading = false;
    private readonly expandedNotes = new Set<number>();

    constructor(
        @Inject(MAT_DIALOG_DATA) private readonly data: DeadMonstersDialogData,
        private readonly groupService: GroupService,
    ) {
    }

    ngOnInit(): void {
        this.loadMore();
    }

    loadMore(): void {
        this.loading = true;
        this.groupService.loadDeadMonsters(this.data.groupId, this.deadMonsters.length, PAGE_SIZE).subscribe({
            next: monsters => {
                this.deadMonsters = this.deadMonsters.concat(monsters);
                this.allLoaded = monsters.length < PAGE_SIZE;
                this.loading = false;
            },
            error: () => this.loading = false
        });
    }

    isNoteCollapsible(monster: DeadMonsterResponse): boolean {
        return (monster.data.note?.length ?? 0) > COLLAPSED_NOTE_MAX_LENGTH;
    }

    isNoteExpanded(monster: DeadMonsterResponse): boolean {
        return this.expandedNotes.has(monster.id);
    }

    toggleNote(monster: DeadMonsterResponse): void {
        if (this.expandedNotes.has(monster.id)) {
            this.expandedNotes.delete(monster.id);
        } else {
            this.expandedNotes.add(monster.id);
        }
    }
}
