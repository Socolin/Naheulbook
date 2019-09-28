import {forkJoin, Subscription} from 'rxjs';

import {map} from 'rxjs/operators';
import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {MatDialog, MatTabChangeEvent} from '@angular/material';
import {Overlay, OverlayRef} from '@angular/cdk/overlay';
import {Portal} from '@angular/cdk/portal';

import {AutocompleteValue, LapCountDecrement, NhbkDialogService, PromptDialogComponent} from '../shared';
import {NotificationsService} from '../notifications';
import {dateOffset2TimeDuration} from '../date/util';
import {NhbkDateOffset} from '../date';

import {GroupService} from './group.service';
import {WebSocketService} from '../websocket';
import {GroupActionService} from './group-action.service';

import {Character, CharacterService} from '../character';
import {Effect} from '../effect';

import {LoginService, User} from '../user';
import {MonsterService} from '../monster';
import {Location, LocationService} from '../location';
import {ItemService} from '../item';
import {ItemTemplate} from '../item-template';

import {FighterSelectorComponent, FighterSelectorDialogData} from './fighter-selector.component';
import {Fighter, Group, GroupInvite} from './group.model';
import {CharacterSheetDialogComponent} from './character-sheet-dialog.component';
import {openCreateItemDialog} from './create-item-dialog.component';
import {
    GroupAddEffectDialogComponent,
    GroupAddEffectDialogData,
    GroupAddEffectDialogResult
} from './group-add-effect-dialog.component';
import {CharacterSearchResponse} from '../api/responses';

@Component({
    templateUrl: './group.component.html',
    styleUrls: ['./group.component.scss'],
    providers: [GroupActionService],
})
export class GroupComponent implements OnInit, OnDestroy {
    public group: Group;

    public currentTabIndex = 0;
    public currentTab = 'infos';
    public tabs: Array<{ hash: string }> = [
        {hash: 'infos'},
        {hash: 'characters'},
        {hash: 'combat'},
        {hash: 'loot'},
        {hash: 'history'},
    ];

    public autocompleteLocationsCallback: Function;

    public loadingGroup = false;

    @ViewChild('changeOwnershipDialog', {static: true})
    public changeOwnershipDialog: Portal<any>;
    public changeOwnershipOverlayRef: OverlayRef;
    public selectedCharacter: Character;
    public changeOwnershipNewOwner: User;
    public filterSearchUser: string | null = null;
    public filteredUsers: Object[] = [];

    @ViewChild('inviteCharacterModal', {static: true})
    public inviteCharacterModal: Portal<any>;
    public inviteCharacterOverlayRef: OverlayRef;
    public searchNameInvite: string;
    public filteredInvitePlayers: CharacterSearchResponse[] = [];
    public selectedInviteCharacter: Character;

    private charactersSubscriptions: { [characterId: number]: { notification: Subscription } } = {};
    private addedCharacterSub: Subscription;
    private removedCharacterSub: Subscription;
    private routeSub: Subscription;
    private routeFragmentSub: Subscription;
    private groupNotificationSub: Subscription;

    constructor(private _route: ActivatedRoute
        , private _router: Router
        , public dialog: MatDialog
        , public _loginService: LoginService
        , private _groupService: GroupService
        , private _monsterService: MonsterService
        , private _locationService: LocationService
        , private _notification: NotificationsService
        , private _actionService: GroupActionService
        , private _websocketService: WebSocketService
        , private _overlay: Overlay
        , private _nhbkDialogService: NhbkDialogService
        , private _itemService: ItemService
        , private _characterService: CharacterService) {
    }


    /* Tabs */

    getTabIndexFromHash(hash: string): number {
        return this.tabs.findIndex(t => t.hash === hash);
    }

    selectTab(tabChangeEvent: MatTabChangeEvent): boolean {
        if (tabChangeEvent.index < this.tabs.length) {
            this.currentTab = this.tabs[tabChangeEvent.index].hash;
            window.location.hash = this.currentTab;
        }
        return false;
    }

    /* Infos tab */

    changeGroupValue(key: string, value: any) {
        this._groupService.editGroupValue(this.group.id, key, value).subscribe(
            () => {
                this.group.data.changeValue(key, value);
            }
        );
    }

    addTime(dateOffset: NhbkDateOffset) {
        let time = dateOffset2TimeDuration(dateOffset);
        let changes = this.group.updateTime('time', time);
        forkJoin([
            this._groupService.addTime(this.group.id, dateOffset),
            this._groupService.saveChangedTime(this.group.id, changes)
        ]).subscribe(([newDate]) => {
            this.group.data.changeValue('date', newDate);
        });
    }

    changeGroupLocation(location: Location) {
        this._groupService.editGroupLocation(this.group.id, location.id).subscribe(
            () => {
                this._notification.info('Lieu', this.group.location.name + ' -> ' + location.name);
                this.group.location = location;
            }
        );
    }

    updateLocationListAutocomplete(filter: string) {
        return this._locationService.searchLocations(filter).pipe(map(
            list => list.map(e => new AutocompleteValue(e, e.name))
        ));
    }

    /* Characters tab */

    displayCharacterSheet(character: Character) {
        this.dialog.open(CharacterSheetDialogComponent, {
            data: {character},
            minWidth: '100vw',
            height: '100vh',
            autoFocus: false
        });
    }

    createNpc() {
        this._router.navigate(['/gm/character/create'], {
            queryParams: {
                isNpc: true,
                groupId: this.group.id
            }
        });
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
                if (!change.value) {
                    if (this.group.pendingModifierChanges) {
                        this._groupService.saveChangedTime(this.group.id, this.group.pendingModifierChanges);
                        this.group.pendingModifierChanges = null;
                    }
                }
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

    kickCharacter(character: Character) {
        this._groupService.kickCharacter(this.group.id, character.id).subscribe((characterId) => {
            this.group.removeCharacter(characterId);
        });
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
        this._groupService.searchPlayersForInvite(this.searchNameInvite).subscribe(
            res => {
                this.filteredInvitePlayers = res;
            }
        );
    }

    _removeFromInvited(characterId: number): void {
        let idx = this.group.invited.findIndex(char => char.id === characterId);
        if (idx !== -1) {
            this.group.invited.splice(idx, 1);
        }
    }

    _removeFromInvites(characterId: number): void {
        let idx = this.group.invites.findIndex(char => char.id === characterId);
        if (idx !== -1) {
            this.group.invites.splice(idx, 1);
        }
    }

    cancelInvite(character): boolean {
        this._characterService.cancelInvite(character.id, this.group.id).subscribe(
            res => {
                this._removeFromInvited(res.characterId);
                this._removeFromInvites(res.characterId);
            }
        );
        return false;
    }

    acceptInvite(invite: GroupInvite) {
        this._characterService.joinGroup(invite.id, this.group.id).subscribe(
            () => {
                this._removeFromInvited(invite.id);
                this._removeFromInvites(invite.id);
            }
        );
        return false;
    }

    inviteCharacter(character) {
        this.closeInviteCharacterModal();
        this._groupService.inviteCharacter(this.group.id, character.id).subscribe(
            res => {
                this.group.onAddInvite(res);
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

        this._websocketService.unregisterElement(this.group);
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
                this.loadingGroup = true;
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
                        this.loadingGroup = false;
                        this.group.characterJoining.subscribe((characterId) => {
                            this._characterService.getCharacter(characterId).subscribe((character) => {
                                this.group.addCharacter(character);
                            });
                        })
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

    openAddEffectDialog(effect?: Effect) {
        const dialogRef = this.dialog.open<GroupAddEffectDialogComponent, GroupAddEffectDialogData, GroupAddEffectDialogResult>(
            GroupAddEffectDialogComponent, {
                minWidth: '100vw',
                height: '100vh',
                data: {effect, fighters: this.group.fighters}
            });
        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }

            const {modifier, fighters} = result;
            for (let fighter of fighters) {
                if (modifier.durationType === 'lap') {
                    modifier.lapCountDecrement = new LapCountDecrement();
                    let currentFighter = this.group.currentFighter;
                    if (!currentFighter) {
                        currentFighter = fighter;
                    }
                    modifier.lapCountDecrement.fighterId = currentFighter.id;
                    modifier.lapCountDecrement.fighterIsMonster = currentFighter.isMonster;
                    modifier.lapCountDecrement.when = 'BEFORE';
                }
                if (fighter.isMonster) {
                    this._monsterService.addModifier(fighter.id, modifier).subscribe(
                        fighter.monster.onAddModifier.bind(fighter.monster)
                    );
                } else {
                    this._characterService.addModifier(fighter.id, modifier).subscribe(
                        fighter.character.onAddModifier.bind(fighter.character)
                    );
                }
            }
        });
    }

    usefulDataAction(event: { action: string, data: any }) {
        switch (event.action) {
            case 'applyEffect': {
                let effect: Effect = event.data;
                this.openAddEffectDialog(effect);
                break;
            }
            case 'addItem': {
                let itemTemplate: ItemTemplate = event.data;
                openCreateItemDialog(this.dialog, (item) => {
                    const dialogRef = this.dialog.open<FighterSelectorComponent, FighterSelectorDialogData, Fighter[]>(
                        FighterSelectorComponent, {
                            data: {
                                group: this.group,
                                title: 'Ajout de l\'objet',
                                subtitle: item.data.name || itemTemplate.name
                            },
                            autoFocus: false
                        });

                    dialogRef.afterClosed().subscribe(fighters => {
                        if (!fighters) {
                            return;
                        }
                        for (let fighter of fighters) {
                            if (fighter.isMonster) {
                                this._itemService.addItemTo('monster', fighter.id, item.template.id, item.data).subscribe();
                            } else {
                                this._itemService.addItemTo('character', fighter.id, item.template.id, item.data).subscribe();
                            }
                        }
                    });
                }, false, itemTemplate);
            }
        }
    }

    openEditGroupNameDialog() {
        const dialogRef = this.dialog.open(PromptDialogComponent, {
            data: {
                confirmText: 'CHANGER',
                cancelText: 'ANNULER',
                placeholder: 'Nom',
                title: 'Renommer le groupe',
                initialValue: this.group.name
            }
        });

        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            this._groupService.editGroupValue(this.group.id, 'name', result.text)
                .subscribe(() => {
                    this.group.name = result.text;
                });
        });
    }
}
