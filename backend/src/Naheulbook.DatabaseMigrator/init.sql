-- Adminer 4.8.1 MySQL 5.5.5-10.9.8-MariaDB-1:10.9.8+maria~ubu2204 dump

SET NAMES utf8;
SET time_zone = '+00:00';
SET foreign_key_checks = 0;
SET sql_mode = 'NO_AUTO_VALUE_ON_ZERO';

CREATE TABLE `calendars` (
                             `id` bigint(20) NOT NULL AUTO_INCREMENT,
                             `endday` int(11) NOT NULL,
                             `name` varchar(255) NOT NULL,
                             `note` longtext DEFAULT NULL,
                             `startday` int(11) NOT NULL,
                             PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


SET NAMES utf8mb4;

CREATE TABLE `characters` (
                              `id` bigint(20) NOT NULL AUTO_INCREMENT,
                              `active` bit(1) NOT NULL DEFAULT b'1',
                              `ad` smallint(6) NOT NULL,
                              `cha` smallint(6) NOT NULL,
                              `color` varchar(255) NOT NULL DEFAULT '22DD22',
                              `cou` smallint(6) NOT NULL,
                              `ea` smallint(6) DEFAULT NULL,
                              `ev` smallint(6) DEFAULT NULL,
                              `experience` bigint(20) NOT NULL,
                              `fatepoint` smallint(6) NOT NULL DEFAULT 0,
                              `fo` smallint(6) NOT NULL,
                              `gmdata` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`gmdata`)),
                              `groupId` bigint(20) DEFAULT NULL,
                              `int` smallint(6) NOT NULL,
                              `isnpc` bit(1) NOT NULL DEFAULT b'0',
                              `level` smallint(6) NOT NULL,
                              `name` varchar(255) NOT NULL,
                              `originId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                              `sexe` varchar(255) NOT NULL,
                              `statbonusad` varchar(255) DEFAULT NULL,
                              `targetcharacterid` bigint(20) DEFAULT NULL,
                              `targetmonsterid` bigint(20) DEFAULT NULL,
                              `userId` bigint(20) DEFAULT NULL,
                              `notes` longtext DEFAULT NULL,
                              PRIMARY KEY (`id`),
                              KEY `IX_character_targetcharacterid` (`targetcharacterid`),
                              KEY `IX_character_targetmonsterid` (`targetmonsterid`),
                              KEY `IX_characters_originId` (`originId`),
                              KEY `IX_characters_groupId` (`groupId`),
                              KEY `IX_characters_userId` (`userId`),
                              CONSTRAINT `FK_character_character_targetcharacterid` FOREIGN KEY (`targetcharacterid`) REFERENCES `characters` (`id`) ON DELETE SET NULL,
                              CONSTRAINT `FK_character_monster_targetmonsterid` FOREIGN KEY (`targetmonsterid`) REFERENCES `monsters` (`id`) ON DELETE SET NULL,
                              CONSTRAINT `FK_characters_groupId_groups_id` FOREIGN KEY (`groupId`) REFERENCES `groups` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
                              CONSTRAINT `FK_characters_originId_origins_id` FOREIGN KEY (`originId`) REFERENCES `origins` (`id`),
                              CONSTRAINT `FK_characters_userId_users_id` FOREIGN KEY (`userId`) REFERENCES `users` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `character_history_entries` (
                                             `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                             `action` varchar(255) NOT NULL,
                                             `character` bigint(20) NOT NULL,
                                             `data` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`data`)),
                                             `date` timestamp NOT NULL DEFAULT current_timestamp(),
                                             `effect` bigint(20) DEFAULT NULL,
                                             `gm` bit(1) NOT NULL DEFAULT b'0',
                                             `info` longtext DEFAULT NULL,
                                             `item` bigint(20) DEFAULT NULL,
                                             `modifier` bigint(20) DEFAULT NULL,
                                             PRIMARY KEY (`id`),
                                             KEY `IX_character_history_character` (`character`),
                                             KEY `IX_character_history_effect` (`effect`),
                                             KEY `IX_character_history_item` (`item`),
                                             KEY `IX_character_history_modifier` (`modifier`),
                                             CONSTRAINT `FK_character_history_character_character` FOREIGN KEY (`character`) REFERENCES `characters` (`id`) ON DELETE CASCADE,
                                             CONSTRAINT `FK_character_history_character_modifier_modifier` FOREIGN KEY (`modifier`) REFERENCES `character_modifiers` (`id`),
                                             CONSTRAINT `FK_character_history_effect_effect` FOREIGN KEY (`effect`) REFERENCES `effects` (`id`) ON DELETE NO ACTION,
                                             CONSTRAINT `FK_character_history_item_item` FOREIGN KEY (`item`) REFERENCES `items` (`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `character_jobs` (
                                  `characterId` bigint(20) NOT NULL,
                                  `jobId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                  `order` bigint(20) NOT NULL,
                                  PRIMARY KEY (`jobId`,`characterId`),
                                  KEY `IX_character_jobs_jobId` (`jobId`),
                                  KEY `IX_character_jobs_characterId` (`characterId`),
                                  CONSTRAINT `FK_character_jobs_characterId_characters_id` FOREIGN KEY (`characterId`) REFERENCES `characters` (`id`),
                                  CONSTRAINT `FK_character_jobs_jobId_jobs_id` FOREIGN KEY (`jobId`) REFERENCES `jobs` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `character_modifiers` (
                                       `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                       `active` bit(1) NOT NULL DEFAULT b'1',
                                       `characterId` bigint(20) DEFAULT NULL,
                                       `combatcount` int(11) DEFAULT NULL,
                                       `currentcombatcount` int(11) DEFAULT NULL,
                                       `currenttimeduration` int(11) DEFAULT NULL,
                                       `duration` longtext DEFAULT NULL,
                                       `name` varchar(255) NOT NULL,
                                       `permanent` bit(1) NOT NULL,
                                       `reusable` bit(1) NOT NULL DEFAULT b'0',
                                       `timeduration` int(11) DEFAULT NULL,
                                       `lapcount` int(11) DEFAULT NULL,
                                       `currentlapcount` int(11) DEFAULT NULL,
                                       `durationtype` varchar(10) NOT NULL,
                                       `type` text DEFAULT NULL,
                                       `description` text DEFAULT NULL,
                                       `lapCountDecrement` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`lapCountDecrement`)),
                                       PRIMARY KEY (`id`),
                                       KEY `IX_character_modifiers_characterId` (`characterId`),
                                       CONSTRAINT `FK_character_modifiers_characterId_characters_id` FOREIGN KEY (`characterId`) REFERENCES `characters` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `character_modifier_values` (
                                             `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                             `characterModifierId` bigint(20) NOT NULL,
                                             `stat` varchar(255) NOT NULL,
                                             `type` varchar(255) NOT NULL DEFAULT 'ADD',
                                             `value` smallint(6) NOT NULL,
                                             PRIMARY KEY (`id`),
                                             KEY `IX_character_modifier_value_stat` (`stat`),
                                             KEY `IX_character_modifier_values_characterModifierId` (`characterModifierId`),
                                             CONSTRAINT `FK_character_modifier_value_stat_stat` FOREIGN KEY (`stat`) REFERENCES `stats` (`name`) ON DELETE CASCADE,
                                             CONSTRAINT `FK_character_modifier_values_characterModifierId` FOREIGN KEY (`characterModifierId`) REFERENCES `character_modifiers` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `character_skills` (
                                    `characterId` bigint(20) NOT NULL,
                                    `skillId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                    PRIMARY KEY (`characterId`,`skillId`),
                                    KEY `IX_character_skills_skillId` (`skillId`),
                                    KEY `IX_character_skills_characterId` (`characterId`),
                                    CONSTRAINT `FK_character_skills_characterId_characters_id` FOREIGN KEY (`characterId`) REFERENCES `characters` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
                                    CONSTRAINT `FK_character_skills_skillId_skills_id` FOREIGN KEY (`skillId`) REFERENCES `skills` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `character_specialities` (
                                          `characterId` bigint(20) NOT NULL,
                                          `specialityId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                          PRIMARY KEY (`characterId`,`specialityId`),
                                          KEY `IX_character_specialities_specialityId` (`specialityId`),
                                          KEY `IX_character_specialities_characterId` (`characterId`),
                                          CONSTRAINT `FK_character_specialities_characterId_characters_id` FOREIGN KEY (`characterId`) REFERENCES `characters` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
                                          CONSTRAINT `FK_character_specialities_specialityId_specialities_id` FOREIGN KEY (`specialityId`) REFERENCES `specialities` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `effects` (
                           `id` bigint(20) NOT NULL AUTO_INCREMENT,
                           `subCategoryId` bigint(20) NOT NULL,
                           `combatcount` int(11) DEFAULT NULL,
                           `description` longtext DEFAULT NULL,
                           `dice` smallint(6) DEFAULT NULL,
                           `duration` longtext DEFAULT NULL,
                           `name` varchar(255) NOT NULL,
                           `timeduration` int(11) DEFAULT NULL,
                           `lapcount` int(11) DEFAULT NULL,
                           `durationtype` varchar(10) NOT NULL,
                           PRIMARY KEY (`id`),
                           KEY `IX_effect_subCategoryId` (`subCategoryId`),
                           CONSTRAINT `FK_effect_effect_category_category` FOREIGN KEY (`subCategoryId`) REFERENCES `effect_subcategories` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `effect_modifiers` (
                                    `effectId` bigint(20) NOT NULL,
                                    `stat` varchar(255) NOT NULL,
                                    `type` varchar(255) NOT NULL DEFAULT 'ADD',
                                    `value` smallint(6) NOT NULL,
                                    PRIMARY KEY (`effectId`,`stat`),
                                    KEY `IX_effect_modifier_stat` (`stat`),
                                    KEY `IX_effect_modifiers_effectId` (`effectId`),
                                    CONSTRAINT `FK_effect_modifier_stat_stat` FOREIGN KEY (`stat`) REFERENCES `stats` (`name`) ON DELETE CASCADE,
                                    CONSTRAINT `FK_effect_modifiers_effectId_effects_id` FOREIGN KEY (`effectId`) REFERENCES `effects` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `effect_subcategories` (
                                        `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                        `dicecount` smallint(6) NOT NULL,
                                        `dicesize` smallint(6) NOT NULL,
                                        `name` varchar(255) NOT NULL,
                                        `note` longtext DEFAULT NULL,
                                        `typeid` bigint(20) NOT NULL,
                                        PRIMARY KEY (`id`),
                                        KEY `effect_category_effect_type_id_fk` (`typeid`),
                                        CONSTRAINT `effect_category_effect_type_id_fk` FOREIGN KEY (`typeid`) REFERENCES `effect_types` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `effect_types` (
                                `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                `name` varchar(255) DEFAULT NULL,
                                PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `events` (
                          `id` int(11) NOT NULL AUTO_INCREMENT,
                          `groupId` bigint(20) DEFAULT NULL,
                          `name` varchar(255) NOT NULL,
                          `description` text DEFAULT NULL,
                          `timestamp` bigint(20) NOT NULL,
                          PRIMARY KEY (`id`),
                          KEY `IX_events_groupId` (`groupId`),
                          CONSTRAINT `FK_events_groupId_groups_id` FOREIGN KEY (`groupId`) REFERENCES `groups` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `fights` (
                          `id` bigint(20) NOT NULL AUTO_INCREMENT,
                          `name` varchar(255) NOT NULL,
                          `groupid` bigint(20) NOT NULL,
                          PRIMARY KEY (`id`),
                          KEY `FK_fights_groupId_groups_id` (`groupid`),
                          CONSTRAINT `FK_fights_groupId_groups_id` FOREIGN KEY (`groupid`) REFERENCES `groups` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `gods` (
                        `id` int(11) NOT NULL AUTO_INCREMENT,
                        `displayname` varchar(255) NOT NULL,
                        `description` text DEFAULT NULL,
                        `techname` varchar(32) NOT NULL,
                        PRIMARY KEY (`id`),
                        UNIQUE KEY `god_displayname_uindex` (`displayname`),
                        UNIQUE KEY `god_techname_uindex` (`techname`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `groups` (
                          `id` bigint(20) NOT NULL AUTO_INCREMENT,
                          `combatlootid` bigint(20) DEFAULT NULL,
                          `data` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`data`)),
                          `masterId` bigint(20) DEFAULT NULL,
                          `name` varchar(255) NOT NULL,
                          `config` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`config`)),
                          PRIMARY KEY (`id`),
                          UNIQUE KEY `IX_group_combatlootid` (`combatlootid`),
                          KEY `IX_groups_masterId` (`masterId`),
                          CONSTRAINT `FK_group_loot_combatlootid` FOREIGN KEY (`combatlootid`) REFERENCES `loots` (`id`) ON DELETE SET NULL,
                          CONSTRAINT `FK_groups_masterId_users_id` FOREIGN KEY (`masterId`) REFERENCES `users` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `group_history_entries` (
                                         `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                         `action` varchar(255) NOT NULL,
                                         `data` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`data`)),
                                         `date` timestamp NOT NULL DEFAULT current_timestamp(),
                                         `gm` bit(1) NOT NULL,
                                         `group` bigint(20) NOT NULL,
                                         `info` longtext DEFAULT NULL,
                                         PRIMARY KEY (`id`),
                                         KEY `IX_group_history_group` (`group`),
                                         CONSTRAINT `FK_group_history_group_group` FOREIGN KEY (`group`) REFERENCES `groups` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `group_invitations` (
                                     `groupId` bigint(20) NOT NULL,
                                     `characterId` bigint(20) NOT NULL,
                                     `fromgroup` bit(1) NOT NULL,
                                     PRIMARY KEY (`groupId`,`characterId`),
                                     KEY `IX_group_invitations_groupId` (`groupId`),
                                     KEY `IX_group_invitations_characterId` (`characterId`),
                                     CONSTRAINT `FK_group_invitations_characterId_characters_id` FOREIGN KEY (`characterId`) REFERENCES `characters` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
                                     CONSTRAINT `FK_group_invitations_groupId_groups_id` FOREIGN KEY (`groupId`) REFERENCES `groups` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `items` (
                         `id` bigint(20) NOT NULL AUTO_INCREMENT,
                         `characterid` bigint(20) DEFAULT NULL,
                         `container` bigint(20) DEFAULT NULL,
                         `data` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL CHECK (json_valid(`data`)),
                         `itemTemplateId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                         `lootid` bigint(20) DEFAULT NULL,
                         `modifiers` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`modifiers`)),
                         `monsterid` bigint(20) DEFAULT NULL,
                         `lifetimetype` varchar(30) GENERATED ALWAYS AS (json_unquote(json_extract(`data`,'$.lifetime.durationType'))) VIRTUAL,
                         PRIMARY KEY (`id`),
                         KEY `IX_item_characterid` (`characterid`),
                         KEY `IX_item_container` (`container`),
                         KEY `IX_item_lootid` (`lootid`),
                         KEY `IX_item_monsterid` (`monsterid`),
                         KEY `IX_items_itemTemplateId` (`itemTemplateId`),
                         CONSTRAINT `FK_item_character_characterid` FOREIGN KEY (`characterid`) REFERENCES `characters` (`id`) ON DELETE CASCADE,
                         CONSTRAINT `FK_item_loot_lootid` FOREIGN KEY (`lootid`) REFERENCES `loots` (`id`) ON DELETE CASCADE,
                         CONSTRAINT `FK_item_monster_monsterid` FOREIGN KEY (`monsterid`) REFERENCES `monsters` (`id`) ON DELETE SET NULL,
                         CONSTRAINT `FK_items_itemTemplateId_item_templates_id` FOREIGN KEY (`itemTemplateId`) REFERENCES `item_templates` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `item_templates` (
                                  `subCategoryId` bigint(20) NOT NULL,
                                  `cleanname` varchar(255) DEFAULT NULL,
                                  `data` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`data`)),
                                  `name` varchar(255) NOT NULL,
                                  `techname` varchar(255) DEFAULT NULL,
                                  `source` enum('private','official','community') DEFAULT 'official',
                                  `sourceuserid` bigint(20) DEFAULT NULL,
                                  `sourceusernamecache` text DEFAULT NULL,
                                  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                  PRIMARY KEY (`id`),
                                  UNIQUE KEY `item_template_techname_uindex` (`techname`),
                                  KEY `IX_item_template_subCategoryId` (`subCategoryId`),
                                  KEY `item_template_user_id_fk` (`sourceuserid`),
                                  KEY `IX_item_templates_id` (`id`),
                                  CONSTRAINT `FK_item_template_item_category_category` FOREIGN KEY (`subCategoryId`) REFERENCES `item_template_subcategories` (`id`) ON DELETE CASCADE,
                                  CONSTRAINT `item_template_user_id_fk` FOREIGN KEY (`sourceuserid`) REFERENCES `users` (`id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `item_template_modifiers` (
                                           `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                           `itemTemplateId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                           `requiredJobId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
                                           `requiredOriginId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
                                           `special` varchar(2048) DEFAULT NULL,
                                           `stat` varchar(255) NOT NULL,
                                           `type` varchar(255) NOT NULL DEFAULT 'ADD',
                                           `value` bigint(20) NOT NULL,
                                           PRIMARY KEY (`id`),
                                           KEY `IX_item_effect_stat` (`stat`),
                                           KEY `IX_item_template_modifiers_requiredOriginId` (`requiredOriginId`),
                                           KEY `IX_item_template_modifiers_requiredJobId` (`requiredJobId`),
                                           KEY `IX_item_template_modifiers_itemTemplateId` (`itemTemplateId`),
                                           CONSTRAINT `FK_item_effect_stat_stat` FOREIGN KEY (`stat`) REFERENCES `stats` (`name`) ON DELETE CASCADE,
                                           CONSTRAINT `FK_item_template_modifiers_itemTemplateId_item_templates_id` FOREIGN KEY (`itemTemplateId`) REFERENCES `item_templates` (`id`),
                                           CONSTRAINT `FK_item_template_modifiers_requiredJobId_jobs_id` FOREIGN KEY (`requiredJobId`) REFERENCES `jobs` (`id`),
                                           CONSTRAINT `FK_item_template_modifiers_requiredOriginId_origins_id` FOREIGN KEY (`requiredOriginId`) REFERENCES `origins` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `item_template_requirements` (
                                              `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                              `itemTemplateId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                              `maxvalue` bigint(20) DEFAULT NULL,
                                              `minvalue` bigint(20) DEFAULT NULL,
                                              `stat` varchar(255) NOT NULL,
                                              PRIMARY KEY (`id`),
                                              KEY `IX_item_requirement_stat` (`stat`),
                                              KEY `IX_item_template_requirements_itemTemplateId` (`itemTemplateId`),
                                              CONSTRAINT `FK_item_requirement_stat_stat` FOREIGN KEY (`stat`) REFERENCES `stats` (`name`) ON DELETE CASCADE,
                                              CONSTRAINT `FK_item_template_requirements_itemTemplateId_item_templates_id` FOREIGN KEY (`itemTemplateId`) REFERENCES `item_templates` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `item_template_sections` (
                                          `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                          `name` varchar(255) NOT NULL,
                                          `note` longtext NOT NULL,
                                          `special` varchar(2048) NOT NULL,
                                          `icon` varchar(64) DEFAULT NULL,
                                          PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `item_template_skills` (
                                        `skillId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                        `itemTemplateId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                        PRIMARY KEY (`skillId`,`itemTemplateId`),
                                        KEY `IX_item_template_skills_skillId` (`skillId`),
                                        KEY `IX_item_template_skills_itemTemplateId` (`itemTemplateId`),
                                        CONSTRAINT `FK_item_template_skills_itemTemplateId_item_templates_id` FOREIGN KEY (`itemTemplateId`) REFERENCES `item_templates` (`id`),
                                        CONSTRAINT `FK_item_template_skills_skillId_skills_id` FOREIGN KEY (`skillId`) REFERENCES `skills` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `item_template_skill_modifiers` (
                                                 `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                                 `itemTemplateId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                                 `skillId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                                 `value` smallint(6) NOT NULL,
                                                 PRIMARY KEY (`id`),
                                                 KEY `IX_item_template_skill_modifiers_skillId` (`skillId`),
                                                 KEY `IX_item_template_skill_modifiers_itemTemplateId` (`itemTemplateId`),
                                                 CONSTRAINT `FK_item_template_skill_modifiers_itemTemplateId` FOREIGN KEY (`itemTemplateId`) REFERENCES `item_templates` (`id`),
                                                 CONSTRAINT `FK_item_template_skill_modifiers_skillId_skills_id` FOREIGN KEY (`skillId`) REFERENCES `skills` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `item_template_slots` (
                                       `slot` bigint(20) NOT NULL,
                                       `itemTemplateId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                       PRIMARY KEY (`slot`,`itemTemplateId`),
                                       KEY `IX_item_template_slots_itemTemplateId` (`itemTemplateId`),
                                       CONSTRAINT `FK_item_template_slot_item_slot_slot` FOREIGN KEY (`slot`) REFERENCES `slots` (`id`) ON DELETE CASCADE,
                                       CONSTRAINT `FK_item_template_slots_itemTemplateId_item_templates_id` FOREIGN KEY (`itemTemplateId`) REFERENCES `item_templates` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `item_template_subcategories` (
                                               `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                               `description` varchar(255) NOT NULL,
                                               `name` varchar(255) NOT NULL,
                                               `note` longtext NOT NULL,
                                               `techname` varchar(255) NOT NULL DEFAULT '',
                                               `section` bigint(20) NOT NULL,
                                               PRIMARY KEY (`id`),
                                               KEY `IX_item_category_type` (`section`),
                                               CONSTRAINT `FK_item_category_item_type_type` FOREIGN KEY (`section`) REFERENCES `item_template_sections` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `item_template_unskills` (
                                          `skillId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                          `itemTemplateId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                          PRIMARY KEY (`skillId`,`itemTemplateId`),
                                          KEY `IX_item_template_unskills_skillId` (`skillId`),
                                          KEY `IX_item_template_unskills_itemTemplateId` (`itemTemplateId`),
                                          CONSTRAINT `FK_item_template_unskills_itemTemplateId_item_templates_id` FOREIGN KEY (`itemTemplateId`) REFERENCES `item_templates` (`id`),
                                          CONSTRAINT `FK_item_template_unskills_skillId_skills_id` FOREIGN KEY (`skillId`) REFERENCES `skills` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `item_types` (
                              `id` int(11) NOT NULL AUTO_INCREMENT,
                              `techName` varchar(255) NOT NULL,
                              `displayName` varchar(255) NOT NULL,
                              PRIMARY KEY (`id`),
                              UNIQUE KEY `item_type_displayName_uindex` (`displayName`),
                              UNIQUE KEY `item_type_name_uindex` (`techName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `jobs` (
                        `informations` longtext DEFAULT NULL,
                        `ismagic` bit(1) DEFAULT b'0',
                        `name` varchar(255) NOT NULL,
                        `playerDescription` longtext NOT NULL,
                        `playerSummary` text NOT NULL,
                        `flags` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`flags`)),
                        `data` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL CHECK (json_valid(`data`)),
                        `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                        PRIMARY KEY (`id`),
                        KEY `IX_jobs_id` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `job_bonuses` (
                               `id` bigint(20) NOT NULL AUTO_INCREMENT,
                               `description` longtext NOT NULL,
                               `jobId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                               `flags` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`flags`)),
                               PRIMARY KEY (`id`),
                               KEY `IX_job_bonuses_jobId` (`jobId`),
                               CONSTRAINT `FK_job_bonuses_jobId_jobs_id` FOREIGN KEY (`jobId`) REFERENCES `jobs` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `job_requirements` (
                                    `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                    `jobId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                    `maxvalue` bigint(20) DEFAULT NULL,
                                    `minvalue` bigint(20) DEFAULT NULL,
                                    `stat` varchar(255) NOT NULL,
                                    PRIMARY KEY (`id`),
                                    KEY `IX_job_requirement_stat` (`stat`),
                                    KEY `IX_job_requirements_jobId` (`jobId`),
                                    CONSTRAINT `FK_job_requirement_stat_stat` FOREIGN KEY (`stat`) REFERENCES `stats` (`name`) ON DELETE CASCADE,
                                    CONSTRAINT `FK_job_requirements_jobId_jobs_id` FOREIGN KEY (`jobId`) REFERENCES `jobs` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `job_restrictions` (
                                    `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                    `jobId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                    `text` longtext NOT NULL,
                                    `flags` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`flags`)),
                                    PRIMARY KEY (`id`),
                                    KEY `IX_job_restrictions_jobId` (`jobId`),
                                    CONSTRAINT `FK_job_restrictions_jobId_jobs_id` FOREIGN KEY (`jobId`) REFERENCES `jobs` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `job_skills` (
                              `jobId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                              `skillId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                              `default` bit(1) NOT NULL,
                              PRIMARY KEY (`jobId`,`skillId`),
                              KEY `IX_job_skills_skillId` (`skillId`),
                              KEY `IX_job_skills_jobId` (`jobId`),
                              CONSTRAINT `FK_job_skills_jobId_jobs_id` FOREIGN KEY (`jobId`) REFERENCES `jobs` (`id`),
                              CONSTRAINT `FK_job_skills_skillId_skills_id` FOREIGN KEY (`skillId`) REFERENCES `skills` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `loots` (
                         `id` bigint(20) NOT NULL AUTO_INCREMENT,
                         `dead` datetime(6) DEFAULT NULL,
                         `groupid` bigint(20) NOT NULL,
                         `name` varchar(255) NOT NULL,
                         `visibleForPlayer` bit(1) NOT NULL,
                         PRIMARY KEY (`id`),
                         KEY `IX_loot_groupid` (`groupid`),
                         CONSTRAINT `FK_loot_group_groupid` FOREIGN KEY (`groupid`) REFERENCES `groups` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `maps` (
                        `id` int(11) NOT NULL AUTO_INCREMENT,
                        `name` varchar(255) NOT NULL,
                        `data` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL CHECK (json_valid(`data`)),
                        `imageData` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL CHECK (json_valid(`imageData`)),
                        PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `map_layers` (
                              `id` int(11) NOT NULL AUTO_INCREMENT,
                              `mapId` int(11) NOT NULL,
                              `name` varchar(255) NOT NULL,
                              `source` varchar(25) NOT NULL,
                              `userId` bigint(20) DEFAULT NULL,
                              `isGm` tinyint(1) NOT NULL,
                              PRIMARY KEY (`id`),
                              KEY `fk_map_layer_map` (`mapId`),
                              KEY `fk_map_layer_user` (`userId`),
                              CONSTRAINT `fk_map_layer_map` FOREIGN KEY (`mapId`) REFERENCES `maps` (`id`),
                              CONSTRAINT `fk_map_layer_user` FOREIGN KEY (`userId`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `map_markers` (
                               `id` int(11) NOT NULL AUTO_INCREMENT,
                               `layerId` int(11) NOT NULL,
                               `name` varchar(255) NOT NULL,
                               `type` varchar(25) NOT NULL,
                               `description` text DEFAULT NULL,
                               `markerInfo` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL CHECK (json_valid(`markerInfo`)),
                               PRIMARY KEY (`id`),
                               KEY `fk_map_markers_map_layers` (`layerId`),
                               CONSTRAINT `fk_map_markers_map_layers` FOREIGN KEY (`layerId`) REFERENCES `map_layers` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `map_marker_links` (
                                    `id` int(11) NOT NULL AUTO_INCREMENT,
                                    `mapMarkerId` int(11) NOT NULL,
                                    `name` varchar(255) DEFAULT NULL,
                                    `targetMapId` int(11) NOT NULL,
                                    `targetMapMarkerId` int(11) DEFAULT NULL,
                                    PRIMARY KEY (`id`),
                                    KEY `fk_map_markers_links_map_markers` (`mapMarkerId`),
                                    KEY `fk_map_markers_link_target_map` (`targetMapId`),
                                    KEY `fk_map_markers_links_target_map_marker` (`targetMapMarkerId`),
                                    CONSTRAINT `fk_map_markers_link_target_map` FOREIGN KEY (`targetMapId`) REFERENCES `maps` (`id`),
                                    CONSTRAINT `fk_map_markers_links_map_markers` FOREIGN KEY (`mapMarkerId`) REFERENCES `map_markers` (`id`),
                                    CONSTRAINT `fk_map_markers_links_target_map_marker` FOREIGN KEY (`targetMapMarkerId`) REFERENCES `map_markers` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `monsters` (
                            `id` bigint(20) NOT NULL AUTO_INCREMENT,
                            `data` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`data`)),
                            `dead` datetime DEFAULT NULL,
                            `group` bigint(20) NOT NULL,
                            `lootid` bigint(20) DEFAULT NULL,
                            `name` varchar(255) NOT NULL,
                            `targetcharacterid` bigint(20) DEFAULT NULL,
                            `targetmonsterid` bigint(20) DEFAULT NULL,
                            `modifiers` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`modifiers`)),
                            `fightId` bigint(20) DEFAULT NULL,
                            PRIMARY KEY (`id`),
                            KEY `IX_monster_group` (`group`),
                            KEY `IX_monster_lootid` (`lootid`),
                            KEY `IX_monster_targetcharacterid` (`targetcharacterid`),
                            KEY `IX_monster_targetmonsterid` (`targetmonsterid`),
                            KEY `FK_monsters_fightId_fights_id` (`fightId`),
                            CONSTRAINT `FK_monster_character_targetcharacterid` FOREIGN KEY (`targetcharacterid`) REFERENCES `characters` (`id`) ON DELETE SET NULL,
                            CONSTRAINT `FK_monster_group_group` FOREIGN KEY (`group`) REFERENCES `groups` (`id`) ON DELETE CASCADE,
                            CONSTRAINT `FK_monster_loot_lootid` FOREIGN KEY (`lootid`) REFERENCES `loots` (`id`) ON DELETE SET NULL,
                            CONSTRAINT `FK_monster_monster_targetmonsterid` FOREIGN KEY (`targetmonsterid`) REFERENCES `monsters` (`id`) ON DELETE SET NULL,
                            CONSTRAINT `FK_monsters_fightId_fights_id` FOREIGN KEY (`fightId`) REFERENCES `fights` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `monster_subcategories` (
                                         `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                         `name` varchar(255) NOT NULL,
                                         `typeid` bigint(20) NOT NULL,
                                         PRIMARY KEY (`id`),
                                         KEY `monster_category_monster_type_id_fk` (`typeid`),
                                         CONSTRAINT `monster_category_monster_type_id_fk` FOREIGN KEY (`typeid`) REFERENCES `monster_types` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `monster_templates` (
                                     `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                     `data` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL CHECK (json_valid(`data`)),
                                     `name` varchar(255) NOT NULL,
                                     `subCategoryId` bigint(20) NOT NULL,
                                     PRIMARY KEY (`id`),
                                     KEY `IX_monster_template_subCategoryId` (`subCategoryId`),
                                     CONSTRAINT `monster_templates_ibfk_1` FOREIGN KEY (`subCategoryId`) REFERENCES `monster_subcategories` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `monster_template_inventory_elements` (
                                                       `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                                       `chance` float NOT NULL,
                                                       `itemTemplateId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                                       `maxCount` int(11) NOT NULL,
                                                       `minCount` int(11) NOT NULL,
                                                       `monstertemplateid` bigint(20) NOT NULL,
                                                       PRIMARY KEY (`id`),
                                                       KEY `IX_monster_template_simple_inventory_monstertemplateid` (`monstertemplateid`),
                                                       KEY `IX_monster_template_inventory_elements_itemTemplateId` (`itemTemplateId`),
                                                       CONSTRAINT `FK_monster_template_inventory_elements_itemTemplateId` FOREIGN KEY (`itemTemplateId`) REFERENCES `item_templates` (`id`),
                                                       CONSTRAINT `FK_monster_template_simple_inventory_monster_template_monstertem` FOREIGN KEY (`monstertemplateid`) REFERENCES `monster_templates` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `monster_traits` (
                                  `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                  `description` longtext NOT NULL,
                                  `levels` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`levels`)),
                                  `name` varchar(255) NOT NULL,
                                  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `monster_types` (
                                 `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                 `name` varchar(255) NOT NULL,
                                 PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `npcs` (
                        `id` int(11) NOT NULL AUTO_INCREMENT,
                        `groupId` bigint(20) NOT NULL,
                        `name` varchar(255) NOT NULL,
                        `data` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL CHECK (json_valid(`data`)),
                        PRIMARY KEY (`id`),
                        KEY `FK_npcs_groupId_groups_id` (`groupId`),
                        CONSTRAINT `FK_npcs_groupId_groups_id` FOREIGN KEY (`groupId`) REFERENCES `groups` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `origins` (
                           `advantage` longtext DEFAULT NULL,
                           `description` longtext NOT NULL,
                           `name` varchar(255) NOT NULL,
                           `size` longtext DEFAULT NULL,
                           `playerDescription` longtext DEFAULT NULL,
                           `playerSummary` text DEFAULT NULL,
                           `flags` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`flags`)),
                           `data` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL CHECK (json_valid(`data`)),
                           `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                           PRIMARY KEY (`id`),
                           KEY `IX_origins_id` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `origin_bonuses` (
                                  `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                  `description` longtext NOT NULL,
                                  `originId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                  `flags` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`flags`)),
                                  PRIMARY KEY (`id`),
                                  KEY `IX_origin_bonuses_originId` (`originId`),
                                  CONSTRAINT `FK_origin_bonuses_originId_origins_id` FOREIGN KEY (`originId`) REFERENCES `origins` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `origin_information` (
                                      `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                      `description` longtext NOT NULL,
                                      `originId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                      `title` varchar(255) NOT NULL,
                                      PRIMARY KEY (`id`),
                                      KEY `IX_origin_information_originId` (`originId`),
                                      CONSTRAINT `FK_origin_information_originId_origins_id` FOREIGN KEY (`originId`) REFERENCES `origins` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `origin_random_name_urls` (
                                           `id` int(11) NOT NULL AUTO_INCREMENT,
                                           `originId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                           `sex` varchar(255) NOT NULL,
                                           `url` varchar(255) NOT NULL,
                                           PRIMARY KEY (`id`),
                                           UNIQUE KEY `IX_origin_random_name_urls_originId_sec` (`originId`,`sex`),
                                           KEY `IX_origin_random_name_urls_originId` (`originId`),
                                           CONSTRAINT `FK_origin_random_name_urls_originId_origins_id` FOREIGN KEY (`originId`) REFERENCES `origins` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `origin_requirements` (
                                       `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                       `maxvalue` bigint(20) DEFAULT NULL,
                                       `minvalue` bigint(20) DEFAULT NULL,
                                       `originId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                       `stat` varchar(255) NOT NULL,
                                       PRIMARY KEY (`id`),
                                       KEY `IX_origin_requirement_stat` (`stat`),
                                       KEY `IX_origin_requirements_originId` (`originId`),
                                       CONSTRAINT `FK_origin_requirement_stat_stat` FOREIGN KEY (`stat`) REFERENCES `stats` (`name`) ON DELETE CASCADE,
                                       CONSTRAINT `FK_origin_requirements_originId_origins_id` FOREIGN KEY (`originId`) REFERENCES `origins` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `origin_restrictions` (
                                       `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                       `originId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                       `text` longtext NOT NULL,
                                       `flags` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`flags`)),
                                       PRIMARY KEY (`id`),
                                       KEY `IX_origin_restrictions_originId` (`originId`),
                                       CONSTRAINT `FK_origin_restrictions_originId_origins_id` FOREIGN KEY (`originId`) REFERENCES `origins` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `origin_skills` (
                                 `originId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                 `skillId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                 `default` bit(1) NOT NULL,
                                 PRIMARY KEY (`originId`,`skillId`),
                                 KEY `IX_origin_skills_skillId` (`skillId`),
                                 KEY `IX_origin_skills_originId` (`originId`),
                                 CONSTRAINT `FK_origin_skills_originId_origins_id` FOREIGN KEY (`originId`) REFERENCES `origins` (`id`),
                                 CONSTRAINT `FK_origin_skills_skillId_skills_id` FOREIGN KEY (`skillId`) REFERENCES `skills` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `quests` (
                          `id` bigint(20) NOT NULL AUTO_INCREMENT,
                          `data` longtext NOT NULL,
                          `group` bigint(20) NOT NULL,
                          `name` varchar(255) NOT NULL,
                          PRIMARY KEY (`id`),
                          KEY `IX_quest_group` (`group`),
                          CONSTRAINT `FK_quest_group_group` FOREIGN KEY (`group`) REFERENCES `groups` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `quest_templates` (
                                   `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                   `data` longtext NOT NULL,
                                   `name` varchar(255) NOT NULL,
                                   PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `skills` (
                          `description` longtext DEFAULT NULL,
                          `name` varchar(255) NOT NULL,
                          `require` longtext DEFAULT NULL,
                          `resist` longtext DEFAULT NULL,
                          `roleplay` longtext DEFAULT NULL,
                          `stat` varchar(255) DEFAULT NULL,
                          `test` smallint(6) DEFAULT NULL,
                          `using` longtext DEFAULT NULL,
                          `playerDescription` longtext DEFAULT NULL,
                          `flags` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`flags`)),
                          `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                          PRIMARY KEY (`id`),
                          KEY `IX_skills_id` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `skill_effects` (
                                 `skillId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                 `stat` varchar(255) NOT NULL,
                                 `value` bigint(20) NOT NULL,
                                 PRIMARY KEY (`skillId`,`stat`),
                                 KEY `IX_skill_effect_stat` (`stat`),
                                 KEY `IX_skill_effects_skillId` (`skillId`),
                                 CONSTRAINT `FK_skill_effects_skillId_skills_id` FOREIGN KEY (`skillId`) REFERENCES `skills` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `slots` (
                         `id` bigint(20) NOT NULL AUTO_INCREMENT,
                         `count` smallint(6) NOT NULL,
                         `name` varchar(255) NOT NULL,
                         `stackable` bit(1) DEFAULT NULL,
                         `techname` varchar(255) NOT NULL,
                         PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `specialities` (
                                `description` longtext NOT NULL,
                                `jobId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                `name` varchar(255) NOT NULL,
                                `flags` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`flags`)),
                                `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                PRIMARY KEY (`id`),
                                KEY `IX_specialities_jobId` (`jobId`),
                                KEY `IX_specialities_id` (`id`),
                                CONSTRAINT `FK_specialities_jobId_jobs_id` FOREIGN KEY (`jobId`) REFERENCES `jobs` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `speciality_modifiers` (
                                        `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                        `specialityId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                        `stat` varchar(255) NOT NULL,
                                        `value` bigint(20) NOT NULL,
                                        PRIMARY KEY (`id`),
                                        KEY `IX_speciality_modifier_stat` (`stat`),
                                        KEY `IX_speciality_modifiers_specialityId` (`specialityId`),
                                        CONSTRAINT `FK_speciality_modifiers_specialityId_specialities_id` FOREIGN KEY (`specialityId`) REFERENCES `specialities` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `speciality_specials` (
                                       `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                       `description` longtext NOT NULL,
                                       `isbonus` bit(1) NOT NULL,
                                       `specialityId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                       `flags` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`flags`)),
                                       PRIMARY KEY (`id`),
                                       KEY `IX_speciality_specials_specialityId` (`specialityId`),
                                       CONSTRAINT `FK_speciality_specials_specialityId_specialities_id` FOREIGN KEY (`specialityId`) REFERENCES `specialities` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `spells` (
                          `id` bigint(20) NOT NULL AUTO_INCREMENT,
                          `category` bigint(20) NOT NULL,
                          `cost` bigint(20) NOT NULL,
                          `description` longtext NOT NULL,
                          `distance` bigint(20) DEFAULT NULL,
                          `distancenote` longtext DEFAULT NULL,
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
                          CONSTRAINT `FK_spell_spell_category_category` FOREIGN KEY (`category`) REFERENCES `spell_categories` (`id`) ON DELETE CASCADE,
                          CONSTRAINT `FK_spell_stat_teststat` FOREIGN KEY (`teststat`) REFERENCES `stats` (`name`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `spell_categories` (
                                    `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                    `default` bit(1) NOT NULL,
                                    `name` varchar(255) NOT NULL,
                                    PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `stats` (
                         `name` varchar(255) NOT NULL,
                         `bonus` longtext DEFAULT NULL,
                         `description` longtext NOT NULL,
                         `displayname` varchar(255) NOT NULL,
                         `penality` longtext DEFAULT NULL,
                         PRIMARY KEY (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

INSERT INTO `stats` (`name`, `bonus`, `description`, `displayname`, `penality`) VALUES
('AD', 'Si l\'ADRESSE est supérieure à 12 : +1 à l’attaque OU à la parade (un seul point quel que soit le score)\n',   'la caractéristique la plus souvent testée hors du combat. La plupart des actions risquées\nentreprises par un héros nécessitent une épreuve d\'adresse : escalader un mur, se faufiler, désamorcer un\npiège, grimper à un arbre, esquiver des coups... Bon nombre de compétences sont liées à l\'adresse. N\'hésitez\npas à lancer à la volée des épreuves d\'adresse quand un héros désire faire l\'équilibriste. Une bonne adresse\npermet également d\'améliorer l\'habileté au combat (attaque/parade) et l\'utilisation d\'armes de jet.',       'Adresse',      'une caractéristique ADRESSE de 8 ou inférieur : -1 à l’attaque OU à la parade'),
('AT', NULL,   '',     'Attaque',      NULL),
('CHA',        NULL,   'représente l\'apparence du héros et son “aura”, prestigieuse ou non... Permet d\'influencer\nles gens, de les convaincre, et d\'une manière générale d\'influencer les dieux. C\'est la caractéristique\nprincipale des prêtres et des ménestrels.',   'Charisme',     NULL),
('COU',        NULL,   'utilisé principalement pour déterminer qui frappe le premier dans un combat au corps à\ncorps, on s\'en sert aussi pour gérer les réactions des héros (ou des monstres) à une situation stressante. Si le\nhéros se retrouve brusquement cerné par 3 orques, le MJ peut lui demander de passer une épreuve de\ncourage. La partie continue normalement s\'il réussit, mais en cas d\'échec il paniquera et tentera de\nprendre la fuite, ou bien il se mettra à pleurer',      'Courage',      NULL),
('CRIT',       NULL,   'donne 1 chance supplémentaire d\'obtenir un critique en combat sur 1D20, en épreuve AT ou PRD',        'Critiques',    NULL),
('CRIT-AT',    NULL,   'Donne 1 chance supplémentaire d\'obtenir un critique en combat sur 1D20, en épreuve AT',       'Critiques AT', NULL),
('CRIT-MAG',   NULL,   'Donne 1 chance supplémentaire d\'obtenir un critique en combat sur 1D20, sur le lancement des sorts',  'Critique magique',     NULL),
('CRIT-PROD',  NULL,   'Donne 1 chance supplémentaire d\'obtenir un critique en combat sur 1D20, sur les prodige',     'Critique prodige',     NULL),
('EA', NULL,   'cette énergie est utilisée par les mages/prêtres/paladins pour lancer des sorts et\nappeler des prodiges. Chaque action de ce type a un coût en PA (points astraux), en général dépendant de\nla puissance du sort. Les points dépensés sont retirés du capital de PA, en attendant d\'être récupérés soit\npar le repos, soit par l\'étude, soit par le transfert ou par l\'usage d\'une potion.',    'Énergie astrale',      NULL),
('ESQ',        NULL,   '',     'Esquive',      NULL),
('EV', NULL,   '',     'Énergie vitale',       NULL),
('FO', 'Pour chaque point de FORCE supérieur à 12 : +1 point d’impact (dégâts des armes améliorés, au corps à corps ou à distance)\nLe bonus au dégâts sera donc de +1 pour FO 13, et de +3 pour FO 16, etc.', 'inutile de faire un dessin... La Force est utile en combat (elle détermine les dégâts entre autres)\nmais pas prépondérante : c\'est surtout l\'adresse qui est utile, car elle permet d\'augmenter les chances de\ntoucher. Cela dit, la force permet aux héros de pousser quelqu\'un du haut d\'une falaise, de défoncer une\nporte, de soulever un rocher, d\'utiliser des armes de bourrin, de transporter des charges.',  'Force',        'au contraire sur une carac. de FORCE de 8 ou inférieure : -1 point d’impact (dégâts des armes diminués, car mauviette)\nContrairement au bonus, le malus ne se cumule pas, car on considère que l\'arme de base, même maniée faiblement, peut\nblesser.'),
('INT',        'Pour chaque point d\'INTELLIGENCE supérieur à 12 : +1 point de dégâts des sorts (selon sortilège)\nLe bonus au dégâts sera donc de +1 pour INT 13, et de +3 pour INT 16, etc.\nIl s\'applique à chaque jet de dégât de sortilège : s\'il y a plusieurs cibles, il s\'appliquera donc à chaque cible',  'on teste l\'intelligence en particulier quand on pratique la magie... C\'est la\ncaractéristique qui sert à lancer la plupart des sortilèges ! Elle peut augmenter les dégâts des sorts. Elle\npermet aussi de réussir les potions... Parfois le MJ peut demander un test d\'intelligence pour sortir les\naventuriers d\'une situation. On s\'en sert aussi pour détecter les pièges, trouver son chemin dans un\nlabyrinthe...',     'Intelligence', NULL),
('MV', NULL,   '',     'Mouvement',    NULL),
('PI', NULL,   '',     'Points d\'impacts',    NULL),
('PR', NULL,   '',     'Protection',   NULL),
('PRD',        NULL,   '',     'Parade',       NULL),
('PR_MAGIC',   NULL,   '',     'PR_MAGIC',     NULL),
('RESM',       NULL,   'Dans certains cas, on peut tenter de résister à un sortilège – c\'est une\nmoyenne de plusieurs caractéristiques (COU, INT, FO) qui détermine la volonté du héros, sa capacité à ne\npas flancher face à une attaque magique. Les projectiles magiques (type boule de feu) ne sont pas remis en\nquestion par la résistance à la magie, car une fois lancés ils agissent comme des attaques physiques. En\nrevanche, de nombreux sorts plus élaborés (tels que les attaques mentales, les illusions, les malédictions)\npeuvent être évités (par les héros, mais aussi par les PNJs et les monstres) en réussissant une épreuve de\nrésistance à la magie. Dans tous les tableaux de magie, les sorts concernés par la résistance à la magie\ncomportent la mention « sauf résistance magie » dans la colonne « notes ». Utiliser cette caractéristique à\nbon escient permet d\'éviter que les mages ne deviennent des « tueurs de boss » en utilisant des sortilèges\nstupides sur des monstres ou des ennemis puissants. Ces derniers en effet ont souvent une bonne\nrésistance à la magie ! Important : En augmentant une ou plusieurs caractéristiques impliquées dans cette\nmoyenne, le héros devra recalculer sa résistance à la magie.',    'Résistance à la magie', NULL);


CREATE TABLE `users` (
                         `id` bigint(20) NOT NULL AUTO_INCREMENT,
                         `admin` bit(1) NOT NULL DEFAULT b'0',
                         `displayname` varchar(255) DEFAULT NULL,
                         `fbid` varchar(255) DEFAULT NULL,
                         `password` varchar(255) DEFAULT NULL,
                         `username` varchar(255) DEFAULT NULL,
                         `googleid` varchar(255) DEFAULT NULL,
                         `twitterid` varchar(255) DEFAULT NULL,
                         `liveid` varchar(255) DEFAULT NULL,
                         `activationCode` varchar(128) DEFAULT NULL,
                         `showInSearchUntil` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
                         PRIMARY KEY (`id`),
                         UNIQUE KEY `IX_user_username` (`username`),
                         UNIQUE KEY `IX_users_fbId` (`fbid`),
                         UNIQUE KEY `IX_users_googleId` (`googleid`),
                         UNIQUE KEY `IX_users_twitterId` (`twitterid`),
                         UNIQUE KEY `IX_users_liveId` (`liveid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `user_access_token` (
                                     `id` char(36) NOT NULL,
                                     `userId` bigint(20) NOT NULL,
                                     `name` varchar(255) NOT NULL,
                                     `dateCreated` datetime NOT NULL,
                                     `key` varchar(255) NOT NULL,
                                     PRIMARY KEY (`id`),
                                     KEY `FK_user_access_token_users` (`userId`),
                                     KEY `IX_user_access_token_key` (`key`),
                                     CONSTRAINT `FK_user_access_token_users` FOREIGN KEY (`userId`) REFERENCES `users` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;


CREATE TABLE `user_sessions` (
                                 `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                 `expire` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
                                 `ip` varchar(255) DEFAULT NULL,
                                 `key` varchar(255) NOT NULL,
                                 `start` timestamp NOT NULL DEFAULT current_timestamp(),
                                 `userid` bigint(20) NOT NULL,
                                 PRIMARY KEY (`id`),
                                 UNIQUE KEY `IX_user_session_key` (`key`),
                                 KEY `IX_user_session_userid` (`userid`),
                                 CONSTRAINT `FK_user_session_user_userid` FOREIGN KEY (`userid`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

-- 2025-04-03 06:50:34
