import {Component, EventEmitter, Input, Output} from '@angular/core';

import {Speciality} from '../job';

@Component({
    selector: 'speciality-selector',
    templateUrl: './speciality-selector.component.html'
})
export class SpecialitySelectorComponent {
    @Input() specialities: Speciality[];
    @Input() knownSpecialities: Speciality[];
    @Output() specialityChange: EventEmitter<Speciality> = new EventEmitter<Speciality>();

    public selectedSpeciality: Speciality;

    selectSpeciality(speciality: Speciality) {
        this.selectedSpeciality = speciality;
        this.specialityChange.emit(speciality);
    }

    isAvailable(speciality: Speciality) {
        if (this.knownSpecialities && this.knownSpecialities.length > 0) {
            if (speciality.hasFlag('ONE_SPECIALITY')) {
                return false;
            }
            for (let knownSpeciality of this.knownSpecialities) {
                if (knownSpeciality.id === speciality.id) {
                    return false;
                }
            }
        }
        return true;
    }
}
