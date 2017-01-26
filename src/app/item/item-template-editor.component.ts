import {
    Component, Input, OnInit, OnChanges, SimpleChanges, HostListener, ViewChild, QueryList,
    ViewChildren
} from '@angular/core';

import {ItemTemplate, ItemSection, ItemSlot} from '../item';
import {Effect, EffectService} from '../effect';
import {Skill, SkillService} from '../skill';
import {ItemService} from './item.service';
import {Observable} from 'rxjs';
import {JobService} from '../job/job.service';
import {OriginService} from '../origin/origin.service';
import {isNullOrUndefined} from 'util';
import {AutocompleteValue, AutocompleteInputComponent} from '../shared/autocomplete-input.component';
import {Portal, OverlayRef} from '@angular/material';
import {NhbkDialogService} from '../shared/nhbk-dialog.service';
import {removeDiacritics} from '../shared/remove_diacritics';

@Component({
    selector: 'item-template-editor',
    styleUrls: ['./item-template-editor.component.scss'],
    templateUrl: './item-template-editor.component.html'
})
export class ItemTemplateEditorComponent implements OnInit, OnChanges {
    @Input() itemTemplate: ItemTemplate;

    public modules: string[] = [];

    // Datas usefull for forms
    public skills: Skill[] = [];
    public skillsById: { [skillId: number]: Skill } = {};
    public sections: ItemSection[];
    public slots: ItemSlot[];
    public originsName: { [originId: number]: string };
    public jobsName: { [jobId: number]: string };

    public selectedSection: ItemSection;
    public form: { levels: number[], protection: number[], damage: number[], dices: number[] };

    public filteredEffects: Effect[];

    public autocompleteModuleCallback: Observable<AutocompleteValue[]> = this.updateAutocompleteModule.bind(this);

    @ViewChild('autocompleteModuleModalTextField')
    autocompleteModuleModalTextField: AutocompleteInputComponent;

    @ViewChild('addModuleDialog')
    public addModuleDialog: Portal<any>;
    public addModuleOverlayRef: OverlayRef;

    private modulesDef: any[] = [
        {name: 'charge',       displayName: 'Charges/Utilisations'},
        {name: 'container',    displayName: 'Conteneur'},
        {name: 'currency',     displayName: 'Monnaie'},
        {name: 'damage',       displayName: 'Dégât'},
        {name: 'diceDrop',     displayName: 'Dé'},
        {name: 'gem',          displayName: 'Gemme'},
        {name: 'level',        displayName: 'Niveau requis'},
        {name: 'lifetime',     displayName: 'Temps de conservation'},
        {name: 'modifiers',    displayName: 'Modificateurs'},
        {name: 'prereq',       displayName: 'Prérequis'},
        {name: 'protection',   displayName: 'Protection'},
        {name: 'quantifiable', displayName: 'Quantifiable'},
        {name: 'relic',        displayName: 'Relique'},
        {name: 'rupture',      displayName: 'Rupture'},
        {name: 'sex',          displayName: 'Sexe'},
        {name: 'skill',        displayName: 'Compétences'},
        {name: 'skillBook',    displayName: 'Livre de compétences'},
        {name: 'slots',        displayName: 'Equipement'},
        {name: 'throwable',    displayName: 'Prévue pour le jet'},
        {name: 'weight',       displayName: 'Poids'}
    ];

    constructor(private _itemService: ItemService
        , private _effectService: EffectService
        , private _originService: OriginService
        , private _nhbkDialogService: NhbkDialogService
        , private _jobService: JobService
        , private _skillService: SkillService) {
        this.itemTemplate = new ItemTemplate();
        this.form = {
            levels: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
            protection: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
            damage: [-2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
            dices: [1, 2, 3, 4, 5, 6]
        };
    }

    @HostListener('window:keydown', ['$event'])
    keyboardInput(event: any) {
        if (event.target.tagName === 'BODY') {
            if (event.code === 'KeyT') {
                this.openAddModuleDialog();
            }
        }
        if (event.code === 'Escape') {
            this.closeAddModuleDialog();
        }
    }

    openAddModuleDialog() {
        if (this.addModuleOverlayRef == null) {
            this.addModuleOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.addModuleDialog, true);
            this.addModuleOverlayRef.backdropClick().subscribe(() => this.closeAddModuleDialog());
            setTimeout(() => {
                this.autocompleteModuleModalTextField.focus();
            }, 0);
        }
    }

    closeAddModuleDialog() {
        if (this.addModuleOverlayRef) {
            this.addModuleOverlayRef.detach();
            this.addModuleOverlayRef = null;
        }
    }

    updateAutocompleteModule(filter: string) {
        if (!filter) {
            return Observable.of([]);
        }
        filter = removeDiacritics(filter).toLowerCase();
        let filtered = this.modulesDef
            .filter(m => this.modules.indexOf(m.name) === -1)
            .filter(m => (m.name.indexOf(filter) !== -1 || removeDiacritics(m.displayName.toLowerCase()).indexOf(filter) !== -1))
            .map(m => new AutocompleteValue(m, m.displayName));
        return Observable.of(filtered);
    }

    searchEffect(filterName) {
        this._effectService.searchEffect(filterName).subscribe(
            effects => {
                this.filteredEffects = effects;
            }
        );
    }

    automaticNotIdentifiedName() {
        [this.itemTemplate.data.notIdentifiedName] = this.itemTemplate.name.split(' ');
    }

    updateSelectedSection() {
        if (this.itemTemplate && this.sections) {
            for (let i = 0; i < this.sections.length; i++) {
                let section = this.sections[i];
                for (let j = 0; j < section.categories.length; j++) {
                    let category = section.categories[j];
                    if (category.id === this.itemTemplate.category) {
                        this.selectedSection = section;
                        break;
                    }
                }
            }
        }
    }

    ngOnChanges(changes: SimpleChanges): void {
        if ('itemTemplate' in changes) {
            this.updateSelectedSection();
            this.determineModulesFromItemTemplate();
        }
    }

    addModule(module: any) {
        if (this.modules.indexOf(module.name) !== -1) {
            return;
        }

        let moduleName = module.name;
        this.closeAddModuleDialog();

        switch (moduleName) {
            case 'charge':
                this.itemTemplate.data.charge = 1;
                break;
            case 'container':
                this.itemTemplate.data.container = true;
                break;
            case 'currency':
                this.itemTemplate.data.isCurrency = true;
                break;
            case 'damage':
                this.itemTemplate.data.damageDice = 1;
                break;
            case 'diceDrop':
                this.itemTemplate.data.diceDrop = 1;
                break;
            case 'gem':
                this.itemTemplate.data.useUG = true;
                break;
            case 'level':
                this.itemTemplate.data.requireLevel = 1;
                break;
            case 'lifetime':
                this.itemTemplate.data.lifetime = 1;
                this.itemTemplate.data.lifetimeType = 'combat';
                break;
            case 'modifiers':
                this.itemTemplate.modifiers = [];
                break;
            case 'prereq':
                this.itemTemplate.requirements = [];
                break;
            case 'protection':
                this.itemTemplate.data.protection = 1;
                break;
            case 'quantifiable':
                this.itemTemplate.data.quantifiable = true;
                break;
            case 'relic':
                this.itemTemplate.data.relic = true;
                break;
            case 'rupture':
                this.itemTemplate.data.rupture = 1;
                break;
            case 'sex':
                this.itemTemplate.data.sex = 'h';
                break;
            case 'skill':
                this.itemTemplate.skills = [];
                this.itemTemplate.unskills = [];
                this.itemTemplate.skillModifiers = [];
                break;
            case 'skillBook':
                this.itemTemplate.data.skillBook = true;
                break;
            case 'slots':
                this.itemTemplate.slots = [];
                break;
            case 'throwable':
                this.itemTemplate.data.throwable = true;
                break;
            case 'weight':
                this.itemTemplate.data.weight = 1;
                break;
        }

        this.modules.push(moduleName);
    }

    removeModule(moduleName: string) {
        let moduleIndex = this.modules.indexOf(moduleName);
        if (moduleIndex === -1) {
            return;
        }

        switch (moduleName) {
            case 'charge':
                this.itemTemplate.data.charge = null;
                break;
            case 'container':
                this.itemTemplate.data.container = null;
                break;
            case 'currency':
                this.itemTemplate.data.isCurrency = null;
                break;
            case 'damage':
                this.itemTemplate.data.damageDice = null;
                this.itemTemplate.data.bonusDamage = null;
                this.itemTemplate.data.damageType = null;
                break;
            case 'diceDrop':
                this.itemTemplate.data.diceDrop = null;
                break;
            case 'gem':
                this.itemTemplate.data.useUG = null;
                break;
            case 'level':
                this.itemTemplate.data.requireLevel = null;
                break;
            case 'lifetime':
                this.itemTemplate.data.lifetime = null;
                this.itemTemplate.data.lifetimeType = null;
                break;
            case 'modifiers':
                this.itemTemplate.modifiers = null;
                break;
            case 'prereq':
                this.itemTemplate.requirements = null;
                break;
            case 'protection':
                this.itemTemplate.data.protection = null;
                this.itemTemplate.data.protectionAgainstMagic = null;
                this.itemTemplate.data.magicProtection = null;
                this.itemTemplate.data.protectionAgainstType = null;
                break;
            case 'quantifiable':
                this.itemTemplate.data.quantifiable = null;
                break;
            case 'relic':
                this.itemTemplate.data.relic = null;
                break;
            case 'rupture':
                this.itemTemplate.data.rupture = null;
                break;
            case 'sex':
                this.itemTemplate.data.sex = null;
                break;
            case 'skill':
                this.itemTemplate.skills = null;
                this.itemTemplate.unskills = null;
                this.itemTemplate.skillModifiers = null;
                break;
            case 'skillBook':
                this.itemTemplate.data.skillBook = null;
                break;
            case 'slots':
                this.itemTemplate.slots = null;
                break;
            case 'throwable':
                this.itemTemplate.data.throwable = null;
                break;
            case 'weight':
                this.itemTemplate.data.weight = null;
                break;
        }

        this.modules.splice(moduleIndex, 1);
    }

    determineModulesFromItemTemplate(): void {
        let modules: string[] = [];

        if (this.itemTemplate.data.charge) {
            modules.push('charge');
        }
        if (this.itemTemplate.data.container) {
            modules.push('container');
        }
        if (this.itemTemplate.data.isCurrency) {
            modules.push('currency');
        }
        if (this.itemTemplate.data.damageDice || this.itemTemplate.data.bonusDamage) {
            modules.push('damage');
        }
        if (this.itemTemplate.data.diceDrop) {
            modules.push('diceDrop');
        }
        if (this.itemTemplate.data.useUG) {
            modules.push('gem');
        }
        if (this.itemTemplate.data.requireLevel) {
            modules.push('level');
        }
        if (this.itemTemplate.data.lifetime) {
            modules.push('lifetime');
        }
        if (this.itemTemplate.modifiers !== null && this.itemTemplate.modifiers.length) {
            modules.push('modifiers');
        }
        if (this.itemTemplate.requirements !== null
            && this.itemTemplate.requirements.length) {
            modules.push('prereq');
        }
        if (this.itemTemplate.data.protection
            || this.itemTemplate.data.magicProtection
            || this.itemTemplate.data.protectionAgainstMagic) {
            modules.push('protection');
        }
        if (this.itemTemplate.data.quantifiable) {
            modules.push('quantifiable');
        }
        if (this.itemTemplate.data.relic) {
            modules.push('relic');
        }
        if (!isNullOrUndefined(this.itemTemplate.data.rupture)) {
            modules.push('rupture');
        }
        if (this.itemTemplate.data.sex) {
            modules.push('sex');
        }
        if ((this.itemTemplate.skills !== null && this.itemTemplate.skills.length)
            || (this.itemTemplate.unskills !== null && this.itemTemplate.unskills.length)
            || (this.itemTemplate.skillModifiers !== null && this.itemTemplate.skillModifiers.length)) {
            modules.push('skill');
        }
        if (this.itemTemplate.data.skillBook) {
            modules.push('skillBook');
        }
        if (this.itemTemplate.slots !== null && this.itemTemplate.slots.length) {
            modules.push('slots');
        }
        if (this.itemTemplate.data.throwable) {
            modules.push('throwable');
        }
        if (this.itemTemplate.data.weight) {
            modules.push('weight');
        }

        this.modules = modules;
    }

    ngOnInit() {
        if (!this.itemTemplate) {
            this.itemTemplate = new ItemTemplate();
        } else {
            this.determineModulesFromItemTemplate();
        }
        Observable.forkJoin(
            this._itemService.getSectionsList(),
            this._skillService.getSkills(),
            this._skillService.getSkillsById(),
            this._itemService.getSlots(),
            this._jobService.getJobList(),
            this._originService.getOriginList()
        ).subscribe(([sections, skills, skillsById, slots, jobs, origins]) => {
            this.sections = sections;
            this.skills = skills;
            this.skillsById = skillsById;
            this.slots = slots;

            let jobsName: { [jobId: number]: string } = {};
            for (let i = 0; i < jobs.length; i++) {
                let job = jobs[i];
                jobsName[job.id] = job.name;
            }
            this.jobsName = jobsName;

            let originsName: { [originId: number]: string } = {};
            for (let i = 0; i < origins.length; i++) {
                let origin = origins[i];
                originsName[origin.id] = origin.name;
            }
            this.originsName = originsName;

            this.updateSelectedSection();
        });
    }
}
