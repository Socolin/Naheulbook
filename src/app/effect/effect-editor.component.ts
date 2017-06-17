import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {MdOptionSelectionChange, OverlayRef, Portal} from '@angular/material';

import {EffectService} from './effect.service';
import {Effect, EffectCategory, EffectType} from './effect.model';
import {NhbkDialogService} from '../shared/nhbk-dialog.service';

@Component({
    selector: 'effect-editor',
    styleUrls: ['./effect-editor.component.scss'],
    templateUrl: './effect-editor.component.html',
})
export class EffectEditorComponent implements OnInit {
    @Input() effect: Effect;
    public types: EffectType[] = [];
    public selectedType: EffectType;

    @ViewChild('createCategoryDialog')
    public createCategoryDialog: Portal<any>;
    public createCategorOverlayRef: OverlayRef;

    @ViewChild('createTypeDialog')
    public createTypeDialog: Portal<any>;
    public createTypeOverlayRef: OverlayRef;

    constructor(private _effectService: EffectService
        , private _nhbkDialogService: NhbkDialogService) {
        this.effect = new Effect();
    }

    openCreateCategoryDialog() {
        this.createCategorOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.createCategoryDialog);
    }

    closeCreateCategoryDialog() {
        this.createCategorOverlayRef.detach();
    }

    createCategory(name: string) {
        this.closeCreateCategoryDialog();
        this._effectService.createCategory(this.selectedType, name).subscribe(
            category => {
                this.selectedType.categories.push(category);
                this.effect.category = category;
            });
    }

    openCreateTypeDialog() {
        this.createTypeOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.createTypeDialog);
    }

    closeCreateTypeDialog() {
        this.createTypeOverlayRef.detach();
    }

    createType(name: string) {
        this.closeCreateTypeDialog();
        this._effectService.createType(name).subscribe(
            type => {
                this.types.push(type);
                this.selectType(type);
            });
    }

    selectCategory(category: EffectCategory) {
        if (category === undefined) {
            this.openCreateCategoryDialog();
        }
        this.effect.category = category;
    }

    selectType(type: EffectType) {
        if (type === undefined) {
            this.openCreateTypeDialog();
        }
        this.selectedType = type;
        if (this.selectedType.categories.length) {
            this.effect.category = this.selectedType.categories[0];
        }
    }

    ngOnInit() {
        this._effectService.getEffectTypes().subscribe(types => {
            this.types = types;
            if (this.effect.category) {
                this.selectedType = this.effect.category.type;
            }
            else if (types.length) {
                this.selectType(this.types[0]);
            }
        });
    }
}
