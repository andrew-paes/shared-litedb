using System;
using System.Collections.Generic;

namespace LiteDB
{
    public sealed partial class LiteCollection<T>
    {
        private LazyLoad<LiteEngine> _engine;
        private BsonMapper _mapper;
        private Logger _log;
        private List<string> _includes;
        private MemberMapper _id = null;
        private BsonType _autoId = BsonType.Null;
        private object thisLock = new object();

        /// <summary>
        /// Get collection name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Returns visitor resolver query only for internals implementations
        /// </summary>
        internal QueryVisitor<T> Visitor { get; private set; }

        public LiteCollection(string name, LazyLoad<LiteEngine> engine, BsonMapper mapper, Logger log)
        {
            Name = name ?? mapper.ResolveCollectionName(typeof(T));
            _engine = engine;
            _mapper = mapper;
            _log = log;
            Visitor = new QueryVisitor<T>(mapper);
            _includes = new List<string>();

            // if strong typed collection, get _id member mapped (if exists)
            if (typeof(T) != typeof(BsonDocument))
            {
                var entity = mapper.GetEntityMapper(typeof(T));
                _id = entity.Id;

                if (_id != null && _id.AutoId)
                {
                    _autoId =
                        _id.DataType == typeof(ObjectId) ? BsonType.ObjectId :
                        _id.DataType == typeof(Guid) ? BsonType.Guid :
                        _id.DataType == typeof(DateTime) ? BsonType.DateTime :
                        _id.DataType == typeof(int) ? BsonType.Int32 :
                        _id.DataType == typeof(long) ? BsonType.Int64 :
                        BsonType.Null;
                }
            }
            else
            {
                _autoId = BsonType.ObjectId;
            }
        }
    }
}