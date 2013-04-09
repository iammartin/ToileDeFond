using System;

namespace ToileDeFond.ContentManagement
{
    public abstract class Entity<T> : IEquatable<T> where T : Entity<T>
    {
        protected Entity(Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
        }

        public Guid Id { get; protected internal set; }

        public virtual bool Equals(T other)
        {
            return other.Id.Equals(Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(T)) return false;
            return Equals((T)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}