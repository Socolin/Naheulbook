export type ApplyEntropyEventRequest = {
    /** Id of the calamity (Mankdebol) or blessing (Débilibeuk) chosen by the GM */
    eventId: string;
    /** Character designated by the D6 roll, when the event targets a single character */
    characterId?: number;
}
