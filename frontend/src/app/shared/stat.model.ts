export interface Stat {
    name: string;
    description: string;
    displayName: string;
}

export function generateAllStatsPair(): string[][] {
    let inverses: string[][] = [];
    let statNames = ['cou', 'int', 'fo', 'ad', 'cha'];
    for (let i = 0; i < 5; i++) {
        for (let j = 0; j < i; j++) {
            if (i === j) {
                continue;
            }
            let first = statNames[i];
            let second = statNames[j];
            inverses.push([first, second]);
        }
    }
    return inverses;
}
