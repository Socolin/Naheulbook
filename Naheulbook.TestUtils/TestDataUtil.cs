using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil : IDisposable
    {
        private readonly List<object> _allEntities;
        private readonly DefaultEntityCreator _defaultEntityCreator;
        private readonly NaheulbookDbContext _dbContext;

        public TestDataUtil(DbContextOptions<NaheulbookDbContext> dbContextOptions, DefaultEntityCreator defaultEntityCreator)
        {
            _defaultEntityCreator = defaultEntityCreator;
            _allEntities = new List<object>();
            _dbContext = new NaheulbookDbContext(dbContextOptions);
        }

        public T Get<T>()
        {
            return _allEntities.OfType<T>().Single();
        }

        public bool Contains<T>()
        {
            return _allEntities.OfType<T>().Any();
        }

        public T GetLast<T>()
        {
            var last = _allEntities.OfType<T>().LastOrDefault();
            if (last == null)
                throw new Exception($"Enable to find a element of type {typeof(T).Name} in the test data. Did you forget to call Add{typeof(T).Name}() before ?");
            return last;
        }

        public T GetLastIfExists<T>()
        {
            return _allEntities.OfType<T>().LastOrDefault();
        }

        public T GetFromEnd<T>(int index)
        {
            return _allEntities.OfType<T>().Reverse().ElementAt(index);
        }

        public List<T> GetAll<T>()
        {
            return _allEntities.OfType<T>().ToList();
        }

        public T Get<T>(int idx)
        {
            return _allEntities.OfType<T>().ElementAt(idx);
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        private TestDataUtil SaveEntity<T>(T entity, Action<T> customizer)
            where T : class
        {
            customizer?.Invoke(entity);

            _dbContext.Add(entity);
            _allEntities.Add(entity);
            _dbContext.SaveChanges();
            return this;
        }

        public TestDataUtil Cleanup()
        {
            var dbConnection = _dbContext.Database.GetDbConnection();
            dbConnection.Open();
            using (var cleanupSqlCommand = dbConnection.CreateCommand())
            {
                cleanupSqlCommand.CommandText =
                    "SET FOREIGN_KEY_CHECKS=0;" +
                    "DELETE FROM `calendar`;" +
                    "DELETE FROM `character`;" +
                    "DELETE FROM `character_history`;" +
                    "DELETE FROM `character_job`;" +
                    "DELETE FROM `character_modifier`;" +
                    "DELETE FROM `character_modifier_value`;" +
                    "DELETE FROM `character_skills`;" +
                    "DELETE FROM `character_speciality`;" +
                    "DELETE FROM `effect`;" +
                    "DELETE FROM `effect_category`;" +
                    "DELETE FROM `effect_modifier`;" +
                    "DELETE FROM `effect_type`;" +
                    // "DELETE FROM `error_report`;" +
                    "DELETE FROM `event`;" +
                    "DELETE FROM `god`;" +
                    "DELETE FROM `group`;" +
                    "DELETE FROM `group_history`;" +
                    "DELETE FROM `group_invitations`;" +
                    "DELETE FROM `icon`;" +
                    "DELETE FROM `item`;" +
                    "DELETE FROM `item_slot`;" +
                    "DELETE FROM `item_template`;" +
                    "DELETE FROM `item_template_category`;" +
                    "DELETE FROM `item_template_modifier`;" +
                    "DELETE FROM `item_template_requirement`;" +
                    "DELETE FROM `item_template_section`;" +
                    "DELETE FROM `item_template_skill`;" +
                    "DELETE FROM `item_template_skill_modifiers`;" +
                    "DELETE FROM `item_template_slot`;" +
                    "DELETE FROM `item_template_unskill`;" +
                    "DELETE FROM `item_type`;" +
                    "DELETE FROM `job`;" +
                    "DELETE FROM `job_bonus`;" +
                    "DELETE FROM `job_origin_blacklist`;" +
                    "DELETE FROM `job_origin_whitelist`;" +
                    "DELETE FROM `job_requirement`;" +
                    "DELETE FROM `job_restrict`;" +
                    "DELETE FROM `job_skill`;" +
                    "DELETE FROM `location`;" +
                    "DELETE FROM `location_map`;" +
                    "DELETE FROM `loot`;" +
                    "DELETE FROM `monster`;" +
                    "DELETE FROM `monster_category`;" +
                    "DELETE FROM `monster_location`;" +
                    "DELETE FROM `monster_template`;" +
                    "DELETE FROM `monster_template_simple_inventory`;" +
                    "DELETE FROM `monster_trait`;" +
                    "DELETE FROM `monster_type`;" +
                    "DELETE FROM `origin`;" +
                    "DELETE FROM `origin_bonus`;" +
                    "DELETE FROM `origin_info`;" +
                    "DELETE FROM `origin_requirement`;" +
                    "DELETE FROM `origin_restrict`;" +
                    "DELETE FROM `origin_skill`;" +
                    // "DELETE FROM `quest`;" +
                    // "DELETE FROM `quest_template`;" +
                    "DELETE FROM `skill`;" +
                    "DELETE FROM `skill_effect`;" +
                    "DELETE FROM `speciality`;" +
                    "DELETE FROM `speciality_modifier`;" +
                    "DELETE FROM `speciality_special`;" +
                    // "DELETE FROM `spell`;" +
                    // "DELETE FROM `spell_category`;" +
                    "DELETE FROM `stat`;" +
                    "DELETE FROM `user`;" +
                    // "DELETE FROM `user_session`;" +
                    "SET FOREIGN_KEY_CHECKS=1;";
                cleanupSqlCommand.ExecuteNonQuery();
            }
            dbConnection.Close();

            return this;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public object GetByTypeName(string typeName)
        {
            return _allEntities.FirstOrDefault(x => x.GetType().Name == typeName);
        }

        public IList<object> GetAllByTypeName(string typeName)
        {
            return _allEntities.Where(x => x.GetType().Name == typeName).ToList();
        }
    }
}