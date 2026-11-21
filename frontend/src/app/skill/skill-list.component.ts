import {Component, OnInit, ChangeDetectionStrategy} from '@angular/core';
import {forkJoin} from 'rxjs';

import {Skill} from './skill.model';
import {SkillService} from './skill.service';
import {SkillComponent, SkillSources} from './skill.component';

import {removeDiacritics} from '../shared';
import {Origin} from '../origin/origin.model';
import {OriginService} from '../origin/origin.service';
import {Job} from '../job/job.model';
import {JobService} from '../job/job.service';
import {MatFormField, MatLabel, MatPrefix, MatSuffix} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {MatSelect} from '@angular/material/select';
import {MatOption} from '@angular/material/core';
import {MatIcon} from '@angular/material/icon';
import {MatIconButton} from '@angular/material/button';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {FormsModule} from '@angular/forms';

interface SkillView {
    skill: Skill;
    searchText: string;
    sources: SkillSources;
}

@Component({
    selector: 'skill-list',
    templateUrl: './skill-list.component.html',
    styleUrls: ['./skill-list.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [
        MatFormField, MatLabel, MatPrefix, MatSuffix, MatInput, MatSelect, MatOption, MatIcon, MatIconButton,
        MatProgressSpinner, FormsModule, SkillComponent,
    ]
})
export class SkillListComponent implements OnInit {
    public views?: SkillView[];
    public visibleViews: SkillView[] = [];
    public origins: Origin[] = [];
    public jobs: Job[] = [];

    public filter = '';
    public selectedOrigin?: Origin;
    public selectedJob?: Job;

    constructor(
        private readonly skillService: SkillService,
        private readonly originService: OriginService,
        private readonly jobService: JobService,
    ) {
    }

    ngOnInit() {
        forkJoin({
            skills: this.skillService.getSkills(),
            origins: this.originService.getOriginList(),
            jobs: this.jobService.getJobList(),
        }).subscribe(({skills, origins, jobs}) => {
            this.origins = [...origins].sort(byName);
            this.jobs = [...jobs].sort(byName);
            this.views = [...skills]
                .sort(byName)
                .map(skill => ({
                    skill,
                    searchText: normalize([skill.name, skill.description, skill.using, skill.stat?.join(' ')].join(' ')),
                    sources: this.computeSources(skill),
                }));
            this.updateFilter();
        });
    }

    updateFilter() {
        if (!this.views) {
            return;
        }
        const words = normalize(this.filter).split(/\s+/).filter(w => w);
        this.visibleViews = this.views.filter(view =>
            words.every(w => view.searchText.includes(w))
            && (!this.selectedOrigin || hasSkill(this.selectedOrigin, view.skill))
            && (!this.selectedJob || hasSkill(this.selectedJob, view.skill))
        );
    }

    clearFilter() {
        this.filter = '';
        this.updateFilter();
    }

    get hasActiveFilter(): boolean {
        return !!this.filter || !!this.selectedOrigin || !!this.selectedJob;
    }

    private computeSources(skill: Skill): SkillSources {
        return {
            origins: this.origins.filter(o => hasSkill(o, skill)).map(o => ({name: o.name, base: includesSkill(o.skills, skill)})),
            jobs: this.jobs.filter(j => hasSkill(j, skill)).map(j => ({name: j.name, base: includesSkill(j.skills, skill)})),
        };
    }
}

function normalize(text: string): string {
    return removeDiacritics(text).toLowerCase();
}

function byName(a: { name: string }, b: { name: string }): number {
    return a.name.localeCompare(b.name, 'fr', {sensitivity: 'base'});
}

function includesSkill(skills: Skill[] | undefined, skill: Skill): boolean {
    return !!skills?.some(s => s?.id === skill.id);
}

function hasSkill(source: { skills: Skill[], availableSkills: Skill[] }, skill: Skill): boolean {
    return includesSkill(source.skills, skill) || includesSkill(source.availableSkills, skill);
}
