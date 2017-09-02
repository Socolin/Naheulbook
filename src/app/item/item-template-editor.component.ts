import {
    Component, Input, OnInit, OnChanges, SimpleChanges, HostListener, ViewChild
} from '@angular/core';
import {Portal, OverlayRef} from '@angular/material';
import {Observable} from 'rxjs';

import {isNullOrUndefined} from 'util';
import {
    removeDiacritics, NhbkDialogService, AutocompleteValue, AutocompleteInputComponent
} from '../shared';
import {LoginService} from '../user';
import {Skill, SkillService} from '../skill';
import {JobService} from '../job';
import {OriginService} from '../origin';

import {ItemSection, ItemSlot, ItemTemplate, ItemTemplateGunData, ItemType} from './item-template.model';
import {ItemTemplateService} from './item-template.service'

@Component({
    selector: 'item-template-editor',
    styleUrls: ['./item-template-editor.component.scss'],
    templateUrl: './item-template-editor.component.html'
})
export class ItemTemplateEditorComponent implements OnInit, OnChanges {
    @Input() itemTemplate: ItemTemplate;
    @Input() title: string|undefined;

    public modules: string[] = [];

    // Datas usefull for forms
    public skills: Skill[] = [];
    public skillsById: { [skillId: number]: Skill } = {};
    public sections: ItemSection[];
    public slots: ItemSlot[];
    public itemTypes: ItemType[];
    public originsName: { [originId: number]: string };
    public jobsName: { [jobId: number]: string };

    public selectedSection: ItemSection;
    public form: { levels: number[], protection: number[], damage: number[], dices: number[] };

    public autocompleteModuleCallback: Observable<AutocompleteValue[]> = this.updateAutocompleteModule.bind(this);

    @ViewChild('autocompleteModuleModalTextField')
    autocompleteModuleModalTextField: AutocompleteInputComponent;

    @ViewChild('addModuleDialog')
    public addModuleDialog: Portal<any>;
    public addModuleOverlayRef?: OverlayRef;

    private modulesDef: any[] = [
        {name: 'charge',       displayName: 'Charges/Utilisations'},
        {name: 'collect',      displayName: 'Possibilités de récolte'},
        {name: 'container',    displayName: 'Conteneur'},
        {name: 'currency',     displayName: 'Monnaie'},
        {name: 'damage',       displayName: 'Dégât'},
        {name: 'diceDrop',     displayName: 'Dé'},
        {name: 'enchantment',  displayName: 'Enchantement'},
        {name: 'gem',          displayName: 'Gemme'},
        {name: 'gun',          displayName: 'Arme à poudre'},
        {name: 'itemTypes',    displayName: 'Type d\'objet'},
        {name: 'level',        displayName: 'Niveau requis'},
        {name: 'lifetime',     displayName: 'Temps de conservation'},
        {name: 'modifiers',    displayName: 'Modificateurs'},
        {name: 'origin',       displayName: 'Origine'},
        {name: 'prereq',       displayName: 'Prérequis'},
        {name: 'protection',   displayName: 'Protection'},
        {name: 'quantifiable', displayName: 'Quantifiable'},
        {name: 'relic',        displayName: 'Relique'},
        {name: 'rarity',       displayName: 'Indice de rareté'},
        {name: 'rupture',      displayName: 'Rupture'},
        {name: 'sex',          displayName: 'Sexe'},
        {name: 'skill',        displayName: 'Compétences'},
        {name: 'skillBook',    displayName: 'Livre de compétences'},
        {name: 'slots',        displayName: 'Equipement'},
        {name: 'space',        displayName: 'Encombrement'},
        {name: 'throwable',    displayName: 'Prévue pour le jet'},
        {name: 'weight',       displayName: 'Poids'}
    ];

    constructor(private _itemTemplateservice: ItemTemplateService
        , private _originService: OriginService
        , private _nhbkDialogService: NhbkDialogService
        , private _jobService: JobService
        , public _loginService: LoginService
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
            this.addModuleOverlayRef = undefined;
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

    changeSource(source: 'official' | 'community' | 'private') {
        if (!this._loginService.currentLoggedUser) {
            throw new Error('changeSource: No logged user');
        }
        this.itemTemplate.source = source;
        if (source === 'official') {
            this.itemTemplate.sourceUser = undefined;
            this.itemTemplate.sourceUserId = undefined;
        } else {
            this.itemTemplate.sourceUser = this._loginService.currentLoggedUser.displayName;
            this.itemTemplate.sourceUserId = this._loginService.currentLoggedUser.id;
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
            case 'collect':
                this.itemTemplate.data.availableLocation = '';
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
            case 'enchantment':
                this.itemTemplate.data.enchantment = '';
                break;
            case 'gem':
                this.itemTemplate.data.useUG = true;
                break;
            case 'gun':
                this.itemTemplate.data.gun = new ItemTemplateGunData();
                break;
            case 'level':
                this.itemTemplate.data.requireLevel = 1;
                break;
            case 'lifetime':
                this.itemTemplate.data.lifetime = {durationType: 'time', timeDuration: 0};
                break;
            case 'modifiers':
                this.itemTemplate.modifiers = [];
                break;
            case 'itemTypes':
                this.itemTemplate.data.itemTypes = [];
                break;
            case 'origin':
                this.itemTemplate.data.origin = '';
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
            case 'rarity':
                this.itemTemplate.data.rarityIndicator = '';
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
            case 'space':
                this.itemTemplate.data.space = '';
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
                this.itemTemplate.data.charge = undefined;
                break;
            case 'collect':
                this.itemTemplate.data.availableLocation = undefined;
                break;
            case 'container':
                this.itemTemplate.data.container = undefined;
                break;
            case 'currency':
                this.itemTemplate.data.isCurrency = undefined;
                break;
            case 'damage':
                this.itemTemplate.data.damageDice = undefined;
                this.itemTemplate.data.bonusDamage = undefined;
                this.itemTemplate.data.damageType = undefined;
                break;
            case 'diceDrop':
                this.itemTemplate.data.diceDrop = undefined;
                break;
            case 'enchantment':
                this.itemTemplate.data.enchantment = undefined;
                break;
            case 'gem':
                this.itemTemplate.data.useUG = undefined;
                break;
            case 'gun':
                this.itemTemplate.data.gun = undefined;
                break;
            case 'level':
                this.itemTemplate.data.requireLevel = undefined;
                break;
            case 'lifetime':
                this.itemTemplate.data.lifetime = undefined;
                break;
            case 'modifiers':
                this.itemTemplate.modifiers = [];
                break;
            case 'itemTypes':
                this.itemTemplate.data.itemTypes = undefined;
                break;
            case 'origin':
                this.itemTemplate.data.origin = undefined;
                break;
            case 'prereq':
                this.itemTemplate.requirements = [];
                break;
            case 'protection':
                this.itemTemplate.data.protection = undefined;
                this.itemTemplate.data.protectionAgainstMagic = undefined;
                this.itemTemplate.data.magicProtection = undefined;
                this.itemTemplate.data.protectionAgainstType = undefined;
                break;
            case 'quantifiable':
                this.itemTemplate.data.quantifiable = undefined;
                break;
            case 'rarity':
                this.itemTemplate.data.rarityIndicator = undefined;
                break;
            case 'relic':
                this.itemTemplate.data.relic = undefined;
                break;
            case 'rupture':
                this.itemTemplate.data.rupture = undefined;
                break;
            case 'sex':
                this.itemTemplate.data.sex = undefined;
                break;
            case 'skill':
                this.itemTemplate.skills = [];
                this.itemTemplate.unskills = [];
                this.itemTemplate.skillModifiers = [];
                break;
            case 'skillBook':
                this.itemTemplate.data.skillBook = undefined;
                break;
            case 'slots':
                this.itemTemplate.slots = [];
                break;
            case 'space':
                this.itemTemplate.data.space = undefined;
                break;
            case 'throwable':
                this.itemTemplate.data.throwable = undefined;
                break;
            case 'weight':
                this.itemTemplate.data.weight = undefined;
                break;
        }

        this.modules.splice(moduleIndex, 1);
    }

    determineModulesFromItemTemplate(): void {
        let modules: string[] = [];

        if (this.itemTemplate.data.charge) {
            modules.push('charge');
        }
        if (this.itemTemplate.data.availableLocation) {
            modules.push('collect');
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
        if (!isNullOrUndefined(this.itemTemplate.data.enchantment)) {
            modules.push('enchantment');
        }
        if (this.itemTemplate.data.useUG) {
            modules.push('gem');
        }
        if (this.itemTemplate.data.gun) {
            modules.push('gun');
        }
        if (this.itemTemplate.data.requireLevel) {
            modules.push('level');
        }
        if (this.itemTemplate.data.lifetime) {
            modules.push('lifetime');
        }
        if (!isNullOrUndefined(this.itemTemplate.modifiers) && this.itemTemplate.modifiers.length) {
            modules.push('modifiers');
        }
        if (!isNullOrUndefined(this.itemTemplate.data.itemTypes) && this.itemTemplate.data.itemTypes.length) {
            modules.push('itemTypes');
        }
        if (!isNullOrUndefined(this.itemTemplate.data.origin)) {
            modules.push('origin');
        }
        if (!isNullOrUndefined(this.itemTemplate.requirements)
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
        if (!isNullOrUndefined(this.itemTemplate.data.rarityIndicator)) {
            modules.push('rarity');
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
        if ((!isNullOrUndefined(this.itemTemplate.skills) && this.itemTemplate.skills.length)
            || (!isNullOrUndefined(this.itemTemplate.unskills) && this.itemTemplate.unskills.length)
            || (!isNullOrUndefined(this.itemTemplate.skillModifiers) && this.itemTemplate.skillModifiers.length)) {
            modules.push('skill');
        }
        if (this.itemTemplate.data.skillBook) {
            modules.push('skillBook');
        }
        if (!isNullOrUndefined(this.itemTemplate.slots) && this.itemTemplate.slots.length) {
            modules.push('slots');
        }
        if (this.itemTemplate.data.space) {
            modules.push('space');
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
            this._itemTemplateservice.getItemTypes(),
        ).subscribe(([itemTypes]) => {
            Observable.forkJoin(
                this._itemTemplateservice.getSectionsList(),
                this._skillService.getSkills(),
                this._skillService.getSkillsById(),
                this._itemTemplateservice.getSlots(),
                this._jobService.getJobList(),
                this._originService.getOriginList()
            ).subscribe(([sections, skills, skillsById, slots, jobs, origins]) => {
                this.sections = sections;
                this.skills = skills;
                this.skillsById = skillsById;
                this.slots = slots;
                this.itemTypes = itemTypes;

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
        });
    }
}
