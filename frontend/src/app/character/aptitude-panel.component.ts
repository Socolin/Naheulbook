import {Component, Input} from '@angular/core';
import {MatCardModule} from '@angular/material/card';
import {MatButtonModule} from '@angular/material/button';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatIconModule} from '@angular/material/icon';
import {Character} from './character.model';
import {NhbkMatDialog} from '../material-workaround';
import {AddAptitudeDialogComponent, AddAptitudeDialogData, AddAptitudeDialogResult} from './add-aptitude-dialog.component';
import {AptitudeService} from '../aptitude/aptitude.service';
import {firstValueFrom} from 'rxjs';
import {MatCheckbox} from '@angular/material/checkbox';
import {MatMenuModule} from '@angular/material/menu';
import {CharacterAptitudeResponse} from '../api/responses';

@Component({
    selector: 'aptitude-panel',
    imports: [
        MatCardModule,
        MatIconModule,
        MatButtonModule,
        MatToolbarModule,
        MatCheckbox,
        MatMenuModule,
    ],
    templateUrl: './aptitude-panel.component.html',
    styleUrl: './aptitude-panel.component.scss'
})
export class AptitudePanelComponent {
    @Input() character: Character;

    constructor(
        private readonly dialog: NhbkMatDialog,
        private readonly aptitudeService: AptitudeService,
    ) {
    }

    openAddAptitudeDialog() {

        const dialogRef = this.dialog.openFullScreen<AddAptitudeDialogComponent, AddAptitudeDialogData, AddAptitudeDialogResult>(
            AddAptitudeDialogComponent,
            {
                data: {
                    aptitudeGroupId: this.character.origin.aptitudeGroupId
                }
            }
        );
        dialogRef.afterClosed().subscribe(async result => {
            if (!result) {
                return;
            }
            for (let selectedAptitudeId of result.selectedAptitudeIds) {
                let response = await firstValueFrom(this.aptitudeService.addCharacterAptitude(this.character.id, {aptitudeId: selectedAptitudeId}))
                this.character.updateAptitude(response.aptitude, response.count, response.active)
            }
        });
    }


    removeCharacterAptitude(characterAptitude: CharacterAptitudeResponse) {
        let count = characterAptitude.count - 1;
        this.aptitudeService.removeCharacterAptitude(this.character.id, characterAptitude.aptitude.id).subscribe(() => {
            this.character.updateAptitude(characterAptitude.aptitude, count, false);
        })
    }

    updateActiveAptitude(characterAptitude: CharacterAptitudeResponse, active: boolean) {
        console.log('updateActiveAptitude', characterAptitude, active);
        this.aptitudeService.updateCharacterAptitude(this.character.id, characterAptitude.aptitude.id, {active: active}).subscribe(() => {
            this.character.updateAptitude(characterAptitude.aptitude, characterAptitude.count, active);
        })
    }
}
