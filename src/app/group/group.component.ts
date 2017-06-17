import {
    Component, OnInit, OnDestroy, ViewChild, QueryList,
    ViewChildren
} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {NotificationsService} from '../notifications';

import {Group, CharacterInviteInfo, Fighter} from '.';
import {GroupService} from './group.service';
import {Character} from '../character';

import {LoginService} from '../user';
import {AutocompleteValue} from '../shared';
import {LocationService, Location} from '../location';
import {NhbkDateOffset} from '../date';
import {GroupActionService} from './group-action.service';
import {MdTabChangeEvent, OverlayRef, Portal, Overlay, OverlayState, TemplatePortalDirective} from '@angular/material';
import {User} from '../user/user.model';
import {NhbkDialogService} from '../shared/nhbk-dialog.service';
import {CharacterService} from '../character/character.service';
import {Subscription} from 'rxjs/Subscription';
import {WebSocketService} from '../websocket/websocket.service';
import {Effect} from '../effect/effect.model';
import {ActiveStatsModifier} from '../shared/stat-modifier.model';
import {AddEffectModalComponent} from '../effect/add-effect-modal.component';
import {MonsterService} from '../monster/monster.service';
import {FighterSelectorComponent} from './fighter-selector.component';

@Component({
    templateUrl: './group.component.html',
    styleUrls: ['./group.component.scss'],
    providers: [GroupActionService],
})
export class GroupComponent implements OnInit, OnDestroy {
    public group: Group;

    public currentTabIndex = 0;
    public currentTab = 'infos';
    public tabs: Array<{hash: string}> = [
        {hash: 'infos'},
        {hash: 'characters'},
        {hash: 'combat'},
        {hash: 'loot'},
        {hash: 'history'},
    ];

    public autocompleteLocationsCallback: Function;

    @ViewChildren('characters')
    public characterSheetsDialog: QueryList<TemplatePortalDirective>;
    private characterSheetOverlayRef: OverlayRef;

    @ViewChild('changeOwnershipDialog')
    public changeOwnershipDialog: Portal<any>;
    public changeOwnershipOverlayRef: OverlayRef;
    public selectedCharacter: Character;
    public changeOwnershipNewOwner: User;
    public filterSearchUser: string = null;
    public filteredUsers: Object[] = [];

    @ViewChild('inviteCharacterModal')
    public inviteCharacterModal: Portal<any>;
    public inviteCharacterOverlayRef: OverlayRef;
    public searchNameInvite: string;
    public filteredInvitePlayers: CharacterInviteInfo[] = [];
    public selectedInviteCharacter: Character;

    private charactersSubscriptions: {[characterId: number]: {notification: Subscription }} = {};
    private addedCharacterSub: Subscription;
    private removedCharacterSub: Subscription;
    private routeSub: Subscription;
    private routeFragmentSub: Subscription;
    private groupNotificationSub: Subscription;

    @ViewChild('addEffectModal')
    public addEffectModal: AddEffectModalComponent;

    @ViewChild('fighterSelector')
    public fighterSelector: FighterSelectorComponent;

    private currentSelectAction: string;
    private tmpModifier: ActiveStatsModifier;

    constructor(private _route: ActivatedRoute
        , private _router: Router
        , public _loginService: LoginService
        , private _groupService: GroupService
        , private _monsterService: MonsterService
        , private _locationService: LocationService
        , private _notification: NotificationsService
        , private _actionService: GroupActionService
        , private _websocketService: WebSocketService
        , private _overlay: Overlay
        , private _nhbkDialogService: NhbkDialogService
        , private _characterService: CharacterService) {
    }


    /* Tabs */

    getTabIndexFromHash(hash: string): number {
        return this.tabs.findIndex(t => t.hash === hash);
    }

    selectTab(tabChangeEvent: MdTabChangeEvent): boolean {
        if (tabChangeEvent.index < this.tabs.length) {
            this.currentTab = this.tabs[tabChangeEvent.index].hash;
            window.location.hash = this.currentTab;
        }
        return false;
    }

    /* Infos tab */

    changeGroupValue(key: string, value: any) {
        this._groupService.editGroupValue(this.group.id, key, value).subscribe(
            data => {
                this.group.data.changeValue(key, data[key]);
            }
        );
    }

    addTime(dateOffset: NhbkDateOffset) {
        this._groupService.addTime(this.group.id, dateOffset).subscribe(
            data => {
                this.group.data.changeValue('date', data.date);
            }
        );
    }

    changeGroupLocation(location: Location) {
        this._groupService.editGroupValue(this.group.id, 'location', location.id).subscribe(
            () => {
                this._notification.info('Lieu', this.group.location.name + ' -> ' + location.name);
                this.group.location = location;
            }
        );
    }

    updateLocationListAutocomplete(filter: string) {
        return this._locationService.searchLocations(filter).map(
            list => list.map(e => new AutocompleteValue(e, e.name))
        );
    }

    /* Characters tab */

    displayCharacterSheet(character: Character) {
        let characterSheetDialog = this.characterSheetsDialog.toArray();
        let index = 0;
        for (let i = 0; i < this.group.characters.length; i++) {
            if (this.group.characters[i] === character) {
                break;
            }
            if (this.group.characters[i].active) {
                index++;
            }
        }

        if (index < characterSheetDialog.length) {
            let config = new OverlayState();

            config.positionStrategy = this._overlay.position()
                .global()
                .top('5vh')
                .centerHorizontally();
            config.hasBackdrop = true;

            let overlayRef = this._overlay.create(config);
            overlayRef.attach(characterSheetDialog[index]);
            overlayRef.backdropClick().subscribe(() => overlayRef.detach());
            this.characterSheetOverlayRef = overlayRef;
        }
    }

    closeCharacterSheet() {
        this.characterSheetOverlayRef.detach();
    }

    createNpc() {
        this._router.navigate(['/gm/character/create'], {queryParams: {isNpc: true, groupId: this.group.id}});
        return false;
    }

    activeAllCharacter(active: boolean) {
        for (let i = 0; i < this.group.characters.length; i++) {
            let character = this.group.characters[i];
            this._characterService.changeGmData(character.id, 'active', active).subscribe(
                change => {
                    character.changeActive(change.value);
                }
            );
        }
    }

    toggleActiveCharacter(character) {
        this._characterService.changeGmData(character.id, 'active', !character.active).subscribe(
            change => {
                character.changeActive(change.value);
            }
        );
    }

    takeOwnership(character) {
        this._characterService.changeGmData(character.id, 'owner', 0).subscribe(
            change => {
                character.user = change.value;
                this._notification.success('Modification personnage', 'Ce personnage vous appartient a présent');
            }
        );
    }

    setOwnership(user) {
        let character = this.selectedCharacter;
        this._characterService.changeGmData(character.id, 'owner', user.id).subscribe(
            change => {
                character.user = change.value;
                this._notification.success('Modification personnage', 'Ce personnage a changé de propriétaire');
            }
        );
    }

    openChangeOwnershipDialog(character: Character) {
        this.selectedCharacter = character;

        this.changeOwnershipOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.changeOwnershipDialog);
    }

    closeChangeOwnershipDialog() {
        this.changeOwnershipOverlayRef.detach();
    }

    changeOwnershipConfirm() {
        this.setOwnership(this.changeOwnershipNewOwner);
        this.closeChangeOwnershipDialog();
    }

    openInviteCharacterModal() {
        this.inviteCharacterOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.inviteCharacterModal);
    }

    closeInviteCharacterModal() {
        this.inviteCharacterOverlayRef.detach();
    }

    updateFilteredPlayer() {
        if (!this.searchNameInvite) {
            this.filteredInvitePlayers = [];
            return;
        }
        this._characterService.searchPlayersForInvite(this.searchNameInvite, this.group.id).subscribe(
            res => {
                this.filteredInvitePlayers = res;
            }
        );
    }

    _removeFromInvited(character): void {
        let idx = this.group.invited.findIndex(char => char.id === character.id);
        if (idx !== -1) {
            this.group.invited.splice(idx, 1);
        }
    }

    _removeFromInvites(character): void {
        let idx = this.group.invites.findIndex(char => char.id === character.id);
        if (idx !== -1) {
            this.group.invites.splice(idx, 1);
        }
    }

    cancelInvite(character): boolean {
        this._characterService.cancelInvite(character.id, this.group.id).subscribe(
            res => {
                this._removeFromInvited(res.character);
                this._removeFromInvites(res.character);
            }
        );
        return false;
    }

    acceptInvite(character) {
        this._characterService.joinGroup(character.id, this.group.id).subscribe(
            res => {
                character.invites = [];
                character.group = res.group;
            }
        );
        return false;
    }

    inviteCharacter(character) {
        this._characterService.inviteCharacter(this.group.id, character.id).subscribe(
            res => {
                this.group.invited.push(res);
                for (let i = 0; i < this.filteredInvitePlayers.length; i++) {
                    let char = this.filteredInvitePlayers[i];
                    if (char.id === res.id) {
                        this.filteredInvitePlayers.splice(i, 1);
                        break;
                    }
                }
            }
        );
        return false;
    }

    updateSearchUser() {
        if (this.filterSearchUser) {
            this._loginService.searchUser(this.filterSearchUser).subscribe(
                res => {
                    this.filteredUsers = res;
                }
            );
        } else {
            this.filteredUsers = [];
        }
    }

    /* Combat tab */

    startCombat() {
        this.changeGroupValue('inCombat', true);
    }

    endCombat() {
        this.changeGroupValue('inCombat', false);
    }


    /* Misc */

    unregisterCharacterNotification(character: Character) {
        if (character.id in this.charactersSubscriptions) {
            this.charactersSubscriptions[character.id].notification.unsubscribe();
        }
    }

    registerCharacterNotification(character: Character) {
        let notifSub = character.onNotification.subscribe(notificationData => {
            this._notification.info('', character.name + ':' + notificationData.message);
        });

        this.charactersSubscriptions[character.id] = {notification: notifSub};
    }

    private clearGroupSub() {
        if (!this.group) {
            return;
        }

        for (let character of this.group.characters) {
            this.unregisterCharacterNotification(character);
        }

        this.groupNotificationSub.unsubscribe();
        this.group.dispose();
        this.addedCharacterSub.unsubscribe();
        this.removedCharacterSub.unsubscribe();
    }

    ngOnDestroy(): void {
        this.clearGroupSub();
        this.routeSub.unsubscribe();
        this.routeFragmentSub.unsubscribe();
    }

    ngOnInit() {
        this._actionService.registerAction('displayCharacterSheet').subscribe(
            data => {
                this.displayCharacterSheet(data.data);
            });

        this.routeSub = this._route.params.subscribe(
            params => {
                let id = +params['id'];
                this._groupService.getGroup(id).subscribe(
                    group => {
                        this.clearGroupSub();
                        this.group = group;
                        this.groupNotificationSub = group.onNotification.subscribe(notificationData => {
                            this._notification.info('', notificationData.message);
                        });
                        for (let character of this.group.characters) {
                            this.registerCharacterNotification(character);
                        }
                        this.addedCharacterSub = this.group.characterAdded.subscribe(character => {
                            this.registerCharacterNotification(character);
                        });
                        this.removedCharacterSub = this.group.characterRemoved.subscribe(character => {
                            this.unregisterCharacterNotification(character);
                        });
                        this._websocketService.registerElement(this.group);
                    }
                );
            }
        );

        this.routeFragmentSub = this._route.fragment.subscribe(value => {
            if (value) {
                this.currentTabIndex = this.getTabIndexFromHash(value);
                this.currentTab = value;
            }
        });
        this.autocompleteLocationsCallback = this.updateLocationListAutocomplete.bind(this);
    }

    onSelectFighters(fighters: Fighter[]) {
        if (this.currentSelectAction === 'applyModifier') {
            for (let fighter of fighters) {
                if (fighter.isMonster) {
                    this._monsterService.addModifier(fighter.id, this.tmpModifier).subscribe(
                        fighter.monster.onAddModifier.bind(fighter.monster)
                    );
                }
                else {
                    this._characterService.addModifier(fighter.id, this.tmpModifier).subscribe(
                        fighter.character.onAddModifier.bind(fighter.character)
                    );
                }
            }
            this.tmpModifier = null;
        }

        this.currentSelectAction = null;
    }

    selectCustomModifier(modifier: ActiveStatsModifier) {
        this.tmpModifier = modifier;
        this.currentSelectAction = 'applyModifier';
        this.fighterSelector.open('Choisir sur qui appliquer l\'effet ' + modifier.name);
    }

    usefullDataAction(event: {action: string, data: any}) {
        switch (event.action) {
            case 'applyEffect': {
                let effect: Effect = event.data;
                this.addEffectModal.openEffect(effect);
                break;
            }
        }
    }
}
