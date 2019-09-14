import {PanelNames} from '../useful-data.model';

export interface UsefulDataDialogResult {
    openPanel?: {
        panelName: PanelNames,
        arg?: any
    },
    action?: {
        action: string;
        data: any
    }
}
