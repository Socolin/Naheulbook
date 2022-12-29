import {Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges,} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {Subscription} from 'rxjs';

import {LoginService} from '../user';
import {NhbkMatDialog} from '../material-workaround';

import {EffectService} from './effect.service';
import {Effect, EffectSubCategory, EffectType} from './effect.model';
import {EditEffectDialogComponent, EditEffectDialogData} from './edit-effect-dialog.component';

@Component({
    selector: 'effect-list',
    templateUrl: './effect-list.component.html',
    styleUrls: ['../shared/number-shadow.scss', './effect-list.component.scss'],
})
export class EffectListComponent implements OnInit, OnChanges, OnDestroy {
    @Input() inputSubCategoryId: number | undefined;
    @Input() isOverlay = false;
    @Input() options: string[] = [];
    @Output() onAction = new EventEmitter<{ action: string, data: any }>();

    public effectTypes: EffectType[];
    public selectedType: EffectType;
    public selectedSubCategory?: EffectSubCategory;
    public effects: { [subcategoryId: number]: Effect[] } = {};
    public editable = false;
    public sub: Subscription;

    constructor(
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly loginService: LoginService,
        private readonly effectService: EffectService,
        private readonly dialog: NhbkMatDialog
    ) {
    }

    selectType(type: EffectType) {
        this.selectedType = type;
        if (this.selectedType.subCategories.length) {
            this.selectSubCategory(this.selectedType.subCategories[0]);
        }
    }

    selectSubCategory(subCategory?: EffectSubCategory) {
        this.selectedSubCategory = subCategory;
        if (subCategory) {
            this.loadSubCategory(subCategory.id);
        }
    }

    selectSubCategoryId(subCategoryId: number) {
        let i = this.effectTypes.findIndex(t => t.subCategories.findIndex(c => c.id === subCategoryId) !== -1);
        if (i === -1) {
            return;
        }
        this.selectedType = this.effectTypes[i];
        let ci = this.selectedType.subCategories.findIndex(c => c.id === subCategoryId);
        this.selectSubCategory(this.selectedType.subCategories[ci]);
    }

    hasOption(option: string): boolean {
        return this.options.indexOf(option) !== -1;
    }

    emitAction(actionName: string, data: any) {
        this.onAction.emit({action: actionName, data: data});
    }

    editEffect(effect: Effect) {
        const dialogRef = this.dialog.openFullScreen<EditEffectDialogComponent, EditEffectDialogData, Effect>(
            EditEffectDialogComponent, {
                data: {effect}
            });

        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }
            const previousEffectSubCategory = this.effects[effect.subCategory.id];
            const index = previousEffectSubCategory.findIndex(e => e.id === result.id);
            if (effect.subCategory.id === result.subCategory.id) {
                if (index !== -1) {
                    previousEffectSubCategory[index] = result;
                }
            } else {
                if (index !== -1) {
                    previousEffectSubCategory.splice(index, 1);
                }
                if (!(effect.subCategory.id in this.effects)) {
                    this.effects[effect.subCategory.id] = [];
                }
                this.effects[effect.subCategory.id].push(result);
                this.effects[effect.subCategory.id].sort((a, b) => {
                    if (a.dice && b.dice) {
                        return a.dice - b.dice;
                    }
                    return a.id - b.id;
                });
                this.selectSubCategory(result.subCategory);
            }
        })
    }

    createEffect() {
        const dialogRef = this.dialog.openFullScreen<EditEffectDialogComponent, EditEffectDialogData, Effect>(
            EditEffectDialogComponent, {
                data: {subCategory: this.selectedSubCategory}
            });

        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }
        })
    }

    loadSubCategory(subCategoryId: number): void {
        this.effectService.getEffects(subCategoryId).subscribe(
            effects => {
                this.effects[subCategoryId] = effects;
                this.effects[subCategoryId].sort((a, b) => {
                    if (a.dice && b.dice) {
                        return a.dice - b.dice;
                    }
                    return a.id - b.id
                });
            }
        );
    }

    ngOnChanges(changes: SimpleChanges) {
        if ('inputSubCategoryId' in changes && !changes['inputSubCategoryId'].isFirstChange()) {
            this.selectSubCategoryId(+changes['inputSubCategoryId'].currentValue);
        }
    }

    ngOnInit() {
        this.effectService.getEffectTypes().subscribe(types => {
            this.effectTypes = types;
            if (types.length) {
                this.selectType(types[0]);
            }
            if (this.isOverlay && this.inputSubCategoryId != null) {
                this.selectSubCategoryId(this.inputSubCategoryId);
            } else {
                if (!this.route.snapshot.data['id'] && this.selectedSubCategory) {
                    this.loadSubCategory(this.selectedSubCategory.id);
                }
                this.sub = this.route.queryParams.subscribe(params => {
                    this.selectSubCategoryId(+params['id']);
                });
            }
        });
        if (!this.isOverlay) {
            this.loginService.loggedUser.subscribe(
                user => {
                    this.editable = (user != null && user.admin);
                });
        }
    }

    ngOnDestroy() {
        if (this.sub) {
            this.sub.unsubscribe();
        }
    }
}
