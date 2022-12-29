import {FormControl} from '@angular/forms';

export function requiredFileType(types: string[]) {
    return function (control: FormControl) {
        const file = control.value as File | undefined;
        if (file) {
            const extension = file.name.split('.')[1].toLowerCase();
            for (const type of types) {
                if (type.toLowerCase() === extension.toLowerCase()) {
                    return null;
                }
            }
            return {
                requiredFileType: true
            };
        }

        return null;
    };
}
