import {Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges,} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {Subscription} from 'rxjs';

import {LoginService} from '../user';
import {NhbkMatDialog} from '../material-workaround';

import {EffectService} from './effect.service';
import {Effect, EffectCategory, EffectType} from './effect.model';
import {EditEffectDialogComponent, EditEffectDialogData} from './edit-effect-dialog.component';

@Component({
    selector: 'effect-list',
    templateUrl: './effect-list.component.html',
    styleUrls: ['../shared/number-shadow.scss', './effect-list.component.scss'],
})
export class EffectListComponent implements OnInit, OnChanges, OnDestroy {
    @Input() inputCategoryId: number | undefined;
    @Input() isOverlay = false;
    @Input() options: string[] = [];
    @Output() onAction = new EventEmitter<{ action: string, data: any }>();

    public effectTypes: EffectType[];
    public selectedType: EffectType;
    public selectedCategory?: EffectCategory;
    public effects: { [categoryId: number]: Effect[] } = {};
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
        if (this.selectedType.categories.length) {
            this.selectCategory(this.selectedType.categories[0]);
        }
    }

    selectCategory(category?: EffectCategory) {
        this.selectedCategory = category;
        if (category) {
            this.loadCategory(category.id);
        }
    }

    selectCategoryId(categoryId: number) {
        let i = this.effectTypes.findIndex(t => t.categories.findIndex(c => c.id === categoryId) !== -1);
        if (i === -1) {
            return;
        }
        this.selectedType = this.effectTypes[i];
        let ci = this.selectedType.categories.findIndex(c => c.id === categoryId);
        this.selectCategory(this.selectedType.categories[ci]);
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
            const previousEffectCategory = this.effects[effect.category.id];
            const index = previousEffectCategory.findIndex(e => e.id === result.id);
            if (effect.category.id === result.category.id) {
                if (index !== -1) {
                    previousEffectCategory[index] = result;
                }
            } else {
                if (index !== -1) {
                    previousEffectCategory.splice(index, 1);
                }
                if (!(effect.category.id in this.effects)) {
                    this.effects[effect.category.id] = [];
                }
                this.effects[effect.category.id].push(result);
                this.effects[effect.category.id].sort((a, b) => {
                    if (a.dice && b.dice) {
                        return a.dice - b.dice;
                    }
                    return a.id - b.id;
                });
                this.selectCategory(result.category);
            }
        })
    }

    createEffect() {
        const dialogRef = this.dialog.openFullScreen<EditEffectDialogComponent, EditEffectDialogData, Effect>(
            EditEffectDialogComponent, {
                data: {category: this.selectedCategory}
            });

        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }
        })
    }

    loadCategory(categoryId: number): void {
        this.effectService.getEffects(categoryId).subscribe(
            effects => {
                this.effects[categoryId] = effects;
                this.effects[categoryId].sort((a, b) => {
                    if (a.dice && b.dice) {
                        return a.dice - b.dice;
                    }
                    return a.id - b.id
                });
            }
        );
    }

    ngOnChanges(changes: SimpleChanges) {
        if ('inputCategoryId' in changes && !changes['inputCategoryId'].isFirstChange()) {
            this.selectCategoryId(+changes['inputCategoryId'].currentValue);
        }
    }

    ngOnInit() {
        this.effectService.getEffectTypes().subscribe(types => {
            this.effectTypes = types;
            if (types.length) {
                this.selectType(types[0]);
            }
            if (this.isOverlay && this.inputCategoryId != null) {
                this.selectCategoryId(this.inputCategoryId);
            } else {
                if (!this.route.snapshot.data['id'] && this.selectedCategory) {
                    this.loadCategory(this.selectedCategory.id);
                }
                this.sub = this.route.queryParams.subscribe(params => {
                    this.selectCategoryId(+params['id']);
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
