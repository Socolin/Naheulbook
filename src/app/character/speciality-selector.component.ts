import {Component, EventEmitter, Input, Output} from '@angular/core';

import {Speciality} from './speciality.model';

@Component({
    selector: 'speciality-selector',
    templateUrl: 'app/character/speciality-selector.component.html'
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
            if (speciality.specials) {
                for (let i = 0; i < speciality.specials.length; i++) {
                    let special = speciality.specials[i];
                    if (special.token === 'ONE_SPECIALITY') {
                        return false;
                    }
                }
            }
            for (let i = 0; i < this.knownSpecialities.length; i++) {
                let spe = this.knownSpecialities[i];
                if (spe.id === speciality.id) {
                    return false;
                }
            }
        }
        return true;
    }
}
