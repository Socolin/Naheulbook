import {Component, OnDestroy, OnInit} from '@angular/core';
import {OriginService} from './origin.service';
import {MatLegacyDialogRef as MatDialogRef} from '@angular/material/legacy-dialog';
import {Origin} from './origin.model';
import {UntypedFormControl, UntypedFormGroup, Validators} from '@angular/forms';
import {BehaviorSubject, combineLatest, of, Subscription} from 'rxjs';
import {catchError, distinctUntilChanged, filter, map, switchMap} from 'rxjs/operators';
import {CharacterSex} from '../api/shared/enums';

export interface NameGeneratorDialogResult {
    name: string;
    originName: string;
    sex: CharacterSex;
}

@Component({
    templateUrl: './name-generator-dialog.component.html',
    styleUrls: ['./name-generator-dialog.component.scss']
})
export class NameGeneratorDialogComponent implements OnInit, OnDestroy {
    private onClick = new BehaviorSubject<boolean>(true);
    private subscription = new Subscription();
    public origins: Origin[] = [];
    public form = new UntypedFormGroup({
        originId: new UntypedFormControl(undefined, Validators.required),
        sex: new UntypedFormControl('Homme', Validators.required),
        name: new UntypedFormControl(undefined, Validators.required),
    });

    constructor(
        private readonly originService: OriginService,
        private readonly dialogRef: MatDialogRef<NameGeneratorDialogComponent, NameGeneratorDialogResult>
    ) {
    }

    valid() {
        this.dialogRef.close({
            ...this.form.value,
            originName: this.origins.find(x => x.id === this.form.value.originId)!.name
        });
    }

    ngOnInit() {
        const formChange$ = this.form.valueChanges.pipe(
            map(value => ({originId: value.originId, sex: value.sex})),
            distinctUntilChanged((a, b) => a.originId === b.originId && a.sex === b.sex),
            filter(value => value.originId && value.sex)
        );
        this.subscription.add(combineLatest([formChange$, this.onClick])
            .pipe(
                map(([formValue]) => formValue),
                switchMap((value) => {
                    return this.originService.getRandomName(value.originId, value.sex).pipe(
                        catchError(err => of(''))
                    )
                })
            ).subscribe((name) => {
                this.form.controls['name'].setValue(name)
            }));
        this.originService.getOriginList().subscribe(origins => {
            this.origins = origins;
            this.form.controls['originId'].setValue(origins[0].id);
        });
    }

    ngOnDestroy(): void {
        this.subscription.unsubscribe();
    }

    randomName() {
        this.onClick.next(true);
    }
}
