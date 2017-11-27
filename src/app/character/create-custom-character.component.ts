import {Component, OnChanges, OnInit, SimpleChanges} from '@angular/core';
import {Router} from '@angular/router';

import {Job, JobService} from '../job';
import {Origin, OriginService} from '../origin';
import {Skill, SkillService} from '../skill';
import {CharacterService} from './character.service';
import {Observable} from 'rxjs/Observable';
import {Speciality} from '../job/speciality.model';

@Component({
    templateUrl: './create-custom-character.component.html',
    styleUrls: ['./create-custom-character.component.scss'],
})
export class CreateCustomCharacterComponent implements OnInit {
    public cou: number;
    public int: number;
    public cha: number;
    public ad: number;
    public fo: number;

    public xp = 0;
    public level = 1;
    public fatePoint = 0;
    public sex = 'Homme';
    public name = '';

    public origins: Origin[] = [];
    public jobs: Job[] = [];
    public availableJobs: Job[] = [];
    public skills: Skill[] = [];
    public knownSkills: Skill[] = [];
    public availableSkills: Skill[] = [];
    public selectedOrigin?: Origin;
    public selectedJob?: Job;
    public selectedSkills: Skill[] = [];
    public specialities: Speciality[] = [];
    public ea = 0;
    public ev = 0;
    public at = 8;
    public prd = 10;

    constructor(private _router: Router
        , private _jobService: JobService
        , private _originService: OriginService
        , private _skillService: SkillService
        , private _characterService: CharacterService) {
    }

    updateBaseStats(): void {
        this.at = 8;
        this.prd = 10;
        if (this.selectedJob && this.selectedJob.baseAT) {
            this.at = this.selectedJob.baseAT;
        }
        if (this.selectedJob && this.selectedJob.basePRD) {
            this.prd = this.selectedJob.basePRD;
        }
        if (this.selectedOrigin) {
            this.at += this.selectedOrigin.bonusAT;
            this.prd += this.selectedOrigin.bonusPRD;
        }

        this.ev = this.selectedOrigin.baseEV;
        this.ea = 0;

        if (this.selectedJob) {
            if (this.selectedJob.baseEv) {
                this.ev = this.selectedJob.baseEv;
            }
            if (this.selectedJob.factorEv) {
                this.ev *= this.selectedJob.factorEv;
                this.ev = Math.round(this.ev);
            }
            if (this.selectedJob.bonusEv) {
                this.ev += this.selectedJob.bonusEv;
            }
            if (this.selectedJob.baseEa) {
                this.ea = this.selectedJob.baseEa;
            }
        }
    }

    updateAvailableSkills(): void {
        if (!this.selectedOrigin) {
            return;
        }
        let knownSkills: Skill[] = [];
        let availableSkills: Skill[] = [];
        for (let skill of this.skills) {
            if (this.selectedOrigin.skills.findIndex(s => s.id === skill.id) !== -1) {
                knownSkills.push(skill);
            }
            else if (this.selectedJob && this.selectedJob.skills.findIndex(s => s.id === skill.id) !== -1) {
                knownSkills.push(skill);
            }
            else {
                availableSkills.push(skill);
            }
        }
        this.knownSkills = knownSkills;
        this.availableSkills = availableSkills;
        this.updateBaseStats();
    }

    updateAvailableJobs(): void {
        let availableJobs: Job[] = [];
        for (let job of this.jobs) {
            if (job.originsWhitelist && job.originsWhitelist.length) {
                if (job.originsWhitelist.findIndex(w => w.id === this.selectedOrigin.id) !== -1) {
                    availableJobs.push(job);
                }
            }
            else if (job.originsBlacklist && job.originsBlacklist.length) {
                if (job.originsBlacklist.findIndex(w => w.id === this.selectedOrigin.id) === -1) {
                    availableJobs.push(job);
                }
            }
            else {
                availableJobs.push(job);
            }
        }
        this.availableJobs = availableJobs;
        if (this.selectedJob) {
            if (availableJobs.findIndex(j => j.id === this.selectedJob.id) === -1) {
                this.selectedJob = undefined;
            }
        }

        this.updateAvailableSkills();
    }

    ngOnInit(): void {
        Observable.forkJoin(
            this._jobService.getJobList(),
            this._originService.getOriginList(),
            this._skillService.getSkills(),
        ).subscribe(([jobs, origins, skills]) => {
            this.jobs = jobs;
            this.origins = origins;
            this.skills = skills;
        });
    }
}
