-- Adminer 4.5.0 MySQL dump

SET NAMES utf8;
SET time_zone = '+00:00';
SET foreign_key_checks = 0;
SET sql_mode = 'NO_AUTO_VALUE_ON_ZERO';

DROP TABLE IF EXISTS `calendar`;
CREATE TABLE `calendar` (
                          `id` bigint(20) NOT NULL AUTO_INCREMENT,
                          `endday` int(11) NOT NULL,
                          `name` varchar(255) NOT NULL,
                          `note` longtext,
                          `startday` int(11) NOT NULL,
                          PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `character`;
CREATE TABLE `character` (
                           `id` bigint(20) NOT NULL AUTO_INCREMENT,
                           `active` bit(1) NOT NULL DEFAULT b'1',
                           `ad` smallint(6) NOT NULL,
                           `cha` smallint(6) NOT NULL,
                           `color` varchar(255) NOT NULL DEFAULT '22DD22',
                           `cou` smallint(6) NOT NULL,
                           `ea` smallint(6) DEFAULT NULL,
                           `ev` smallint(6) DEFAULT NULL,
                           `experience` bigint(20) NOT NULL,
                           `fatepoint` smallint(6) NOT NULL DEFAULT '0',
                           `fo` smallint(6) NOT NULL,
                           `gmdata` json DEFAULT NULL,
                           `group` bigint(20) DEFAULT NULL,
                           `int` smallint(6) NOT NULL,
                           `isnpc` bit(1) NOT NULL DEFAULT b'0',
                           `level` smallint(6) NOT NULL,
                           `name` varchar(255) NOT NULL,
                           `origin` bigint(20) NOT NULL,
                           `sexe` varchar(255) NOT NULL,
                           `statbonusad` varchar(255) DEFAULT NULL,
                           `targetcharacterid` bigint(20) DEFAULT NULL,
                           `targetmonsterid` bigint(20) DEFAULT NULL,
                           `user` bigint(20) DEFAULT NULL,
                           PRIMARY KEY (`id`),
                           KEY `IX_character_group` (`group`),
                           KEY `IX_character_origin` (`origin`),
                           KEY `IX_character_targetcharacterid` (`targetcharacterid`),
                           KEY `IX_character_targetmonsterid` (`targetmonsterid`),
                           KEY `IX_character_user` (`user`),
                           CONSTRAINT `FK_character_character_targetcharacterid` FOREIGN KEY (`targetcharacterid`) REFERENCES `character` (`id`) ON DELETE SET NULL,
                           CONSTRAINT `FK_character_group_group` FOREIGN KEY (`group`) REFERENCES `group` (`id`) ON DELETE SET NULL,
                           CONSTRAINT `FK_character_monster_targetmonsterid` FOREIGN KEY (`targetmonsterid`) REFERENCES `monster` (`id`) ON DELETE SET NULL,
                           CONSTRAINT `FK_character_origin_origin` FOREIGN KEY (`origin`) REFERENCES `origin` (`id`) ON DELETE NO ACTION,
                           CONSTRAINT `FK_character_user_user` FOREIGN KEY (`user`) REFERENCES `user` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `character_history`;
CREATE TABLE `character_history` (
                                   `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                   `action` varchar(255) NOT NULL,
                                   `character` bigint(20) NOT NULL,
                                   `data` json DEFAULT NULL,
                                   `date` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
                                   `effect` bigint(20) DEFAULT NULL,
                                   `gm` bit(1) NOT NULL DEFAULT b'0',
                                   `info` longtext,
                                   `item` bigint(20) DEFAULT NULL,
                                   `modifier` bigint(20) DEFAULT NULL,
                                   `user` bigint(20) DEFAULT NULL,
                                   PRIMARY KEY (`id`),
                                   KEY `IX_character_history_character` (`character`),
                                   KEY `IX_character_history_effect` (`effect`),
                                   KEY `IX_character_history_item` (`item`),
                                   KEY `IX_character_history_modifier` (`modifier`),
                                   KEY `IX_character_history_user` (`user`),
                                   CONSTRAINT `FK_character_history_character_character` FOREIGN KEY (`character`) REFERENCES `character` (`id`) ON DELETE CASCADE,
                                   CONSTRAINT `FK_character_history_character_modifier_modifier` FOREIGN KEY (`modifier`) REFERENCES `character_modifier` (`id`),
                                   CONSTRAINT `FK_character_history_effect_effect` FOREIGN KEY (`effect`) REFERENCES `effect` (`id`) ON DELETE NO ACTION,
                                   CONSTRAINT `FK_character_history_item_item` FOREIGN KEY (`item`) REFERENCES `item` (`id`) ON DELETE SET NULL,
                                   CONSTRAINT `FK_character_history_user_user` FOREIGN KEY (`user`) REFERENCES `user` (`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `character_job`;
CREATE TABLE `character_job` (
                               `character` bigint(20) DEFAULT NULL,
                               `job` bigint(20) DEFAULT NULL,
                               `order` int(11) DEFAULT NULL,
                               UNIQUE KEY `character_job_character_job_uindex` (`character`,`job`),
                               KEY `character_job_job_id_fk` (`job`),
                               CONSTRAINT `character_job_character_id_fk` FOREIGN KEY (`character`) REFERENCES `character` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
                               CONSTRAINT `character_job_job_id_fk` FOREIGN KEY (`job`) REFERENCES `job` (`id`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `character_modifier`;
CREATE TABLE `character_modifier` (
                                    `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                    `active` bit(1) NOT NULL DEFAULT b'1',
                                    `character` bigint(20) DEFAULT NULL,
                                    `combatcount` int(11) DEFAULT NULL,
                                    `currentcombatcount` int(11) DEFAULT NULL,
                                    `currenttimeduration` int(11) DEFAULT NULL,
                                    `duration` longtext,
                                    `name` varchar(255) NOT NULL,
                                    `permanent` bit(1) NOT NULL,
                                    `reusable` bit(1) NOT NULL DEFAULT b'0',
                                    `timeduration` int(11) DEFAULT NULL,
                                    `lapcount` int(11) DEFAULT NULL,
                                    `currentlapcount` int(11) DEFAULT NULL,
                                    `durationtype` varchar(255) DEFAULT NULL,
                                    `type` text,
                                    `description` text,
                                    `lapCountDecrement` json DEFAULT NULL,
                                    PRIMARY KEY (`id`),
                                    KEY `IX_character_modifier_character` (`character`),
                                    CONSTRAINT `FK_character_modifier_character_character` FOREIGN KEY (`character`) REFERENCES `character` (`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `character_modifier_value`;
CREATE TABLE `character_modifier_value` (
                                          `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                          `charactermodifier` bigint(20) NOT NULL,
                                          `stat` varchar(255) NOT NULL,
                                          `type` varchar(255) NOT NULL DEFAULT 'ADD',
                                          `value` smallint(6) NOT NULL,
                                          PRIMARY KEY (`id`),
                                          KEY `IX_character_modifier_value_charactermodifier` (`charactermodifier`),
                                          KEY `IX_character_modifier_value_stat` (`stat`),
                                          CONSTRAINT `FK_character_modifier_value_character_modifier_charactermodifier` FOREIGN KEY (`charactermodifier`) REFERENCES `character_modifier` (`id`) ON DELETE CASCADE,
                                          CONSTRAINT `FK_character_modifier_value_stat_stat` FOREIGN KEY (`stat`) REFERENCES `stat` (`name`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `character_skills`;
CREATE TABLE `character_skills` (
                                  `character` bigint(20) NOT NULL,
                                  `skill` bigint(20) NOT NULL,
                                  PRIMARY KEY (`character`,`skill`),
                                  KEY `IX_character_skills_character` (`character`),
                                  KEY `IX_character_skills_skill` (`skill`),
                                  CONSTRAINT `FK_character_skills_character_character` FOREIGN KEY (`character`) REFERENCES `character` (`id`) ON DELETE CASCADE,
                                  CONSTRAINT `FK_character_skills_skill_skill` FOREIGN KEY (`skill`) REFERENCES `skill` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `character_speciality`;
CREATE TABLE `character_speciality` (
                                      `character` bigint(20) NOT NULL,
                                      `speciality` bigint(20) NOT NULL,
                                      PRIMARY KEY (`character`,`speciality`),
                                      KEY `IX_character_speciality_speciality` (`speciality`),
                                      CONSTRAINT `FK_character_speciality_character_character` FOREIGN KEY (`character`) REFERENCES `character` (`id`) ON DELETE CASCADE,
                                      CONSTRAINT `FK_character_speciality_speciality_speciality` FOREIGN KEY (`speciality`) REFERENCES `speciality` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `effect`;
CREATE TABLE `effect` (
                        `id` bigint(20) NOT NULL AUTO_INCREMENT,
                        `category` bigint(20) NOT NULL,
                        `combatcount` int(11) DEFAULT NULL,
                        `description` longtext,
                        `dice` smallint(6) DEFAULT NULL,
                        `duration` longtext,
                        `name` varchar(255) NOT NULL,
                        `timeduration` int(11) DEFAULT NULL,
                        `lapcount` int(11) DEFAULT NULL,
                        `durationtype` varchar(255) DEFAULT NULL,
                        PRIMARY KEY (`id`),
                        KEY `IX_effect_category` (`category`),
                        CONSTRAINT `FK_effect_effect_category_category` FOREIGN KEY (`category`) REFERENCES `effect_category` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `effect_category`;
CREATE TABLE `effect_category` (
                                 `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                 `dicecount` smallint(6) NOT NULL,
                                 `dicesize` smallint(6) NOT NULL,
                                 `name` varchar(255) NOT NULL,
                                 `note` longtext,
                                 `typeid` bigint(20) DEFAULT NULL,
                                 PRIMARY KEY (`id`),
                                 KEY `effect_category_effect_type_id_fk` (`typeid`),
                                 CONSTRAINT `effect_category_effect_type_id_fk` FOREIGN KEY (`typeid`) REFERENCES `effect_type` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `effect_modifier`;
CREATE TABLE `effect_modifier` (
                                 `effect` bigint(20) NOT NULL,
                                 `stat` varchar(255) NOT NULL,
                                 `type` varchar(255) NOT NULL DEFAULT 'ADD',
                                 `value` smallint(6) NOT NULL,
                                 PRIMARY KEY (`effect`,`stat`),
                                 KEY `IX_effect_modifier_stat` (`stat`),
                                 CONSTRAINT `FK_effect_modifier_effect_effect` FOREIGN KEY (`effect`) REFERENCES `effect` (`id`) ON DELETE CASCADE,
                                 CONSTRAINT `FK_effect_modifier_stat_stat` FOREIGN KEY (`stat`) REFERENCES `stat` (`name`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `effect_type`;
CREATE TABLE `effect_type` (
                             `id` bigint(20) NOT NULL,
                             `name` varchar(255) DEFAULT NULL,
                             PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `error_report`;
CREATE TABLE `error_report` (
                              `id` bigint(20) NOT NULL AUTO_INCREMENT,
                              `data` json NOT NULL,
                              `date` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
                              `useragent` longtext,
                              PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `event`;
CREATE TABLE `event` (
                       `id` int(11) NOT NULL AUTO_INCREMENT,
                       `groupId` bigint(20) DEFAULT NULL,
                       `name` varchar(255) NOT NULL,
                       `description` text,
                       `timestamp` bigint(20) NOT NULL,
                       PRIMARY KEY (`id`),
                       KEY `event_group_id_fk` (`groupId`),
                       CONSTRAINT `event_group_id_fk` FOREIGN KEY (`groupId`) REFERENCES `group` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `god`;
CREATE TABLE `god` (
                     `id` int(11) NOT NULL AUTO_INCREMENT,
                     `displayname` varchar(255) NOT NULL,
                     `description` text,
                     `techname` varchar(255) DEFAULT NULL,
                     PRIMARY KEY (`id`),
                     UNIQUE KEY `god_displayname_uindex` (`displayname`),
                     UNIQUE KEY `god_techname_uindex` (`techname`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `group`;
CREATE TABLE `group` (
                       `id` bigint(20) NOT NULL AUTO_INCREMENT,
                       `combatlootid` bigint(20) DEFAULT NULL,
                       `data` json DEFAULT NULL,
                       `location` bigint(20) NOT NULL DEFAULT '1',
                       `master` bigint(20) DEFAULT NULL,
                       `name` varchar(255) NOT NULL,
                       PRIMARY KEY (`id`),
                       UNIQUE KEY `IX_group_combatlootid` (`combatlootid`),
                       KEY `IX_group_location` (`location`),
                       KEY `IX_group_master` (`master`),
                       CONSTRAINT `FK_group_location_location` FOREIGN KEY (`location`) REFERENCES `location` (`id`) ON DELETE CASCADE,
                       CONSTRAINT `FK_group_loot_combatlootid` FOREIGN KEY (`combatlootid`) REFERENCES `loot` (`id`) ON DELETE SET NULL,
                       CONSTRAINT `FK_group_user_master` FOREIGN KEY (`master`) REFERENCES `user` (`id`) ON DELETE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `group_history`;
CREATE TABLE `group_history` (
                               `id` bigint(20) NOT NULL AUTO_INCREMENT,
                               `action` varchar(255) NOT NULL,
                               `data` json DEFAULT NULL,
                               `date` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
                               `gm` bit(1) NOT NULL,
                               `group` bigint(20) NOT NULL,
                               `info` longtext,
                               `user` bigint(20) DEFAULT NULL,
                               PRIMARY KEY (`id`),
                               KEY `IX_group_history_group` (`group`),
                               KEY `IX_group_history_user` (`user`),
                               CONSTRAINT `FK_group_history_group_group` FOREIGN KEY (`group`) REFERENCES `group` (`id`) ON DELETE CASCADE,
                               CONSTRAINT `FK_group_history_user_user` FOREIGN KEY (`user`) REFERENCES `user` (`id`) ON DELETE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `group_invitations`;
CREATE TABLE `group_invitations` (
                                   `group` bigint(20) NOT NULL,
                                   `character` bigint(20) NOT NULL,
                                   `fromgroup` bit(1) NOT NULL,
                                   PRIMARY KEY (`group`,`character`),
                                   KEY `IX_group_invitations_character` (`character`),
                                   KEY `IX_group_invitations_group` (`group`),
                                   CONSTRAINT `FK_group_invitations_character_character` FOREIGN KEY (`character`) REFERENCES `character` (`id`) ON DELETE CASCADE,
                                   CONSTRAINT `FK_group_invitations_group_group` FOREIGN KEY (`group`) REFERENCES `group` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `icon`;
CREATE TABLE `icon` (
                      `name` varchar(255) NOT NULL,
                      PRIMARY KEY (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `item`;
CREATE TABLE `item` (
                      `id` bigint(20) NOT NULL AUTO_INCREMENT,
                      `characterid` bigint(20) DEFAULT NULL,
                      `container` bigint(20) DEFAULT NULL,
                      `data` json DEFAULT NULL,
                      `itemtemplateid` bigint(20) NOT NULL,
                      `lootid` bigint(20) DEFAULT NULL,
                      `modifiers` json DEFAULT NULL,
                      `monsterid` bigint(20) DEFAULT NULL,
                      `lifetimetype` varchar(30) GENERATED ALWAYS AS (json_unquote(json_extract(`data`,'$.lifetime.durationType'))) VIRTUAL,
                      PRIMARY KEY (`id`),
                      KEY `IX_item_characterid` (`characterid`),
                      KEY `IX_item_container` (`container`),
                      KEY `IX_item_itemtemplateid` (`itemtemplateid`),
                      KEY `IX_item_lootid` (`lootid`),
                      KEY `IX_item_monsterid` (`monsterid`),
                      CONSTRAINT `FK_item_character_characterid` FOREIGN KEY (`characterid`) REFERENCES `character` (`id`) ON DELETE CASCADE,
                      CONSTRAINT `FK_item_item_template_itemtemplateid` FOREIGN KEY (`itemtemplateid`) REFERENCES `item_template` (`id`) ON DELETE NO ACTION,
                      CONSTRAINT `FK_item_loot_lootid` FOREIGN KEY (`lootid`) REFERENCES `loot` (`id`) ON DELETE CASCADE,
                      CONSTRAINT `FK_item_monster_monsterid` FOREIGN KEY (`monsterid`) REFERENCES `monster` (`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `item_slot`;
CREATE TABLE `item_slot` (
                           `id` bigint(20) NOT NULL AUTO_INCREMENT,
                           `count` smallint(6) NOT NULL,
                           `name` varchar(255) NOT NULL,
                           `stackable` bit(1) DEFAULT NULL,
                           `techname` varchar(255) NOT NULL,
                           PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `item_template`;
CREATE TABLE `item_template` (
                               `id` bigint(20) NOT NULL AUTO_INCREMENT,
                               `category` bigint(20) NOT NULL,
                               `cleanname` varchar(255) DEFAULT NULL,
                               `data` json DEFAULT NULL,
                               `name` varchar(255) NOT NULL,
                               `techname` varchar(255) DEFAULT NULL,
                               `source` enum('private','official','community') DEFAULT 'official',
                               `sourceuserid` bigint(20) DEFAULT NULL,
                               `sourceusernamecache` text,
                               PRIMARY KEY (`id`),
                               UNIQUE KEY `item_template_techname_uindex` (`techname`),
                               KEY `IX_item_template_category` (`category`),
                               KEY `item_template_user_id_fk` (`sourceuserid`),
                               CONSTRAINT `FK_item_template_item_category_category` FOREIGN KEY (`category`) REFERENCES `item_template_category` (`id`) ON DELETE CASCADE,
                               CONSTRAINT `item_template_user_id_fk` FOREIGN KEY (`sourceuserid`) REFERENCES `user` (`id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `item_template_category`;
CREATE TABLE `item_template_category` (
                                        `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                        `description` varchar(255) NOT NULL,
                                        `name` varchar(255) NOT NULL,
                                        `note` longtext NOT NULL,
                                        `techname` varchar(255) NOT NULL DEFAULT '',
                                        `section` bigint(20) NOT NULL,
                                        PRIMARY KEY (`id`),
                                        KEY `IX_item_category_type` (`section`),
                                        CONSTRAINT `FK_item_category_item_type_type` FOREIGN KEY (`section`) REFERENCES `item_template_section` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `item_template_modifier`;
CREATE TABLE `item_template_modifier` (
                                        `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                        `itemTemplate` bigint(20) NOT NULL,
                                        `requirejob` bigint(20) DEFAULT NULL,
                                        `requireorigin` bigint(20) DEFAULT NULL,
                                        `special` varchar(2048) DEFAULT NULL,
                                        `stat` varchar(255) NOT NULL,
                                        `type` varchar(255) NOT NULL DEFAULT 'ADD',
                                        `value` bigint(20) NOT NULL,
                                        PRIMARY KEY (`id`),
                                        KEY `IX_item_effect_item` (`itemTemplate`),
                                        KEY `IX_item_effect_requirejob` (`requirejob`),
                                        KEY `IX_item_effect_requireorigin` (`requireorigin`),
                                        KEY `IX_item_effect_stat` (`stat`),
                                        CONSTRAINT `FK_item_effect_item_template_item` FOREIGN KEY (`itemTemplate`) REFERENCES `item_template` (`id`) ON DELETE CASCADE,
                                        CONSTRAINT `FK_item_effect_job_requirejob` FOREIGN KEY (`requirejob`) REFERENCES `job` (`id`) ON DELETE NO ACTION,
                                        CONSTRAINT `FK_item_effect_origin_requireorigin` FOREIGN KEY (`requireorigin`) REFERENCES `origin` (`id`) ON DELETE NO ACTION,
                                        CONSTRAINT `FK_item_effect_stat_stat` FOREIGN KEY (`stat`) REFERENCES `stat` (`name`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `item_template_requirement`;
CREATE TABLE `item_template_requirement` (
                                           `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                           `itemTemplate` bigint(20) NOT NULL,
                                           `maxvalue` bigint(20) DEFAULT NULL,
                                           `minvalue` bigint(20) DEFAULT NULL,
                                           `stat` varchar(255) NOT NULL,
                                           PRIMARY KEY (`id`),
                                           KEY `IX_item_requirement_item` (`itemTemplate`),
                                           KEY `IX_item_requirement_stat` (`stat`),
                                           CONSTRAINT `FK_item_requirement_item_template_item` FOREIGN KEY (`itemTemplate`) REFERENCES `item_template` (`id`) ON DELETE CASCADE,
                                           CONSTRAINT `FK_item_requirement_stat_stat` FOREIGN KEY (`stat`) REFERENCES `stat` (`name`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `item_template_section`;
CREATE TABLE `item_template_section` (
                                       `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                       `name` varchar(255) NOT NULL,
                                       `note` longtext NOT NULL,
                                       `special` varchar(2048) NOT NULL,
                                       PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `item_template_skill`;
CREATE TABLE `item_template_skill` (
                                     `skill` bigint(20) NOT NULL,
                                     `itemTemplate` bigint(20) NOT NULL,
                                     PRIMARY KEY (`skill`,`itemTemplate`),
                                     KEY `IX_item_skill_item` (`itemTemplate`),
                                     KEY `IX_item_skill_skill` (`skill`),
                                     CONSTRAINT `FK_item_skill_item_template_item` FOREIGN KEY (`itemTemplate`) REFERENCES `item_template` (`id`) ON DELETE CASCADE,
                                     CONSTRAINT `FK_item_skill_skill_skill` FOREIGN KEY (`skill`) REFERENCES `skill` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `item_template_skill_modifiers`;
CREATE TABLE `item_template_skill_modifiers` (
                                               `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                               `itemTemplate` bigint(20) NOT NULL,
                                               `skill` bigint(20) NOT NULL,
                                               `value` smallint(6) NOT NULL,
                                               PRIMARY KEY (`id`),
                                               KEY `IX_item_skill_modifiers_item` (`itemTemplate`),
                                               KEY `IX_item_skill_modifiers_skill` (`skill`),
                                               CONSTRAINT `FK_item_skill_modifiers_item_template_item` FOREIGN KEY (`itemTemplate`) REFERENCES `item_template` (`id`) ON DELETE CASCADE,
                                               CONSTRAINT `FK_item_skill_modifiers_skill_skill` FOREIGN KEY (`skill`) REFERENCES `skill` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `item_template_slot`;
CREATE TABLE `item_template_slot` (
                                    `slot` bigint(20) NOT NULL,
                                    `itemTemplate` bigint(20) NOT NULL,
                                    PRIMARY KEY (`slot`,`itemTemplate`),
                                    KEY `IX_item_template_slot_item` (`itemTemplate`),
                                    CONSTRAINT `FK_item_template_slot_item_slot_slot` FOREIGN KEY (`slot`) REFERENCES `item_slot` (`id`) ON DELETE CASCADE,
                                    CONSTRAINT `FK_item_template_slot_item_template_item` FOREIGN KEY (`itemTemplate`) REFERENCES `item_template` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `item_template_unskill`;
CREATE TABLE `item_template_unskill` (
                                       `skill` bigint(20) NOT NULL,
                                       `itemTemplate` bigint(20) NOT NULL,
                                       PRIMARY KEY (`skill`,`itemTemplate`),
                                       KEY `IX_item_unskill_item` (`itemTemplate`),
                                       KEY `IX_item_unskill_skill` (`skill`),
                                       CONSTRAINT `FK_item_unskill_item_template_item` FOREIGN KEY (`itemTemplate`) REFERENCES `item_template` (`id`) ON DELETE CASCADE,
                                       CONSTRAINT `FK_item_unskill_skill_skill` FOREIGN KEY (`skill`) REFERENCES `skill` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `item_type`;
CREATE TABLE `item_type` (
                           `id` int(11) NOT NULL AUTO_INCREMENT,
                           `techName` varchar(255) NOT NULL,
                           `displayName` varchar(255) NOT NULL,
                           PRIMARY KEY (`id`),
                           UNIQUE KEY `item_type_displayName_uindex` (`displayName`),
                           UNIQUE KEY `item_type_name_uindex` (`techName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `job`;
CREATE TABLE `job` (
                     `id` bigint(20) NOT NULL AUTO_INCREMENT,
                     `baseat` smallint(6) DEFAULT NULL,
                     `baseea` smallint(6) DEFAULT NULL,
                     `baseev` smallint(6) DEFAULT NULL,
                     `baseprd` smallint(6) DEFAULT NULL,
                     `bonusev` smallint(6) DEFAULT NULL,
                     `diceealevelup` smallint(6) DEFAULT NULL,
                     `factorev` double DEFAULT NULL,
                     `informations` longtext,
                     `internalname` varchar(255) DEFAULT NULL,
                     `ismagic` bit(1) DEFAULT b'0',
                     `maxarmorpr` smallint(6) DEFAULT NULL,
                     `maxload` bigint(20) DEFAULT NULL,
                     `name` varchar(255) DEFAULT NULL,
                     `parentjob` bigint(20) DEFAULT NULL,
                     `playerDescription` longtext,
                     `playerSummary` text,
                     `flags` json DEFAULT NULL,
                     PRIMARY KEY (`id`),
                     KEY `IX_job_parentjob` (`parentjob`),
                     CONSTRAINT `FK_job_job_parentjob` FOREIGN KEY (`parentjob`) REFERENCES `job` (`id`) ON DELETE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `job_bonus`;
CREATE TABLE `job_bonus` (
                           `id` bigint(20) NOT NULL AUTO_INCREMENT,
                           `description` longtext NOT NULL,
                           `jobid` bigint(20) NOT NULL,
                           `flags` json DEFAULT NULL,
                           PRIMARY KEY (`id`),
                           KEY `IX_job_bonus_jobid` (`jobid`),
                           CONSTRAINT `FK_job_bonus_job_jobid` FOREIGN KEY (`jobid`) REFERENCES `job` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `job_origin_blacklist`;
CREATE TABLE `job_origin_blacklist` (
                                      `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                      `job` bigint(20) NOT NULL,
                                      `origin` bigint(20) NOT NULL,
                                      PRIMARY KEY (`id`),
                                      KEY `IX_job_origin_blacklist_job` (`job`),
                                      KEY `IX_job_origin_blacklist_origin` (`origin`),
                                      CONSTRAINT `FK_job_origin_blacklist_job_job` FOREIGN KEY (`job`) REFERENCES `job` (`id`) ON DELETE CASCADE,
                                      CONSTRAINT `FK_job_origin_blacklist_origin_origin` FOREIGN KEY (`origin`) REFERENCES `origin` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `job_origin_whitelist`;
CREATE TABLE `job_origin_whitelist` (
                                      `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                      `job` bigint(20) NOT NULL,
                                      `origin` bigint(20) NOT NULL,
                                      PRIMARY KEY (`id`),
                                      KEY `IX_job_origin_whitelist_job` (`job`),
                                      KEY `IX_job_origin_whitelist_origin` (`origin`),
                                      CONSTRAINT `FK_job_origin_whitelist_job_job` FOREIGN KEY (`job`) REFERENCES `job` (`id`) ON DELETE CASCADE,
                                      CONSTRAINT `FK_job_origin_whitelist_origin_origin` FOREIGN KEY (`origin`) REFERENCES `origin` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `job_requirement`;
CREATE TABLE `job_requirement` (
                                 `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                 `jobid` bigint(20) NOT NULL,
                                 `maxvalue` bigint(20) DEFAULT NULL,
                                 `minvalue` bigint(20) DEFAULT NULL,
                                 `stat` varchar(255) NOT NULL,
                                 PRIMARY KEY (`id`),
                                 KEY `IX_job_requirement_jobid` (`jobid`),
                                 KEY `IX_job_requirement_stat` (`stat`),
                                 CONSTRAINT `FK_job_requirement_job_jobid` FOREIGN KEY (`jobid`) REFERENCES `job` (`id`) ON DELETE CASCADE,
                                 CONSTRAINT `FK_job_requirement_stat_stat` FOREIGN KEY (`stat`) REFERENCES `stat` (`name`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `job_restrict`;
CREATE TABLE `job_restrict` (
                              `id` bigint(20) NOT NULL AUTO_INCREMENT,
                              `jobid` bigint(20) NOT NULL,
                              `text` longtext NOT NULL,
                              `flags` json DEFAULT NULL,
                              PRIMARY KEY (`id`),
                              KEY `IX_job_restrict_jobid` (`jobid`),
                              CONSTRAINT `FK_job_restrict_job_jobid` FOREIGN KEY (`jobid`) REFERENCES `job` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `job_skill`;
CREATE TABLE `job_skill` (
                           `jobid` bigint(20) NOT NULL,
                           `skillid` bigint(20) NOT NULL,
                           `default` bit(1) NOT NULL,
                           PRIMARY KEY (`jobid`,`skillid`),
                           KEY `IX_job_skill_skillid` (`skillid`),
                           CONSTRAINT `FK_job_skill_job_jobid` FOREIGN KEY (`jobid`) REFERENCES `job` (`id`) ON DELETE CASCADE,
                           CONSTRAINT `FK_job_skill_skill_skillid` FOREIGN KEY (`skillid`) REFERENCES `skill` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `location`;
CREATE TABLE `location` (
                          `id` bigint(20) NOT NULL AUTO_INCREMENT,
                          `data` json DEFAULT NULL,
                          `name` varchar(255) NOT NULL,
                          `parent` bigint(20) DEFAULT NULL,
                          PRIMARY KEY (`id`),
                          KEY `IX_location_parent` (`parent`),
                          CONSTRAINT `FK_location_location_parent` FOREIGN KEY (`parent`) REFERENCES `location` (`id`) ON DELETE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `location_map`;
CREATE TABLE `location_map` (
                              `id` bigint(20) NOT NULL AUTO_INCREMENT,
                              `data` json NOT NULL,
                              `file` varchar(255) NOT NULL,
                              `gm` bit(1) NOT NULL,
                              `location` bigint(20) NOT NULL,
                              `name` varchar(255) NOT NULL,
                              PRIMARY KEY (`id`),
                              KEY `IX_location_map_location` (`location`),
                              CONSTRAINT `FK_location_map_location_location` FOREIGN KEY (`location`) REFERENCES `location` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `loot`;
CREATE TABLE `loot` (
                      `id` bigint(20) NOT NULL AUTO_INCREMENT,
                      `dead` datetime(6) DEFAULT NULL,
                      `groupid` bigint(20) NOT NULL,
                      `name` varchar(255) NOT NULL,
                      `visibleForPlayer` bit(1) NOT NULL,
                      PRIMARY KEY (`id`),
                      KEY `IX_loot_groupid` (`groupid`),
                      CONSTRAINT `FK_loot_group_groupid` FOREIGN KEY (`groupid`) REFERENCES `group` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `monster`;
CREATE TABLE `monster` (
                         `id` bigint(20) NOT NULL AUTO_INCREMENT,
                         `data` json DEFAULT NULL,
                         `dead` datetime DEFAULT NULL,
                         `group` bigint(20) NOT NULL,
                         `lootid` bigint(20) DEFAULT NULL,
                         `name` varchar(255) NOT NULL,
                         `targetcharacterid` bigint(20) DEFAULT NULL,
                         `targetmonsterid` bigint(20) DEFAULT NULL,
                         `modifiers` json DEFAULT NULL,
                         PRIMARY KEY (`id`),
                         KEY `IX_monster_group` (`group`),
                         KEY `IX_monster_lootid` (`lootid`),
                         KEY `IX_monster_targetcharacterid` (`targetcharacterid`),
                         KEY `IX_monster_targetmonsterid` (`targetmonsterid`),
                         CONSTRAINT `FK_monster_character_targetcharacterid` FOREIGN KEY (`targetcharacterid`) REFERENCES `character` (`id`) ON DELETE SET NULL,
                         CONSTRAINT `FK_monster_group_group` FOREIGN KEY (`group`) REFERENCES `group` (`id`) ON DELETE CASCADE,
                         CONSTRAINT `FK_monster_loot_lootid` FOREIGN KEY (`lootid`) REFERENCES `loot` (`id`) ON DELETE SET NULL,
                         CONSTRAINT `FK_monster_monster_targetmonsterid` FOREIGN KEY (`targetmonsterid`) REFERENCES `monster` (`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `monster_category`;
CREATE TABLE `monster_category` (
                                  `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                  `name` varchar(255) NOT NULL,
                                  `typeid` bigint(20) NOT NULL,
                                  PRIMARY KEY (`id`),
                                  KEY `monster_category_monster_type_id_fk` (`typeid`),
                                  CONSTRAINT `monster_category_monster_type_id_fk` FOREIGN KEY (`typeid`) REFERENCES `monster_type` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `monster_location`;
CREATE TABLE `monster_location` (
                                  `monsterid` bigint(20) NOT NULL,
                                  `locationid` bigint(20) NOT NULL,
                                  PRIMARY KEY (`monsterid`,`locationid`),
                                  KEY `IX_monster_location_locationid` (`locationid`),
                                  CONSTRAINT `FK_monster_location_location_locationid` FOREIGN KEY (`locationid`) REFERENCES `location` (`id`) ON DELETE CASCADE,
                                  CONSTRAINT `FK_monster_location_monster_template_monsterid` FOREIGN KEY (`monsterid`) REFERENCES `monster_template` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `monster_template`;
CREATE TABLE `monster_template` (
                                  `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                  `data` json NOT NULL,
                                  `name` varchar(255) NOT NULL,
                                  `categoryid` bigint(20) NOT NULL,
                                  PRIMARY KEY (`id`),
                                  KEY `IX_monster_template_type` (`categoryid`),
                                  CONSTRAINT `FK_monster_template_monster_type_type` FOREIGN KEY (`categoryid`) REFERENCES `monster_type` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `monster_template_simple_inventory`;
CREATE TABLE `monster_template_simple_inventory` (
                                                   `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                                   `chance` float NOT NULL,
                                                   `itemtemplateid` bigint(20) NOT NULL,
                                                   `maxCount` int(11) NOT NULL,
                                                   `minCount` int(11) NOT NULL,
                                                   `monstertemplateid` bigint(20) NOT NULL,
                                                   PRIMARY KEY (`id`),
                                                   KEY `IX_monster_template_simple_inventory_itemtemplateid` (`itemtemplateid`),
                                                   KEY `IX_monster_template_simple_inventory_monstertemplateid` (`monstertemplateid`),
                                                   CONSTRAINT `FK_monster_template_simple_inventory_item_template_itemtemplatei` FOREIGN KEY (`itemtemplateid`) REFERENCES `item_template` (`id`) ON DELETE CASCADE,
                                                   CONSTRAINT `FK_monster_template_simple_inventory_monster_template_monstertem` FOREIGN KEY (`monstertemplateid`) REFERENCES `monster_template` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `monster_trait`;
CREATE TABLE `monster_trait` (
                               `id` bigint(20) NOT NULL AUTO_INCREMENT,
                               `description` longtext NOT NULL,
                               `levels` json DEFAULT NULL,
                               `name` varchar(255) NOT NULL,
                               PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `monster_type`;
CREATE TABLE `monster_type` (
                              `id` bigint(20) NOT NULL AUTO_INCREMENT,
                              `name` varchar(255) NOT NULL,
                              PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `origin`;
CREATE TABLE `origin` (
                        `id` bigint(20) NOT NULL AUTO_INCREMENT,
                        `advantage` longtext,
                        `baseea` smallint(6) DEFAULT NULL,
                        `baseev` smallint(6) DEFAULT NULL,
                        `bonusat` smallint(6) DEFAULT NULL,
                        `bonusprd` smallint(6) DEFAULT NULL,
                        `description` longtext NOT NULL,
                        `diceevlevelup` int(11) NOT NULL,
                        `maxarmorpr` smallint(6) DEFAULT NULL,
                        `maxload` bigint(20) DEFAULT NULL,
                        `name` varchar(255) NOT NULL,
                        `size` longtext,
                        `speedmodifier` smallint(6) DEFAULT NULL,
                        `playerDescription` longtext,
                        `playerSummary` text,
                        `flags` json DEFAULT NULL,
                        PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `origin_bonus`;
CREATE TABLE `origin_bonus` (
                              `id` bigint(20) NOT NULL AUTO_INCREMENT,
                              `description` longtext NOT NULL,
                              `originid` bigint(20) NOT NULL,
                              `flags` json DEFAULT NULL,
                              PRIMARY KEY (`id`),
                              KEY `IX_origin_bonus_originid` (`originid`),
                              CONSTRAINT `FK_origin_bonus_origin_originid` FOREIGN KEY (`originid`) REFERENCES `origin` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `origin_info`;
CREATE TABLE `origin_info` (
                             `id` bigint(20) NOT NULL AUTO_INCREMENT,
                             `description` longtext NOT NULL,
                             `originid` bigint(20) NOT NULL,
                             `title` varchar(255) NOT NULL,
                             PRIMARY KEY (`id`),
                             KEY `IX_origin_info_originid` (`originid`),
                             CONSTRAINT `FK_origin_info_origin_originid` FOREIGN KEY (`originid`) REFERENCES `origin` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `origin_requirement`;
CREATE TABLE `origin_requirement` (
                                    `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                    `maxvalue` bigint(20) DEFAULT NULL,
                                    `minvalue` bigint(20) DEFAULT NULL,
                                    `originid` bigint(20) NOT NULL,
                                    `stat` varchar(255) NOT NULL,
                                    PRIMARY KEY (`id`),
                                    KEY `IX_origin_requirement_originid` (`originid`),
                                    KEY `IX_origin_requirement_stat` (`stat`),
                                    CONSTRAINT `FK_origin_requirement_origin_originid` FOREIGN KEY (`originid`) REFERENCES `origin` (`id`) ON DELETE CASCADE,
                                    CONSTRAINT `FK_origin_requirement_stat_stat` FOREIGN KEY (`stat`) REFERENCES `stat` (`name`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `origin_restrict`;
CREATE TABLE `origin_restrict` (
                                 `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                 `originid` bigint(20) NOT NULL,
                                 `text` longtext NOT NULL,
                                 `flags` json DEFAULT NULL,
                                 PRIMARY KEY (`id`),
                                 KEY `IX_origin_restrict_originid` (`originid`),
                                 CONSTRAINT `FK_origin_restrict_origin_originid` FOREIGN KEY (`originid`) REFERENCES `origin` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `origin_skill`;
CREATE TABLE `origin_skill` (
                              `originid` bigint(20) NOT NULL,
                              `skillid` bigint(20) NOT NULL,
                              `default` bit(1) NOT NULL,
                              PRIMARY KEY (`originid`,`skillid`),
                              KEY `IX_origin_skill_skillid` (`skillid`),
                              CONSTRAINT `FK_origin_skill_origin_originid` FOREIGN KEY (`originid`) REFERENCES `origin` (`id`) ON DELETE CASCADE,
                              CONSTRAINT `FK_origin_skill_skill_skillid` FOREIGN KEY (`skillid`) REFERENCES `skill` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `quest`;
CREATE TABLE `quest` (
                       `id` bigint(20) NOT NULL AUTO_INCREMENT,
                       `data` longtext NOT NULL,
                       `group` bigint(20) NOT NULL,
                       `name` varchar(255) NOT NULL,
                       PRIMARY KEY (`id`),
                       KEY `IX_quest_group` (`group`),
                       CONSTRAINT `FK_quest_group_group` FOREIGN KEY (`group`) REFERENCES `group` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `quest_template`;
CREATE TABLE `quest_template` (
                                `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                `data` longtext NOT NULL,
                                `name` varchar(255) NOT NULL,
                                PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `skill`;
CREATE TABLE `skill` (
                       `id` bigint(20) NOT NULL AUTO_INCREMENT,
                       `description` longtext,
                       `name` varchar(255) NOT NULL,
                       `require` longtext,
                       `resist` longtext,
                       `roleplay` longtext,
                       `stat` varchar(255) DEFAULT NULL,
                       `test` smallint(6) DEFAULT NULL,
                       `using` longtext,
                       `playerDescription` longtext,
                       `flags` json DEFAULT NULL,
                       PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `skill_effect`;
CREATE TABLE `skill_effect` (
                              `skill` bigint(20) NOT NULL,
                              `stat` varchar(255) NOT NULL,
                              `value` bigint(20) NOT NULL,
                              PRIMARY KEY (`skill`,`stat`),
                              KEY `IX_skill_effect_skill` (`skill`),
                              KEY `IX_skill_effect_stat` (`stat`),
                              CONSTRAINT `FK_skill_effect_skill_skill` FOREIGN KEY (`skill`) REFERENCES `skill` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `speciality`;
CREATE TABLE `speciality` (
                            `id` bigint(20) NOT NULL AUTO_INCREMENT,
                            `description` longtext NOT NULL,
                            `job` bigint(20) NOT NULL,
                            `name` varchar(255) NOT NULL,
                            `flags` json DEFAULT NULL,
                            PRIMARY KEY (`id`),
                            KEY `IX_speciality_job` (`job`),
                            CONSTRAINT `FK_speciality_job_job` FOREIGN KEY (`job`) REFERENCES `job` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `speciality_modifier`;
CREATE TABLE `speciality_modifier` (
                                     `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                     `speciality` bigint(20) NOT NULL,
                                     `stat` varchar(255) NOT NULL,
                                     `value` bigint(20) NOT NULL,
                                     PRIMARY KEY (`id`),
                                     KEY `IX_speciality_modifier_speciality` (`speciality`),
                                     KEY `IX_speciality_modifier_stat` (`stat`),
                                     CONSTRAINT `FK_speciality_modifier_speciality_speciality` FOREIGN KEY (`speciality`) REFERENCES `speciality` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `speciality_special`;
CREATE TABLE `speciality_special` (
                                    `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                    `description` longtext NOT NULL,
                                    `isbonus` bit(1) NOT NULL,
                                    `speciality` bigint(20) NOT NULL,
                                    `flags` json DEFAULT NULL,
                                    PRIMARY KEY (`id`),
                                    KEY `IX_speciality_special_speciality` (`speciality`),
                                    CONSTRAINT `FK_speciality_special_speciality_speciality` FOREIGN KEY (`speciality`) REFERENCES `speciality` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `spell`;
CREATE TABLE `spell` (
                       `id` bigint(20) NOT NULL AUTO_INCREMENT,
                       `category` bigint(20) NOT NULL,
                       `cost` bigint(20) NOT NULL,
                       `description` longtext NOT NULL,
                       `distance` bigint(20) DEFAULT NULL,
                       `distancenote` longtext,
                       `durationeffect` bigint(20) NOT NULL,
                       `durationeffectunit` varchar(255) NOT NULL,
                       `durationprepare` bigint(20) NOT NULL,
                       `durationprepareunit` varchar(255) NOT NULL,
                       `name` varchar(255) NOT NULL,
                       `speed` bigint(20) NOT NULL,
                       `test` bigint(20) NOT NULL,
                       `teststat` varchar(255) NOT NULL,
                       `type` varchar(255) NOT NULL,
                       `variablecost` bit(1) NOT NULL,
                       PRIMARY KEY (`id`),
                       KEY `IX_spell_category` (`category`),
                       KEY `IX_spell_teststat` (`teststat`),
                       CONSTRAINT `FK_spell_spell_category_category` FOREIGN KEY (`category`) REFERENCES `spell_category` (`id`) ON DELETE CASCADE,
                       CONSTRAINT `FK_spell_stat_teststat` FOREIGN KEY (`teststat`) REFERENCES `stat` (`name`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `spell_category`;
CREATE TABLE `spell_category` (
                                `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                `default` bit(1) NOT NULL,
                                `name` varchar(255) NOT NULL,
                                PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `stat`;
CREATE TABLE `stat` (
                      `name` varchar(255) NOT NULL,
                      `bonus` longtext,
                      `description` longtext NOT NULL,
                      `displayname` varchar(255) NOT NULL,
                      `penality` longtext,
                      PRIMARY KEY (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `user`;
CREATE TABLE `user` (
                      `id` bigint(20) NOT NULL AUTO_INCREMENT,
                      `admin` bit(1) NOT NULL DEFAULT b'0',
                      `displayname` varchar(255) DEFAULT NULL,
                      `fbid` varchar(255) DEFAULT NULL,
                      `password` varchar(255) DEFAULT NULL,
                      `username` varchar(255) DEFAULT NULL,
                      `googleid` varchar(255) DEFAULT NULL,
                      `twitterid` varchar(255) DEFAULT NULL,
                      `liveid` varchar(255) DEFAULT NULL,
                      PRIMARY KEY (`id`),
                      UNIQUE KEY `IX_user_username` (`username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `user_session`;
CREATE TABLE `user_session` (
                              `id` bigint(20) NOT NULL AUTO_INCREMENT,
                              `expire` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                              `ip` varchar(255) DEFAULT NULL,
                              `key` varchar(255) NOT NULL,
                              `start` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
                              `userid` bigint(20) NOT NULL,
                              PRIMARY KEY (`id`),
                              UNIQUE KEY `IX_user_session_key` (`key`),
                              KEY `IX_user_session_userid` (`userid`),
                              CONSTRAINT `FK_user_session_user_userid` FOREIGN KEY (`userid`) REFERENCES `user` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

INSERT INTO `stat` (`name`, `bonus`, `description`, `displayname`, `penality`) VALUES
('AD',	'Si l\'ADRESSE est supérieure à 12 : +1 à l’attaque OU à la parade (un seul point quel que soit le score)\n',	'la caractéristique la plus souvent testée hors du combat. La plupart des actions risquées\nentreprises par un héros nécessitent une épreuve d\'adresse : escalader un mur, se faufiler, désamorcer un\npiège, grimper à un arbre, esquiver des coups... Bon nombre de compétences sont liées à l\'adresse. N\'hésitez\npas à lancer à la volée des épreuves d\'adresse quand un héros désire faire l\'équilibriste. Une bonne adresse\npermet également d\'améliorer l\'habileté au combat (attaque/parade) et l\'utilisation d\'armes de jet.',	'Adresse',	'une caractéristique ADRESSE de 8 ou inférieur : -1 à l’attaque OU à la parade'),
('AT',	NULL,	'',	'Attaque',	NULL),
('CHA',	NULL,	'représente l\'apparence du héros et son “aura”, prestigieuse ou non... Permet d\'influencer\nles gens, de les convaincre, et d\'une manière générale d\'influencer les dieux. C\'est la caractéristique\nprincipale des prêtres et des ménestrels.',	'Charisme',	NULL),
('COU',	NULL,	'utilisé principalement pour déterminer qui frappe le premier dans un combat au corps à\ncorps, on s\'en sert aussi pour gérer les réactions des héros (ou des monstres) à une situation stressante. Si le\nhéros se retrouve brusquement cerné par 3 orques, le MJ peut lui demander de passer une épreuve de\ncourage. La partie continue normalement s\'il réussit, mais en cas d\'échec il paniquera et tentera de\nprendre la fuite, ou bien il se mettra à pleurer',	'Courage',	NULL),
('CRIT',	NULL,	'donne 1 chance supplémentaire d\'obtenir un critique en combat sur 1D20, en épreuve AT ou PRD',	'Critiques',	NULL),
('CRIT-AT',	NULL,	'Donne 1 chance supplémentaire d\'obtenir un critique en combat sur 1D20, en épreuve AT',	'Critiques AT',	NULL),
('CRIT-MAG',	NULL,	'Donne 1 chance supplémentaire d\'obtenir un critique en combat sur 1D20, sur le lancement des sorts',	'Critique magique',	NULL),
('CRIT-PROD',	NULL,	'Donne 1 chance supplémentaire d\'obtenir un critique en combat sur 1D20, sur les prodige',	'Critique prodige',	NULL),
('EA',	NULL,	'cette énergie est utilisée par les mages/prêtres/paladins pour lancer des sorts et\nappeler des prodiges. Chaque action de ce type a un coût en PA (points astraux), en général dépendant de\nla puissance du sort. Les points dépensés sont retirés du capital de PA, en attendant d\'être récupérés soit\npar le repos, soit par l\'étude, soit par le transfert ou par l\'usage d\'une potion.',	'Énergie astrale',	NULL),
('ESQ',	NULL,	'',	'Esquive',	NULL),
('EV',	NULL,	'',	'Énergie vitale',	NULL),
('FO',	'Pour chaque point de FORCE supérieur à 12 : +1 point d’impact (dégâts des armes améliorés, au corps à corps ou à distance)\nLe bonus au dégâts sera donc de +1 pour FO 13, et de +3 pour FO 16, etc.',	'inutile de faire un dessin... La Force est utile en combat (elle détermine les dégâts entre autres)\nmais pas prépondérante : c\'est surtout l\'adresse qui est utile, car elle permet d\'augmenter les chances de\ntoucher. Cela dit, la force permet aux héros de pousser quelqu\'un du haut d\'une falaise, de défoncer une\nporte, de soulever un rocher, d\'utiliser des armes de bourrin, de transporter des charges.',	'Force',	'au contraire sur une carac. de FORCE de 8 ou inférieure : -1 point d’impact (dégâts des armes diminués, car mauviette)\nContrairement au bonus, le malus ne se cumule pas, car on considère que l\'arme de base, même maniée faiblement, peut\nblesser.'),
('INT',	'Pour chaque point d\'INTELLIGENCE supérieur à 12 : +1 point de dégâts des sorts (selon sortilège)\nLe bonus au dégâts sera donc de +1 pour INT 13, et de +3 pour INT 16, etc.\nIl s\'applique à chaque jet de dégât de sortilège : s\'il y a plusieurs cibles, il s\'appliquera donc à chaque cible',	'on teste l\'intelligence en particulier quand on pratique la magie... C\'est la\ncaractéristique qui sert à lancer la plupart des sortilèges ! Elle peut augmenter les dégâts des sorts. Elle\npermet aussi de réussir les potions... Parfois le MJ peut demander un test d\'intelligence pour sortir les\naventuriers d\'une situation. On s\'en sert aussi pour détecter les pièges, trouver son chemin dans un\nlabyrinthe...',	'Intelligence',	NULL),
('MV',	NULL,	'',	'Mouvement',	NULL),
('PI',	NULL,	'',	'Points d\'impacts',	NULL),
('PR',	NULL,	'',	'Protection',	NULL),
('PRD',	NULL,	'',	'Parade',	NULL),
('PR_MAGIC',	NULL,	'',	'PR_MAGIC',	NULL),
('RESM',	NULL,	'Dans certains cas, on peut tenter de résister à un sortilège – c\'est une\nmoyenne de plusieurs caractéristiques (COU, INT, FO) qui détermine la volonté du héros, sa capacité à ne\npas flancher face à une attaque magique. Les projectiles magiques (type boule de feu) ne sont pas remis en\nquestion par la résistance à la magie, car une fois lancés ils agissent comme des attaques physiques. En\nrevanche, de nombreux sorts plus élaborés (tels que les attaques mentales, les illusions, les malédictions)\npeuvent être évités (par les héros, mais aussi par les PNJs et les monstres) en réussissant une épreuve de\nrésistance à la magie. Dans tous les tableaux de magie, les sorts concernés par la résistance à la magie\ncomportent la mention « sauf résistance magie » dans la colonne « notes ». Utiliser cette caractéristique à\nbon escient permet d\'éviter que les mages ne deviennent des « tueurs de boss » en utilisant des sortilèges\nstupides sur des monstres ou des ennemis puissants. Ces derniers en effet ont souvent une bonne\nrésistance à la magie ! Important : En augmentant une ou plusieurs caractéristiques impliquées dans cette\nmoyenne, le héros devra recalculer sa résistance à la magie.',	'Résistance à la magie',	NULL);

-- 2018-11-08 01:56:15