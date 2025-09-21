import {Component, Inject, OnInit} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import {ItemTemplateModuleDefinition, itemTemplateModulesDefinitions} from './item-template-modules-definitions';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { MatRipple } from '@angular/material/core';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';

export interface AddItemTemplateEditorModuleDialogData {
    selectedModules: string[]
}

@Component({
    templateUrl: './add-item-template-editor-module-dialog.component.html',
    styleUrls: ['./add-item-template-editor-module-dialog.component.scss'],
    imports: [CdkScrollable, MatDialogContent, MatRipple, MatIcon, MatDialogActions, MatButton, MatDialogClose]
})
export class AddItemTemplateEditorModuleDialogComponent implements OnInit {
    public availableModules: ItemTemplateModuleDefinition[] = [];

    constructor(
        public dialogRef: MatDialogRef<AddItemTemplateEditorModuleDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: AddItemTemplateEditorModuleDialogData,
    ) {
    }

    ngOnInit() {
        this.availableModules = itemTemplateModulesDefinitions.filter(m => this.data.selectedModules.indexOf(m.name) === -1)
    }

    selectModule(module: ItemTemplateModuleDefinition) {
        this.dialogRef.close(module.name);
    }
}
