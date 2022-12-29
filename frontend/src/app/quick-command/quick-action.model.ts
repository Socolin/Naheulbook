export enum CommandSuggestionType {
    Page = 'Page',
    Personnage = 'Personnage',
    Info = 'Info',
    Action = 'Action',
}

export interface QuickAction {
    displayText: string;
    icon: string;
    iconFontSet?: string;
    iconColor?: string;
    action: () => void;
    priority: number;
    canBeUsedInRecent: boolean,
    type: CommandSuggestionType
}
