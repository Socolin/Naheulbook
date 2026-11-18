import {ChangeDetectionStrategy, Component, Input} from '@angular/core';
import {MatIcon} from '@angular/material/icon';
import {SpecialAttackResponse} from '../api/responses';
import {SpecialAttackView, toSpecialAttackView} from './special-attack-view';

/**
 * Card displaying all the details of a special attack.
 * Content can be projected in the card with the `cardStatus` (after the header) and `cardActions` (at the bottom) attributes.
 */
@Component({
    selector: 'special-attack-card',
    imports: [MatIcon],
    templateUrl: './special-attack-card.component.html',
    styleUrl: './special-attack-card.component.scss',
    changeDetection: ChangeDetectionStrategy.Eager,
})
export class SpecialAttackCardComponent {
    @Input() compact = false;

    view?: SpecialAttackView;

    @Input({required: true})
    set specialAttack(specialAttack: SpecialAttackResponse) {
        this.view = toSpecialAttackView(specialAttack);
    }
}
