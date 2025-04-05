#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;

namespace Naheulbook.TestUtils;

[PublicAPI]
public partial class TestDataUtil : IDisposable
{
    private readonly List<object> _allEntities = [];
    private readonly NaheulbookDbContext _dbContext;
    private readonly DbContextOptions<NaheulbookDbContext> _dbContextOptions;

    public TestDataUtil(
        DbContextOptions<NaheulbookDbContext> dbContextOptions
    )
    {
        _dbContextOptions = dbContextOptions;
        _dbContext = CreateDbContext();
    }

    public NaheulbookDbContext CreateDbContext() => new(_dbContextOptions);

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

    public T? GetLastIfExists<T>()
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

    private TestDataUtil SaveEntity<T>(T entity, Action<T>? customizer)
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
                "DELETE FROM `calendars`;" +
                "DELETE FROM `characters`;" +
                "DELETE FROM `character_history_entries`;" +
                "DELETE FROM `character_jobs`;" +
                "DELETE FROM `character_modifiers`;" +
                "DELETE FROM `character_modifier_values`;" +
                "DELETE FROM `character_skills`;" +
                "DELETE FROM `character_specialities`;" +
                "DELETE FROM `effects`;" +
                "DELETE FROM `effect_subcategories`;" +
                "DELETE FROM `effect_modifiers`;" +
                "DELETE FROM `effect_types`;" +
                "DELETE FROM `events`;" +
                "DELETE FROM `gods`;" +
                "DELETE FROM `groups`;" +
                "DELETE FROM `group_history_entries`;" +
                "DELETE FROM `group_invitations`;" +
                "DELETE FROM `items`;" +
                "DELETE FROM `item_templates`;" +
                "DELETE FROM `item_template_subcategories`;" +
                "DELETE FROM `item_template_modifiers`;" +
                "DELETE FROM `item_template_requirements`;" +
                "DELETE FROM `item_template_sections`;" +
                "DELETE FROM `item_template_skills`;" +
                "DELETE FROM `item_template_skill_modifiers`;" +
                "DELETE FROM `item_template_slots`;" +
                "DELETE FROM `item_template_unskills`;" +
                "DELETE FROM `item_types`;" +
                "DELETE FROM `jobs`;" +
                "DELETE FROM `job_bonuses`;" +
                "DELETE FROM `job_requirements`;" +
                "DELETE FROM `job_restrictions`;" +
                "DELETE FROM `job_skills`;" +
                "DELETE FROM `loots`;" +
                "DELETE FROM `maps`;" +
                "DELETE FROM `map_marker_links`;" +
                "DELETE FROM `map_markers`;" +
                "DELETE FROM `map_layers`;" +
                "DELETE FROM `monsters`;" +
                "DELETE FROM `monster_subcategories`;" +
                "DELETE FROM `monster_templates`;" +
                "DELETE FROM `monster_template_inventory_elements`;" +
                "DELETE FROM `monster_traits`;" +
                "DELETE FROM `monster_types`;" +
                "DELETE FROM `origins`;" +
                "DELETE FROM `origin_bonuses`;" +
                "DELETE FROM `origin_information`;" +
                "DELETE FROM `origin_requirements`;" +
                "DELETE FROM `origin_restrictions`;" +
                "DELETE FROM `origin_skills`;" +
                // "DELETE FROM `quests`;" +
                // "DELETE FROM `quest_templates`;" +
                "DELETE FROM `skill_effects`;" +
                "DELETE FROM `skills`;" +
                "DELETE FROM `slots`;" +
                "DELETE FROM `specialities`;" +
                "DELETE FROM `speciality_modifiers`;" +
                "DELETE FROM `speciality_specials`;" +
                // "DELETE FROM `spells`;" +
                // "DELETE FROM `spell_categories`;" +
                "DELETE FROM `stats`;" +
                "DELETE FROM `users`;" +
                // "DELETE FROM `user_sessions`;" +
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

    public object? GetByTypeName(string typeName)
    {
        return _allEntities.FirstOrDefault(x => x.GetType().Name == typeName) ?? _allEntities.FirstOrDefault(x => x.GetType().Name == typeName + "Entity");
    }

    public IList<object> GetAllByTypeName(string typeName)
    {
        return _allEntities.Where(x => x.GetType().Name == typeName || x.GetType().Name == typeName + "Entity").ToList();
    }

    public void AddStaticObject(object obj)
    {
        _allEntities.Add(obj);
    }
}