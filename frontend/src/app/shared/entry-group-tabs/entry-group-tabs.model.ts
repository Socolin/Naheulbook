/**
 * Row of a reference list (talent, mastery): name, conditions to take it and its effect
 */
export interface EntryView {
    id: string;
    name: string;
    /** Short text shown before the name (e.g. the D100 roll of a handicap) */
    badge?: string;
    restriction?: string;
    effectLines: string[];
    notes?: string;
    requiredLevel?: number;
    prerequisiteName?: string;
}

export interface EntryGroupView {
    id: string;
    name: string;
    description?: string;
    entries: EntryView[];
}

export interface EntrySource {
    id: string;
    name: string;
    data: {
        restriction?: string;
        effect: string;
        notes?: string;
        requiredLevel?: number;
    };
}

export function toEntryView(source: EntrySource, prerequisiteId: string | undefined, namesById: Map<string, string>): EntryView {
    const data = source.data;
    return {
        id: source.id,
        name: source.name,
        restriction: data.restriction,
        effectLines: data.effect.split('\n').map(l => l.trim()).filter(l => l),
        notes: data.notes,
        requiredLevel: data.requiredLevel,
        prerequisiteName: prerequisiteId ? namesById.get(prerequisiteId) : undefined,
    };
}

/**
 * Lower case, without accents, and with typographic apostrophes replaced so that "c'est" matches "C’est"
 */
export function normalizeEntrySearch(text: string): string {
    return text.normalize('NFD').replace(/[̀-ͯ]/g, '').replace(/[’‘]/g, '\'').toLowerCase();
}
