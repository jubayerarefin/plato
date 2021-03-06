﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using PlatoCore.Abstractions;
using PlatoCore.Abstractions.Extensions;

namespace Plato.Categories.Models
{

    public class CategoryBase :
        IComparable<ICategory>,
        ICategory
    {

        private readonly ConcurrentDictionary<Type, ISerializable> _metaData;
        
        public int Id { get; set; }

        public int ParentId { get; set; }

        public int FeatureId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Alias { get; set; }

        public string IconCss { get; set; }

        public string ForeColor { get; set; }

        public string BackColor { get; set; }

        public int SortOrder { get; set; }

        public int CreatedUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }
        
        public int ModifiedUserId { get; set; }
        
        public DateTimeOffset? ModifiedDate { get; set; }
        
        public IEnumerable<CategoryData> Data { get; set; } = new List<CategoryData>();

        public IDictionary<Type, ISerializable> MetaData => _metaData;

        public IEnumerable<ICategory> Children { get; set; } = new List<ICategory>();

        public ICategory Parent { get; set; }

        public int Depth { get; set; }

        public CategoryBase()
        {
            _metaData = new ConcurrentDictionary<Type, ISerializable>();
        }

        public void AddOrUpdate<T>(T obj) where T : class
        {
            if (_metaData.ContainsKey(typeof(T)))
            {
                _metaData.TryUpdate(typeof(T), (ISerializable)obj, _metaData[typeof(T)]);
            }
            else
            {
                _metaData.TryAdd(typeof(T), (ISerializable)obj);
            }
        }

        public void AddOrUpdate(Type type, ISerializable obj)
        {
            if (_metaData.ContainsKey(type))
            {
                _metaData.TryUpdate(type, (ISerializable)obj, _metaData[type]);
            }
            else
            {
                _metaData.TryAdd(type, obj);
            }
        }

        public T GetOrCreate<T>() where T : class
        {
            if (_metaData.ContainsKey(typeof(T)))
            {
                return (T)_metaData[typeof(T)];
            }

            return ActivateInstanceOf<T>.Instance();

        }
        
        public void PopulateModel(IDataReader dr)
        {
            
            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("ParentId"))
                ParentId = Convert.ToInt32(dr["ParentId"]);

            if (dr.ColumnIsNotNull("FeatureId"))
                FeatureId = Convert.ToInt32(dr["FeatureId"]);

            if (dr.ColumnIsNotNull("Name"))
                Name = Convert.ToString(dr["Name"]);

            if (dr.ColumnIsNotNull("Description"))
                Description = Convert.ToString(dr["Description"]);

            if (dr.ColumnIsNotNull("Alias"))
                Alias = Convert.ToString(dr["Alias"]);

            if (dr.ColumnIsNotNull("IconCss"))
                IconCss = Convert.ToString(dr["IconCss"]);

            if (dr.ColumnIsNotNull("ForeColor"))
                ForeColor = Convert.ToString(dr["ForeColor"]);

            if (dr.ColumnIsNotNull("BackColor"))
                BackColor = Convert.ToString(dr["BackColor"]);

            if (dr.ColumnIsNotNull("SortOrder"))
                SortOrder = Convert.ToInt32(dr["SortOrder"]);

            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = (DateTimeOffset)dr["CreatedDate"];

            if (dr.ColumnIsNotNull("ModifiedUserId"))
                ModifiedUserId = Convert.ToInt32(dr["ModifiedUserId"]);

            if (dr.ColumnIsNotNull("ModifiedDate"))
                ModifiedDate = (DateTimeOffset)dr["ModifiedDate"];
        }

        public int CompareTo(ICategory other)
        {
            if (other == null)
                return 1;
            var sortOrderCompare = other.SortOrder;
            if (this.SortOrder == sortOrderCompare)
                return 0;
            if (this.SortOrder < sortOrderCompare)
                return -1;
            if (this.SortOrder > sortOrderCompare)
                return 1;
            return 0;
        }

    }

}
